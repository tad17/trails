using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trails
{
    public class PokasBase
    {
        public List<string> Raw { get; set; } // необработанный список строк (файл)
        public List<string> Pokas { get; set; }
        public List<string> BaseRaw { get; set; }
        public Dictionary<string, List<string>> pokas;

        public PokasBase()
        {
            Raw = new List<string>();
            Pokas = new List<string>();
            BaseRaw = new List<string>();
            pokas = new Dictionary<string, List<string>>();
        }

        public void Prepare()
        {
            // подготавливает справочник к работе
            foreach (string line in Raw)
            {
                prepareLine(line);
            }

        }
        private void prepareLine(string line)
        {
            line = line.Trim();
            // парсинг одной строки справочника
            if (line.Contains(':'))
            {
                // есть соответствие, разберем
                string[] s = line.Split(':');
                Pokas.Add(s[0].Trim());
                BaseRaw.Add(s[1].Trim()); // список вариантов через запятую
                pokas[s[0].Trim()] = new List<string>(s[1].Split(','));
            }
            else
            {
                // соответствия нет, поэтому просто берем название показания
                Pokas.Add(line);
                BaseRaw.Add(line);

                List<string> dd = new List<string>();
                dd.Add(line);
                pokas[line] = dd;
            }
        }

        public string CheckBase(string bb)
        {
            bb = bb.ToLower(); // сравниваем в одном регистре

            // проверим
            /*
            for (int n = 0; n < BaseRaw.Count; n++)
            {
                string rb = BaseRaw[n];
                string g = Pokas[n];
                string[] variants = rb.Split(',');
                foreach (string v in variants)
                {
                    if (bb.Contains(v.Trim()))
                    {
                        return g;
                    }
                }
            }
            */

            string answer = checkInDictonary(bb); 
            return  answer;
        }

        private string checkInDictonary(string pokasanie)
        {
            foreach (string key in pokas.Keys)
            {
                List<string> values = pokas[key];
                foreach (string val in values)
                {
                    string match = val.Trim().ToLower();
                    if (pokasanie.Contains(match))
                    {
                        return key;
                    }
                }
            }
            return "";
        }

    }
}
