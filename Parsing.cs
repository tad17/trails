using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace trails
{
    public class Base
    {
        public string OriginalAddress { get; set; } //из парсинга
        public string Gorod { get; set; } // соотв. город

        public Base(string orig, string gorod)
        {
            OriginalAddress = orig;
            Gorod = gorod;
        }
    }

    public class Parsing
    {
        // для отображения в таблице сделал дополнительные поля
        // начинаются с буквы t
        public bool Included { get; set; }  // включать ли в список XML
        public string Nomer { get; set; } // номер исследования

        public string Data { get; set; } // дата начала исследования 
        public DateTime tData
        {
            get
            {
                DateTime d;
                bool s = DateTime.TryParse(Data, out d);
                if (s)
                {
                    return d;
                }
                else
                {
                    //MessageBox.Show("ошибка преобразования даты: " + Data);
                    return DateTime.Now;
                }
            }
        }
        public string Pacient { get; set; } // кол-во пациентов

        // кол-во пациентов в цифрах
        public int tPacient
        {
            get
            {
                short nn;
                bool b = Int16.TryParse(Pacient, out nn);
                if (b)
                {
                    return Int16.Parse(Pacient);
                }
                else
                {
                    return 0;
                }
            }
        }

        public string Preparat { get; set; } // лекарственное средство
        public string Producer { get; set; } // фирма-производитель
        public string Protokol { get; set; } // цель исследования (протокол)
        public string Address { get; set; } // адрес??

        public string Duration { get; set; } // продолжительность исследования
        public int tDuration
        {
            get
            {
                short nn;
                bool b = Int16.TryParse(Duration, out nn);
                if (b)
                {
                    return Int16.Parse(Duration);
                }
                else
                {
                    return 0;
                }
            }
        }

        public string Forma { get; set; } // лекарственная форма
        public List<string> Bases { get; set; }    // клинические базы
    }
}
