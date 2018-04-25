using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace CurrenciesApp
{
    public static class DataFromWeb
        //Данный класс реализует методы для закачки данных о валютах из веба
        //Метода два: для двух массивов
        //первый массив: (код валюты : полное название валюты)
        //второй массив: (код валюты : коэффициент по отношению к доллару)
        //Массивы - файлы .json.

        //Из-за бесплатной подписки на тот сайт, где предоставляют эту инфу,
        //я могу получить лишь коэффициенты по отношению к доллару. И всего-лишь 1000 раз в месяц
        //Проклятые капиталисты
    {
        //1) Этот метод возвращает список валюты для выпадающих списков в PrimaryPage
        //   Он качает .json файл со строками вида: "код валюты : полное название валюты" 
        //   и записывает в список CurrenciesData.list строки вида: "(код валюты) полное название валюты"
        public static void GetCurrenciesList()
        {
            //Переменные необходимы для загрузки данных. req - хранит данные о запросе, resp - полученный ответ
            WebRequest req;
            HttpWebResponse resp;
            //Если на устройстве нет интернета или еще какая проблема с доступом к нету... (не знаю насчет файерволла на телефона)
            try
            {
                req = WebRequest.Create("https://openexchangerates.org/api/currencies.json");
                resp = (HttpWebResponse)req.GetResponse();
            }
            //...Заполнить строку, которая возвестит пользователя об ошибке и завершить метод
            catch (WebException)
                {
                    CurrenciesData.dataError = "Could not download a currencies data: Problems with internet connection";
                    return;
                }
            //Насколько я понимаю эту магию, тут дух машины записывает ответ с сервера в виде потока необработанных байтов
            Stream respStream = resp.GetResponseStream();
            
            //Бог-машина не любит беспорядка, ибо строгие последовательности есть Путь. Надобно обтесать 
            //поток данных в string, яко же каменщик обтесывает камень...
            //Штуки в сторону: эти строки пребразуют байты в string и записывают во временнную переменную json
            StreamReader reader = new StreamReader(respStream);
            string json = reader.ReadToEnd();
            
            //После завершения записи - закрыть все наглухо
            reader.Close();
            respStream.Close();
            resp.Close();

            //Первращение полученного json в список строк для выпадающих списков в PrimaryPage
            //Так как, данные в json'e даны в виде (код валюты : полное название валюты), то преобразуем
            //данный json в dictionary<string,string>, а после - в list<string>
            CurrenciesData.list = DictionaryToList(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));
        }

        //Данный метод используется для преобразования словаря в список таким образом:
        //(код валюты : полное название валюты) -> "(код валюты) полное название валюты"
        public static List<string> DictionaryToList(Dictionary<string, string> pairs)
        {
            List<string> temp = new List<string>();
            foreach (string code in pairs.Keys)
            {
                temp.Add("(" + code + ") " + pairs[code]);
            }
            return temp;
        }

        //2) Этот метод делает все тоже самое, что и метод 1), только загружает уже json со строками вида 
        //   "(код валюты : коэффициент к доллару)" и составляет массив: Код валюты : к-т к доллару
        public static void GetCurrenciesRatiosToUSD()
        {
            //аналогично методу 1) производится загрузка..
            WebRequest req;
            HttpWebResponse resp;
            
            //..и отлов исключения
            try
            {
                req = WebRequest.Create("https://openexchangerates.org/api/latest.json?app_id=243cf31e08dd455ca9b7b24764cdabbe");
                resp = (HttpWebResponse)req.GetResponse();
            }
            catch (WebException)
            {
                CurrenciesData.dataError = "Could not download a Currency Data: Problems with internet connection";
                return;
            }

            //Аналогично поток преобразуется в json..
            Stream respStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(respStream);
            string json = reader.ReadToEnd();

            //...Ну не совсем аналогично. Создается уже не строка, а объект JObject из библиотек Newtonsoft
            //закачанный файл json имеет помимо информации о к-тах, еще несколько не используемых полей
            //типа валюты по отношению к которой все эти коэффициенты и приводятся, времени закачки, и еще
            //бог-машина знает чего. Преобразуй это в строку - было-бы проблематично отсечь всю эту информацию
            //для записи остального в массив как это было сделано для списка. 
            //Поэтому, я воспользовался Newtonsoft'ом для создания json объекта, поскольку для него
            //существуют методы изъятия отдельных разделов этого самого объекта
            JObject currenciesRates = JObject.Parse(json);

            //Аналогично все закрывается..
            reader.Close();
            respStream.Close();
            resp.Close();

            //Собственно здесь уже производится запись в массив объекта созданного выше. 
            //currenciesRates["rates"] - обращение к разделу rates в json-объекте 
            //.ToObject<Dictionary<string, decimal>>() - преобразует этот раздел в массив. Просто и легко
            CurrenciesData.ratesToUSD = currenciesRates["rates"].ToObject<Dictionary<string, decimal>>();

        }

    }
    //Данный класс был создан для хранения загруженных данных
    //Наличие в нем переменных с названиями типа list было сделано для
    //скоращения записи, т.к. к данным переменным будут обращаться
    //как CurrenciesData.list - к.м.к. довольно информативно
    public static class CurrenciesData
    {
        static public List<string> list = new List<string>();
        static public Dictionary<string, decimal> ratesToUSD;
        static public string dataError = null;
    }

}
