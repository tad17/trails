using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trails
{
    public class GorodBase
    {
        public List<string> Raw { get; set; } // необработанный список строк (файл)
        public List<string> Gorod { get; set; }
        public List<string> BaseRaw { get; set; }
        public Dictionary<string, List<string>> cities;

        public GorodBase()
        {
            Raw = new List<string>();
            Gorod = new List<string>();
            BaseRaw = new List<string>();
            cities = new Dictionary<string, List<string>>();
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

            if (line == "")
            {
                return;
            }

            // парсинг одной строки справочника
            string[] s;
            if (line.Contains(':'))
            {
                s = line.Split(':');
                // есть соответствие, разберем
                Gorod.Add(s[0].Trim());
                BaseRaw.Add(s[1].Trim()); // список вариантов через запятую

                cities[s[0].Trim()] = new List<string>(s[1].Split(','));
            }
            else
            {
                // соответствия нет, поэтому просто берем название города
                Gorod.Add(line);
                BaseRaw.Add(line);

                List<string> dd = new List<string>();
                dd.Add(line);
                cities[line] = dd;
            }
        }

        public string CheckBase(string bb)
        {

            bb = bb.ToLower(); // сравниваем в одном регистре
            /*
            // проверим наличие в справочнике
            for (int n = 0; n < BaseRaw.Count; n++)
            {
                string g = Gorod[n].Trim();

                string rb = BaseRaw[n]; // различные варианты написаний
                string[] variants = rb.Split(',');
                foreach (string v in variants)
                {
                    string match = v.Trim().ToLower(); // сравниваем в одном регистре
                    if (bb.Contains(match))
                    {
                        return g;
                    }
                }
            }
            */
            string answer = checkInDictonary(bb);
            return answer;
        }

        private string checkInDictonary(string gorod)
        {
            foreach (string key in cities.Keys)
            {
                List<string> values = cities[key];
                foreach (string val in values)
                {
                    string match = val.Trim().ToLower();
                    if (gorod.Contains(match)) {
                        return key;
                    }
                }
            }
            return "";
        }
    }
}
