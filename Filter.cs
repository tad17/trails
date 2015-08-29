using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trails
{
    public class Filter
    {
        public String ParsingFile = "";             // файл парсинга
        public String DoneDir = "done";             // каталог для уже сделанных файлов
        public String DoneTmpl = "*.xml";           // шаблон для выборки файлов
        public DateTime StartDate = DateTime.Now;   // с какой даты
        public DateTime EndDate = DateTime.Now;     // по какую дату
        public bool Exclude = true;                 // исключать обработанных
        public int MinPacient = 0;                  // включать только с большим кол-вом
        public string Prefix = "2015";              // префикс для уникальности
    }
}
