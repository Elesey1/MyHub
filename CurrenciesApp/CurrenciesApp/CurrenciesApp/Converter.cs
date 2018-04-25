using CurrenciesApp.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace CurrenciesApp
{
    //Данный класс реализует единственный метод для конверсии суммы из валюты в валюту.
    public static class Converter
    {
        //В PrimaryPage был вкратце рассмотрен этот метод, тут рассмотрим подробнее:
        //Метод берет в качестве аргументов: 1)Сумма для конверсии insertedValue
        //                                   2)Строка для поиска кода целевой валюты convertedCode
        //                                   3)Строка для поиска кода изначальной валюты baseCode
        //
        //Метод возвращает конвертированную сумму
        public static decimal GetConvertedValue(decimal insertedValue, string convertedCode, string baseCode)
        {
            //Здесь производится запись к-тов для конверсии из массива CurrenciesData.ratesToUSD
            //это массив вида (ключ : значение) в качестве ключа выступает код валюты
            decimal convertedValue = CurrenciesData.ratesToUSD[convertedCode.Substring(1,3)];
            decimal baseValue = CurrenciesData.ratesToUSD[baseCode.Substring(1,3)];

            //convertedCode.Substring(1,3) - подразумевается, что этот метод берет код валюты 
            //из выбранной строки в выпадающем списке на PrimaryPage. В свою очередь в том выпадающем списке
            //используется список CurrenciesData.list, в котором строки вида "(код валюты) название валюты"
            //Соответственно, раз требуется код валюты (а этот код всегда предствляет собой три заглавные буквы)
            //то мы просто возьмем и создадим подстроку, начиная со второй позиции (за скобкой)
            //и длинной ровно из трех символов
            //Если из, к примеру, из строки "(USD) United States Dollar" тупо возьмем три символа,
            //начиная со второй позиции то, получим строку "USD", которая будет ключем для коэффициента
            //конверсии, в данном примере: 1.0

            return insertedValue *(convertedValue/baseValue);
            //Тупо перемножение на суммы на коээфициент. Так как коэффициенты даны по отношению к доллару
            //(т.е. к-т = доллар/валюту) то следует брать обратные к-ты (1/(доллар/валюта)) 
            //рассчитывать отношение (валюта1/валюта2) как (1/к-т валюты1 к доллару)/(1/к-т валюты2 к доллару)
            //И получается (валюта2/валюта1), т.е. convertedValue/baseValue
        }
    }
}
