using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    /// Логика взаимодействия для uc2.xaml
    /// </summary>
    public partial class uc2 : UserControl
    {
        List<RowDefinition> oldRow = new List<RowDefinition>();
        List<Label> oldLabel = new List<Label>();
        List<ComboBox> oldCombo = new List<ComboBox>();
        List<string> sprPokas = new List<string>();
        List<string> sprGorod = new List<string>();
        List<string> sprCountry = new List<string>();
        OutData Data;
        string Prefix = ""; 

        public uc2()
        {
            InitializeComponent();
        }

        public void show(string pref, OutData data,  
            List<string> sprPhase, List<string> sprVid, 
            GorodBase sprGorodBase, 
            PokasBase sprPokasBase, 
            CountrySponsor sprCountrySponsor)
        {

            Prefix = pref;
            Data = data;

            // заполним города из справочника
            sprGorod.Clear();
            foreach (string gorod in sprGorodBase.cities.Keys)
            {
                sprGorod.Add(gorod);
            }

            // заполним показания из справочника
            sprPokas.Clear();
            foreach (string p in sprPokasBase.pokas.Keys)
            {
                sprPokas.Add(p);
            }

            // заполним страны из справочника
            sprCountry.Clear();
            foreach (string p in sprCountrySponsor.country.Keys)
            {
                sprCountry.Add(p);
            }

            lbBases.Text = data.Bases.Count.ToString();
            lbCities.Text = "";
            cbCountry.ItemsSource = sprCountry;
            lbDuration.Text = data.Duration.ToString();
            lbForma.Text = data.Forma;
            lbNomer.Text = data.Nomer;
            lbNomerRas.Text = data.NomerRas.ToString();
            lbPacient.Text = data.Pacient.ToString();
            cbPhase.ItemsSource = sprPhase;
            cbPokas.ItemsSource = sprPokas;
            lbProducer.Text = data.Producer;
            lbProtocol.Text = data.Protocol;
            lbPreparat.Text = data.Preparat;
            cbVid.ItemsSource = sprVid;

            clearOldValue();

            int n = 1;
            foreach (string s in data.Bases)
            {
                addNewValue(sprGorod, n, sprGorodBase);
                n++;
            }

            checkPokas(sprPokas, sprPokasBase);
            checkCountry(Data.Producer, sprCountrySponsor); // проставим страну

            //cbCountry.SelectedIndex = 0;  // по умолчанию поставим Россию
            cbPhase.SelectedIndex = 2; // по умолчанию III фазу
        }

        private void checkCountry(string sponsor, CountrySponsor sprav)
        {
            int n = 0;
            foreach(string cntry in sprav.country.Keys) {
                List<string> spnsrs = sprav.country[cntry]; // список спонсоров
                foreach (string s in spnsrs) {
                    if ((s != "") && sponsor.Contains(s)) {
                        // есть соответствие, теперь найдем в
                        // другом справочнике и установим соответствующий индекс
                        for (int i = 0; i < sprCountry.Count; i++) {
                            if (sprCountry[i].Contains(cntry)) {
                                cbCountry.SelectedItem = cntry;
                                //cbCountry.SelectedIndex = n;
                                return;
                            }
                        }
                    }
                }
                n++;
            }
        }

        private void checkPokas(List<String> sprPokas, PokasBase sprPokasBase)
        {
            // попробуем установить правильные показания
            string protokol = Data.Protocol;

            for (int k = 0; k < sprPokasBase.BaseRaw.Count; k++)
            {
                if (protokol.Contains(sprPokasBase.BaseRaw[k].Trim()))
                {
                    // установим
                    cbPokas.SelectedIndex = k;
                }
            }
        }

        private void addNewValue(List<string> sprGorod, int n, GorodBase sprGorodBase)
        {
            // добавим строчки для каждого города
            var rowDefinition = new RowDefinition { Height = GridLength.Auto };
            dataGrid.RowDefinitions.Add(rowDefinition);
            oldRow.Add(rowDefinition);

            int lastRowIndex = dataGrid.RowDefinitions.Count - 1;

            Label label = new Label();
            label.Content = string.Format("Город {0}:", n);
            label.HorizontalAlignment = System.Windows.HorizontalAlignment.Right;
            label.Margin = new Thickness(0, -6, 10, 0);
            dataGrid.Children.Add(label);
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, lastRowIndex);
            oldLabel.Add(label);

            ComboBox comboElement = new ComboBox();
            comboElement.ItemsSource = sprGorod;
            dataGrid.Children.Add(comboElement);
            Grid.SetColumn(comboElement, 1);
            Grid.SetRow(comboElement, lastRowIndex);
            oldCombo.Add(comboElement);

            // сразу попробуем установить правильный город
            string b = Data.Bases[n-1];
            string g = checkGorodBase(b, sprGorodBase);
            // в справочнике соответствий нашли, теперь найдем в основном справочнике
            for (int k = 0; k < sprGorod.Count; k++)
            {
                if (sprGorod[k] == g)
                {
                    // установим
                    comboElement.SelectedIndex = k;
                }
            }
        }

        private string checkGorodBase(string b, GorodBase sprGorodBase)
        {
            return sprGorodBase.CheckBase(b);
        }

        private void clearOldValue()
        {
            // нужно удалить города от предыдущего РКИ!!!
            for (int i = 0; i < oldRow.Count; i++)
            {
                dataGrid.Children.Remove(oldLabel[i]);
                dataGrid.Children.Remove(oldCombo[i]);
                dataGrid.RowDefinitions.Remove(oldRow[i]);
            }
            oldRow.Clear();
            oldLabel.Clear();
            oldCombo.Clear();

            cbPhase.SelectedIndex = -1;
            cbPokas.SelectedIndex = -1;
            cbCountry.SelectedIndex = -1;
        }

        private void btXML_Click(object sender, RoutedEventArgs e)
        {
            generateXML();
        }

        private void generateXML()
        {
            string txt;
            tbXML.Text = "";
            tbXML.Inlines.Add(new Run("<Worksheet >\n"));
            tbXML.Inlines.Add(new Run("\t<Row>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            txt = lbNomer.Text.Trim();
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"Number\">"));
            txt = lbNomerRas.Text.Trim();
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            txt = lbPreparat.Text.Trim();
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            txt = lbForma.Text.Trim();
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            txt = lbProducer.Text.Trim();
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            if (cbCountry.SelectedItem != null) {
                txt = cbCountry.SelectedItem.ToString();
            }
            else
            {
                txt = "не определено";
            }
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            txt = lbProtocol.Text;
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            if (cbPokas.SelectedItem != null)
            {
                txt = cbPokas.SelectedItem.ToString();
            }
            else
            {
                txt = " не определено";
            }

            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"Number\">"));
            txt = lbDuration.Text;
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            if (cbVid.SelectedItem != null)
            {
                txt = cbVid.SelectedItem.ToString();
            }
            else
            {
                txt = "не определено";
            }
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            if (cbPhase.SelectedItem != null)
            {
                txt = cbPhase.SelectedItem.ToString();
            }
            else
            {
                txt = "не определено";
            }
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"Number\">"));
            txt = Data.Bases.Count.ToString();
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            // названия клинических баз
            string gorod = ",";
            foreach ( ComboBox cb in oldCombo) {
                txt = "не определено";
                if (cb.SelectedItem != null) {
                    txt = cb.SelectedItem.ToString();
                }
                gorod += txt + ",";
            }
            gorod = gorod.Substring(1);

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"String\">"));
            tbXML.Inlines.Add(new Bold(new Run(gorod)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Cell><Data ss:Type=\"Number\">"));
            txt = lbPacient.Text;
            tbXML.Inlines.Add(new Bold(new Run(txt)));
            tbXML.Inlines.Add(new Run("</Data><NamedCell/></Cell>\n"));

            tbXML.Inlines.Add(new Run("\t\t<Row><Cell ss:Index=\"12\"/></Row>\n"));
            foreach (ComboBox cb in oldCombo)
            {
                if (cb.SelectedItem != null) {
                    txt = cb.SelectedItem.ToString();
                    tbXML.Inlines.Add(new Run("\t\t<Row><Cell ss:Index=\"12\"><Data ss:Type=\"String\">"));
                    tbXML.Inlines.Add(new Bold(new Run(txt)));
                    tbXML.Inlines.Add(new Run("</Data></Cell></Row>\n"));
                }
            }
            tbXML.Inlines.Add(new Run("\t</Row>\n"));
            tbXML.Inlines.Add(new Run("</Worksheet >\n"));
        }

        private void btSave_Click(object sender, RoutedEventArgs e)
        {
            // сохранить XML файл
            SaveXML();
        }

        private void SaveXML()
        {
            string dt = DateTime.Now.ToString("yyyy-MM-dd");
            string nm = Data.Nomer;
            string filename = String.Format("xml {0} {1}.xml", dt, nm); // ??
            string content = getContent();
            Encoding outputEnc = new UTF8Encoding(false); // create encoding with no BOM
            File.WriteAllText(filename, content, outputEnc);
            string msg = String.Format("файл {0} успешно записан", filename);
            MessageBox.Show(msg);
        }

        private string getContent()
        {
            string s = "";
            string txt;

            s += "<Worksheet >\n";
            s += "\t<Row>\n";
            s += "\t\t<Cell><Data ss:Type=\"String\">";
            txt = lbNomer.Text.Trim();

            // добавим префикс года, чтобы избежать дублирования номеров
            string pr = Prefix.Trim();
            if (pr != "")
            {
                txt = pr + "-" + txt;
            }
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"Number\">";
            txt = lbNomerRas.Text.Trim();
            
            // преобразуем, чтобы избежать дублирования номеров
            pr = Prefix.Trim();
            if (( pr != "") && (pr.Length == 4))
            {
                // берем 2 последние цифры года
                string nm = pr.Substring(2);
                int gd = int.Parse(nm) * 1000;
                int rs = int.Parse(txt);
                int nomras = gd + rs;
                string snomras = nomras.ToString();
                txt = snomras;
            }
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            txt = lbPreparat.Text.Trim();
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            txt = lbForma.Text.Trim();
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            txt = lbProducer.Text.Trim();
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            if (cbCountry.SelectedItem != null)
            {
                txt = cbCountry.SelectedItem.ToString();
            }
            else
            {
                txt = "не определено";
            }
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            txt = lbProtocol.Text;
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            if (cbPokas.SelectedItem != null)
            {
                txt = cbPokas.SelectedItem.ToString();
            }
            else
            {
                txt = "не определено";
            }
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"Number\">";
            txt = lbDuration.Text;
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            if (cbVid.SelectedItem != null)
            {
                txt = cbVid.SelectedItem.ToString();
            }
            else
            {
                txt = "не определено";
            }
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            if (cbPhase.SelectedItem != null)
            {
                txt = cbPhase.SelectedItem.ToString();
            }
            else
            {
                txt = "не определено";
            }
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"Number\">";
            txt = Data.Bases.Count.ToString();
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";

            // названия клинических баз
            string gorod = ",";
            foreach (ComboBox cb in oldCombo)
            {
                txt = "";
                if (cb.SelectedItem != null)
                {
                    txt = cb.SelectedItem.ToString();
                    // выбираем только города, которые определены
                    // и без повторов
                    if (!gorod.Contains(txt))
                    {
                        gorod += txt + ",";
                    }
                }
            }
            gorod = gorod.Substring(1); // убираем первую запятую

            s += "\t\t<Cell><Data ss:Type=\"String\">";
            s += gorod;
            s += "</Data><NamedCell/></Cell>\n";

            s += "\t\t<Cell><Data ss:Type=\"Number\">";
            txt = lbPacient.Text;
            s += txt;
            s += "</Data><NamedCell/></Cell>\n";
            s += "\t</Row>\n";

            // обработка списка баз
            int n = 0;
            foreach (ComboBox cb in oldCombo)
            {
                if (cb.SelectedItem != null)
                {
                    s += "\t<Row><Cell ss:Index=\"12\"/></Row>\n";
                    txt = cb.SelectedItem.ToString();
                    s += "\t<Row><Cell ss:Index=\"12\"><Data ss:Type=\"String\">";
                    s += Data.Bases[n] + "," +  txt;
                    s += "</Data></Cell></Row>\n";
                    n++;
                }
            }
            s += "</Worksheet >\n";

            return s;
        }

        private void btMoskva_Click(object sender, RoutedEventArgs e)
        {
            setMoskva();
        }

        // Для неотмеченных городов установить Москва
        private void setMoskva()
        {
            foreach (ComboBox cb in oldCombo)
            {
                if (cb.SelectedItem == null)
                {
                    cb.SelectedIndex = 0;
                }
            }
        }

    }
}
