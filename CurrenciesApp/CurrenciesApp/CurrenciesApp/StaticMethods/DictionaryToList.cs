using System;
using System.Collections.Generic;
using System.Text;


namespace CurrenciesApp.StaticMethods
{
    class DictionaryToList
    {
        //And here we are. Just simple code to make "Code" : "Currency" dictionary into
        // "(Code) Currency" list of strings
        public static List<string> MakeItSo(Dictionary<string, string> pairs)
        {
            List<string> temp = new List<string>();
            foreach (string code in pairs.Keys)
            {
                temp.Add("(" + code + ") " + pairs[code]);
            }
            return temp;
        }
    }
}
