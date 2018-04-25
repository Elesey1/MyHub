using System;
using System.Collections.Generic;
using System.Text;

namespace CurrenciesApp.Helpers
{

        public static class CurrenciesData
        {
            static public List<string> list = new List<string>();
            static public Dictionary<string, decimal> ratesToUSD;
            static public string dataError = null;
        }

}
