using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace trails
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Parsing> list = new List<Parsing>();
        string[] source;
        List<string> lines;
        int currentNumber;
        Parsing current;

        //List<string> sprPokas = new List<string>();
        List<string> sprGorod = new List<string>();
        //List<string> sprCountry = new List<string>();
        List<string> sprPhase = new List<string>();
        List<string> sprVid = new List<string>();
        Filter filter;
        List<DoneXML> doneXML = new List<DoneXML>(); // список сделанных

        // справочние соответствия городов
        GorodBase sprGorodBase = new GorodBase();

        // справочние соответствия показаний
        PokasBase sprPokasBase = new PokasBase();

        // справочние соответствия показаний
        CountrySponsor sprCountrySponsor = new CountrySponsor();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void menuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void menuOpen_Click(object sender, RoutedEventArgs e)
        {
            // сначала обновим справочники
            updateSprav();

            // потом загрузим сами данные
            if (!loadParsing())
            {
                MessageBox.Show("ошибка загрузки файла парсинга");
                return;
            }

            //MessageBox.Show("файл парсинга успешно загружен");

            spisokWindow sw = new spisokWindow(list, filter, doneXML);
            sw.ShowDialog();

            leftPanel.spisok = getIncluded(list);  // выбираем только отмеченых
            leftPanel.showParsing(0);
            leftPanel.ItemChanged += leftPanel_ItemChanged;

            if ((leftPanel.spisok == null) || (leftPanel.spisok.Count == 0))
            {
                lbStatus.Text = "нет записей для обработки";
                return;
            }

            Parsing prs = leftPanel.spisok[0];
            lbRki.Content = string.Format("РКИ № {0} ({1})", prs.Nomer, prs.Data);
            lbStatus.Text = string.Format("запись {0} из {1}", 1, leftPanel.spisok.Count);
            current = prs;
        }

        private bool loadParsing()
        {
            string filename = "";
            if (list != null)
            {
                list.Clear();
            }

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".prs";
            dlg.Filter = "Файлы парсинга (.prs)|*.prs|Все файлы|*.*";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                filename = dlg.FileName;
            }
            else
            {
                MessageBox.Show("файл парсинга не загружен");
                return false;
            }

            lbStatus.Text = "загрузка файла парсинга...";
            list = OpenFile(filename);
            if (list == null)
            {
                MessageBox.Show("не могу обработать фильтр");
                return false;
            }


            return true;
        }

        // обновить справочники
        private void updateSprav()
        {
            lbStatus.Text = "загрузка справочников...";
            try
            {
                //sprPokas = loadSprav("pokas.spr");
                //sprGorod = loadSprav("gorod.spr");
                //sprCountry = loadSprav("country.spr");
                sprPhase = loadSprav("phase.spr");
                sprVid = loadSprav("vid.spr");
                sprGorodBase.Raw = loadSprav("gorod-base.spr");
                sprPokasBase.Raw = loadSprav("pokas-base.spr");
                sprCountrySponsor.Raw = loadSprav("country-sponsors.spr");

                sprGorodBase.Prepare(); // обязательно !
                sprPokasBase.Prepare(); // обязательно !
                sprCountrySponsor.Prepare(); // обязательно !
            }
            catch (Exception)
            {
                MessageBox.Show("ошибка загрузки справочников");
                //throw;
            }
            lbStatus.Text = "загружено 7 справочников";
        }

        // выбираем только те исследования,
        // которые включаются.
        private List<Parsing> getIncluded(List<Parsing> list)
        {
            List<Parsing> newlist = new List<Parsing>();
            foreach (Parsing p in list)
            {
                if (p.Included)
                {
                    newlist.Add(p);
                }
            }
            return newlist;
        }

        private List<string> loadSprav(string filename)
        {
            List<string> spr = new List<string>();
            source = File.ReadAllLines(filename);
            foreach (string s in source)
            {
                spr.Add(s);
            }
            return spr;
        }

        void leftPanel_ItemChanged(object sender, RkiEventArgs e)
        {
            Parsing prs = e.Item;
            lbRki.Content = string.Format("РКИ № {0} ({1})", prs.Nomer, prs.Data);
            lbStatus.Text = string.Format("запись {0} из {1}", e.Current + 1, leftPanel.spisok.Count);
            currentNumber = e.Current;
            current = prs;
        }

        private List<Parsing> OpenFile(string filename)
        {
            FilterWindow fw = new FilterWindow(filename, "done");
            fw.ShowDialog();
            if (fw.DialogResult != true)
            {
                return null; // пустой список
            }

            filter = fw.filter;
            
            doneXML = loadDoneXML(fw.filter.DoneDir);
            //showFilter(fw.filter, doneXML);

            // читаем весь файл за раз и помещаем в массив строк
            source = File.ReadAllLines(filename);

            // немного почистим список
            lines = checkLines();

            int n = 0;

            // пропускаем начало файла до значений
            while (lines[n].StartsWith("=") == false)
            {
                n++;
            }

            // обработка данных
            while (n != -1)
            {
                n = getNextItem(n);
            }

            current = list[0];
            return list;
        }

        private List<DoneXML> loadDoneXML(string dir)
        {
            List<DoneXML> dn = new List<DoneXML>();

            // читаем каждый XML файл в каталоге dir
            // и загружаем в списко уже сделланных исследований
            
            // сначала получим список файлов
            string[] files = Directory.GetFiles(dir, "*.xml");
            foreach (string fn in files)
            {
                // пример имени файла xml 2015-02-05 701.xml
                Regex regex = new Regex(@"xml \d{4}-\d{2}-\d{2} (\d+).xml");
                foreach (Match match in regex.Matches(fn))
                {
                    string nom = match.Groups[1].Value;
                    DoneXML d = new DoneXML();
                    d.Nomer = int.Parse(nom);
                    dn.Add(d);
                } 
            }

            return dn;
        }


        private void showFilter(Filter f, List<DoneXML> dn)
        {
            if (dn == null || dn.Count == 0)
            {
                MessageBox.Show("Ошибка: пустой список уже обработаннх файлов");
                return;
            }

            string msg = "фильтр: ";
//            msg = string.Format("дата 1 = {0}, дата 2 = {1}, пациентов = {2}",
//                f.StartDate, f.EndDate, f.MinPacient);
            foreach (DoneXML d in dn)
            {
                msg += d.Nomer.ToString() + ",";
            }
            MessageBox.Show(msg);
        }

        // признаком окончания данных является -1
        private int getNextItem(int startPos)
        {
            int n = startPos;
            string preparat;
            string nomer;
            string data;
            string protokol;
            string producer;
            string pacient;
            string address;
            string forma = "";
            string duration;
            List<string> bases = new List<string>();

            Parsing prs = new Parsing();

            n++;
            string ss = lines[n];

            preparat = lines[n].Substring(27); // препарат
            n++;

            producer = lines[n].Substring(27); // фирма-производитель
            n++;

            nomer = lines[n].Substring(27); // номер исследования
            n++;

            data = lines[n].Substring(27); // дата исследование
            n++;

            protokol = lines[n].Substring(27); // протокол
            protokol = protokol.Trim().Replace("\"", "");

            n++;
            pacient = lines[n].Substring(27);

            n++;
            address = lines[n].Substring(27);

            n++;
            duration = lines[n].Substring(27);

            n++;
            // обработка баз
            while (lines[n].StartsWith("=") == false)
            {
                if (lines[n].Substring(12, 5) == "форма")
                {
                    forma = lines[n].Substring(21);
                }
                else
                {
                    bases.Add(lines[n].Substring(21));
                }

                n++;
                if (n == lines.Count)
                {
                    n = -1;  // конец обработки
                    break;
                }
            }

            prs.Included = true;
            prs.Nomer = nomer;
            prs.Data = data;
            prs.Preparat = preparat;
            prs.Producer = producer;
            prs.Protokol = protokol;
            prs.Pacient = pacient;
            prs.Address = address;
            prs.Duration = duration;
            prs.Forma = forma;
            prs.Bases = bases;
            list.Add(prs);

            return n;

        }

        private List<string> checkLines()
        {
            List<string> ln = new List<string>();

            for (int n = 0; n < source.Length; n++)
            {
                string s = source[n];
                if ((s != "") && (s.Substring(0, 1) == "=" || s.Substring(0, 1) == " "))
                {
                    ln.Add(s);
                }
            }
            return ln;
        }

        private void btGo_Click(object sender, RoutedEventArgs e)
        {
            process();
        }

        private void process()
        {
            // преобразование из формата парсинга в XML
            OutData data = new OutData();

            //Parsing current = list[currentNumber];

            data.Nomer = current.Nomer;
            data.NomerRas = int.Parse(current.Nomer);
            data.Preparat = current.Preparat;
            data.Forma = current.Forma;
            data.Producer = current.Producer;
            data.Country = current.Address;
            data.Protocol = current.Protokol;
            data.Pokas = current.Nomer;
            data.Duration = int.Parse(current.Duration);
            data.Vid = current.Nomer;
            data.Phase = current.Nomer;
            data.Pacient = int.Parse(current.Pacient);

            data.Bases = new List<string>();
            foreach (string s in current.Bases)
            {
                data.Bases.Add(s);
            }

            rightPanel.show(filter.Prefix, data, 
                sprPhase, sprVid, sprGorodBase, sprPokasBase, sprCountrySponsor);
        }

        private void menuSave_Checked(object sender, RoutedEventArgs e)
        {
        }

        private void merge()
        {
            string[] files;
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.DefaultExt = ".xml";
            dlg.Filter = "Файлы импорта(.xml)|*.xml|Все файлы|*.*";
            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                files = dlg.FileNames;
                List<string> dst = mergefiles(files);
                savedst(dst);
            }
            else
            {
                return;
            }
        }

        private List<string> mergefiles(string[] files)
        {
            string[] src;
            List<string> dst = new List<string>();
            dst.Add("<Worksheet >\n");  // на весь файл

            foreach (string filename in files)
            {
                // читаем весь файл за раз и помещаем в массив строк
                src = File.ReadAllLines(filename);
                for (int i = 1; i < src.Length - 2; i++)
                {
                    // внимание пропускаем первую и последнюю строчку!
                    string s = src[i];
                    dst.Add(s);
                }
            }

            dst.Add("</Worksheet >\n");
            return dst;
        }

        private void savedst(List<string> dst)
        {
            string dt = DateTime.Now.ToString("yyyy-MM-dd");
            string filename = String.Format("импорт {0}.xml", dt); // ??
            string content = string.Join("\n", dst.ToArray());
            Encoding outputEnc = new UTF8Encoding(false); // create encoding with no BOM
            File.WriteAllText(filename, content, outputEnc);
            string msg = String.Format("файл \"{0}\" успешно записан", filename);
            MessageBox.Show(msg);
        }

        private void menuSave_Click(object sender, RoutedEventArgs e)
        {
            // слить подготовленные xml-файлы
            merge();
        }

        private void menuSpravOpen_Click(object sender, RoutedEventArgs e)
        {
            updateSprav();
            MessageBox.Show("Справочники обновлены");
        }
    }
}
