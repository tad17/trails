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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace trails
{
    // для передачи сообщений
    public delegate void RkiEventHandler(object sender, RkiEventArgs e);

    public class RkiEventArgs : EventArgs
    {
        private Parsing parsing;
        private int current;

        public RkiEventArgs(Parsing parsing, int current)
        {
            this.parsing = parsing;
            this.current = current;
        }

        public Parsing Item
        {
            get { return parsing; }
        }

        public int Current
        {
            get { return current; }
        }
    }

    // для правильного выравнивания элементов
    public class MyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo ture)
        {
            // учитываются отступы и ширина скролбара
            return (int)((double)value - 32);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo ture)
        {
            throw new NotImplementedException();
        }
    }

    public class PageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo ture)
        {
            // учитываются высота кнопок >> и <<
            return (int)((double)value - 40);
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo ture)
        {
            throw new NotImplementedException();
        }
    }
    /// <summary>
    /// Логика взаимодействия для uc1.xaml
    /// </summary>
    public partial class uc1 : UserControl
    {
        public event RkiEventHandler ItemChanged;

        public List<Parsing> spisok;
        int current;
        
        private Parsing item;
        public Parsing Item // текущее РКИ
        {
            get
            {
                return item;
            }

            set
            {
                if (item != value)
                {
                    item = value;
                    OnItemChanged(new RkiEventArgs(value, current));
                }
            }
        }

        public uc1()
        {
            InitializeComponent();
        }

        protected virtual void OnItemChanged(RkiEventArgs e)
        {
            if (ItemChanged != null)
                ItemChanged(this, e);
        }
        
        public void showParsing(int start)
        {
            if ((spisok == null) || (spisok.Count == 0))
            {
                return;
            }

            Parsing parsing = spisok[start];
            current = start;

            lbNomer.Text = parsing.Nomer;
            lbData.Text = parsing.Data;
            lbPreparat.Text = parsing.Preparat;
            lbProducer.Text = parsing.Producer;
            tbProtokol.Text = parsing.Protokol;
            lbPacient.Text = parsing.Pacient;
            lbAddress.Text = parsing.Address;
            lbDuration.Text = parsing.Duration;
            lbForma.Text = parsing.Forma;

            // для ширины поля
            Binding bind = new Binding();
            bind.Source = this;
            bind.Path = new PropertyPath("ActualWidth");
            bind.Mode = BindingMode.OneWay;
            bind.Converter = new MyConverter();

            int n = 1;
            boxBases.Items.Clear();
            foreach (string b in parsing.Bases) {
                TextBlock tb;
                tb = new TextBlock();
                tb.TextWrapping = TextWrapping.Wrap;
                tb.SetBinding(TextBlock.WidthProperty, bind);
                tb.Margin = new Thickness(4);
                string nomer = string.Format("{0}. ", n);
                tb.Inlines.Add(new Bold(new Run(nomer)));
                tb.Inlines.Add(b);
                boxBases.Items.Add(tb);
                n++;
            }
        }

        private void btNext_Click(object sender, RoutedEventArgs e)
        {
            //след запись
            current++;
            if (current < spisok.Count())
            {
                Item = spisok[current];
                showParsing(current);
            }
            else
            {
                MessageBox.Show("записей больше нет");
            }
        }

        private void btPrev_Click(object sender, RoutedEventArgs e)
        {
            //пред запись
            if (current > 0)
            {
                current--;
                Item = spisok[current];
                showParsing(current);
            }
        }
    }
}
