using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace trails
{
    public class CountrySponsor
    {
        public List<string> Raw { get; set; } // необработанный список строк (файл)
        public Dictionary<string, List<string>> country;

        public CountrySponsor()
        {
            Raw = new List<string>();
            country = new Dictionary<string, List<string>>();
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
                List<string> dd = new List<string>(s[1].Split(','));
                for (int i = 0; i < dd.Count; i++)
                {
                    dd[i] = dd[i].Trim();
                }
                country[s[0].Trim()] = dd;
            }
            else
            {
                // соответствия нет, поэтому просто берем название страны
                List<string> dd = new List<string>();
                dd.Add(line);
                country[line] = dd;
            }
        }

        public string CheckCountry(string bb)
        {
            bb = bb.ToLower(); // сравниваем в одном регистре
            string answer = checkInDictonary(bb);
            return answer;
        }

        private string checkInDictonary(string sponsor)
        {
            foreach (string key in country.Keys)
            {
                List<string> values = country[key];
                foreach (string val in values)
                {
                    string match = val.Trim().ToLower();
                    if (sponsor.Contains(match))
                    {
                        return key;
                    }
                }
            }
            return "";
        }
    }
}
