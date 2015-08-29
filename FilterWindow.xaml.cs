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
using System.Windows.Shapes;

namespace trails
{
    /// <summary>
    /// Логика взаимодействия для FilterWindow.xaml
    /// </summary>
    public partial class FilterWindow : Window
    {
        public Filter filter = new Filter();

        public FilterWindow(string filename, string dn)
        {
            InitializeComponent();

            filter.ParsingFile = filename;
            filter.DoneDir = System.IO.Path.Combine(Directory.GetCurrentDirectory(), dn);

            // для отображения
            tbParsingFile.Text = filter.ParsingFile;
            tbDoneDir.Text = filter.DoneDir;
            tbStartDate.Text = DateTime.Now.ToShortDateString();
            tbEndDate.Text = DateTime.Now.ToShortDateString();
            tbMinPacient.Text = "0";
            tbPrefix.Text = "2015";
        }

        private void btDone_Click(object sender, RoutedEventArgs e)
        {
            updateFilter();
            DialogResult = true;
            Close();
        }

        private void updateFilter()
        {
            filter.StartDate = DateTime.Parse(tbStartDate.Text);
            filter.EndDate = DateTime.Parse(tbEndDate.Text);
            filter.MinPacient = int.Parse(tbMinPacient.Text);
            filter.DoneDir = tbDoneDir.Text;
            filter.Prefix = tbPrefix.Text;
        }
    }
}
