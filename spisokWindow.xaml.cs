using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Shapes;

namespace trails
{
    // для правильного отображения даты
    public class MyDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo ture)
        {
            DateTime d = (DateTime)value; 
            return d.ToShortDateString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo ture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Логика взаимодействия для spisokWindow.xaml
    /// </summary>
    public partial class spisokWindow : Window
    {
        List<Parsing> list = new List<Parsing>();
        List<DoneXML> doneXml = new List<DoneXML>();
        Filter filter;

        public spisokWindow(List<Parsing> lp, Filter fl, List<DoneXML> dn)
        {
            InitializeComponent();
            list = lp;
            filter = fl;
            doneXml = dn;
            dataList.ItemsSource = list;
            
            InitializeGrid();
            
            applyFilter();
            updateStatusLabel();
        }

        private void applyFilter()
        {
            foreach (Parsing p in list)
            {
                // проверка на попадание в диапазон дат
                if ((p.tData > filter.EndDate) || (p.tData < filter.StartDate))
                {
                    p.Included = false;
                }

                // проверка на количество пациентов
                if (p.tPacient < filter.MinPacient)
                {
                    p.Included = false;
                }

                // проверка на уже обработанные
                foreach(DoneXML d in doneXml) {
                    if ((p.Nomer == "") || (p.Nomer == d.Nomer.ToString()))
                    {
                        p.Included = false;
                    }
                }
            }
        }

        private void updateStatusLabel()
        {
            int s1 = 0;
            int s2 = list.Count();

            foreach (Parsing p in list)
            {
                if (p.Included)
                {
                    s1 += 1;
                } 
            }

            string msg = string.Format("Выбрано {0} иссл. из {1}", s1, s2);
            lbSelected.Content = msg;
        }

        private void InitializeGrid()
        {
            // для переноса строк
            System.Windows.Style mystyle = new System.Windows.Style(typeof(TextBlock));
            mystyle.Setters.Add(new Setter(TextBlock.TextWrappingProperty, TextWrapping.Wrap));

            // формирует таблицу

            // ================== Included ===============================
            DataGridCheckBoxColumn col1 = new DataGridCheckBoxColumn();
            col1.Binding = new Binding("Included");
            col1.Header = "Вкл";
            //col1.Width = 40;
            dataList.Columns.Add(col1);

            // ================== Nomer ===============================
            DataGridTextColumn col2 = new DataGridTextColumn();
            col2.Binding = new Binding("Nomer");
            col2.Header = "№";
            //col2.Width = 50;
            dataList.Columns.Add(col2);

            // ================== Pacient ===============================
            DataGridTextColumn col3 = new DataGridTextColumn();
            col3.Binding = new Binding("tPacient");
            col3.Header = "Пцнт";
            col3.IsReadOnly = true;
            dataList.Columns.Add(col3);

            // ================== Data ===============================
            DataGridTextColumn col4 = new DataGridTextColumn();
            col4.Binding = new Binding("tData");
            col4.Binding.StringFormat = "dd.MM.yyyy";
            col4.Header = "Дата";
            col4.IsReadOnly = true;
            dataList.Columns.Add(col4);

            // ================== Preparat ===============================
            DataGridTextColumn col5 = new DataGridTextColumn();
            col5.Binding = new Binding("Preparat");
            col5.Header = "Препарат";
            col5.Width = 150;
            col5.ElementStyle = mystyle;
            col5.IsReadOnly = true;
            dataList.Columns.Add(col5);

            // ================== Producer ===============================
            DataGridTextColumn col6 = new DataGridTextColumn();
            col6.Binding = new Binding("Producer");
            col6.Header = "Фирма производитель";
            col6.Width = 200;
            col6.ElementStyle = mystyle;
            dataList.Columns.Add(col6);

            // ================== Protokol ===============================
            DataGridTextColumn col7 = new DataGridTextColumn();
            col7.Binding = new Binding("Protokol");
            col7.Header = "Пртокол исследований";
            col7.Width = 250;
            col7.ElementStyle = mystyle;
            dataList.Columns.Add(col7);

            // ================== Pacient ===============================
            DataGridTextColumn col8 = new DataGridTextColumn();
            col8.Binding = new Binding("tDuration");
            col8.Header = "Прод.";
            col8.IsReadOnly = true;
            dataList.Columns.Add(col8);

        }

        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void selectAll(bool cb)
        {
            //MessageBox.Show(cb.ToString());
            foreach (Parsing p in list)
            {
                p.Included = cb;
            }

            // передернем
            dataList.ItemsSource = null;
            dataList.ItemsSource = list;
            updateStatusLabel();
        }

        private void cbSelectAll_Click(object sender, RoutedEventArgs e)
        {
            if (cbSelectAll.IsChecked == true)
            {
                selectAll(true);
            }
            else
            {
                selectAll(false);
            }
        }

        private void btRecalc_Click(object sender, RoutedEventArgs e)
        {
            updateStatusLabel();
        }
    }
}
