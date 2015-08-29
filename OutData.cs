using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trails
{
    public class OutData
    {
        public string Nomer { get; set; }       // Вн. номер
        public int NomerRas { get; set; }       // Номер разработки
        public string Preparat { get; set; }    // Лекарственное средство
        public string Forma { get; set; }       // Лекарственная форма
        public string Producer { get; set; }    // Производитель
        public string Country { get; set; }     // Страна производителя
        public string Protocol { get; set; }    // Протокол
        public string Pokas { get; set; }       // Показания
        public int Duration { get; set; }       // Продолжительность
        public string Vid { get; set; }         // Вид 
        public string Phase { get; set; }       // Фаза
        public List<string> Bases { get; set; } // Клинические базы 
        public int Pacient { get; set; }        // Кол-во пациентов
    }
}
