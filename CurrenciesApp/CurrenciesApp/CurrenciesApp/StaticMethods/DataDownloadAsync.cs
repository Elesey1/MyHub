using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace CurrenciesApp.StaticMethods
{
    class DataDownloadAsync
    {
        //Here we have methods dedicated to download a data from web asynchronically
        public async static Task<Dictionary<string, decimal>> GetCurrenciesRatiosToUSD()
        {

            //Here we have method to download a currencies rates dictionary
            //Dictionary will have itmes like "Currency Code" : RateToUsd
            //But now we're creating client, giving adress to it, and waiting for response
            WebRequest req;
            WebResponse resp;
            req = WebRequest.Create("https://openexchangerates.org/api/latest.json?app_id=243cf31e08dd455ca9b7b24764cdabbe");
            resp = await req.GetResponseAsync();

            //After we'vw got a response we proceed it to string json (Originally file on serever has .json extension)
            Stream respStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(respStream);
            string json = reader.ReadToEnd();

            //Here we use Newtonian.Json class JObject to create actual json object
            JObject currenciesRates = JObject.Parse(json);

            //Now we're closing all data-streams
            reader.Close();
            respStream.Close();
            resp.Close();

            //And then we return a dictionary here. JObject has a method to do so directly
            //including only parts we want. In original downloaded file there were other not-so-useful sections
            //like "baseCurrency" or "timeStamp", so methods of JObject class provides us easy solution to cut them off
            return currenciesRates["rates"].ToObject<Dictionary<string, decimal>>();

        }

        public async static Task<List<string>> GetCurrenciesList()
        {
            //Here we go again. But for currencies list
            WebRequest req;
            WebResponse resp;
            req = WebRequest.Create("https://openexchangerates.org/api/currencies.json");
            resp = await req.GetResponseAsync();
            Stream respStream = resp.GetResponseStream();
            StreamReader reader = new StreamReader(respStream);
            string json = reader.ReadToEnd();

            reader.Close();
            respStream.Close();
            resp.Close();

            //Here we have no need in creating separate JObject, since the only section in that file is "rates"
            //So basically, since we have ditionary with content like "Code" : "Name", we need to make a list
            // of it, so it could be utilised by picker in page. This static method is desribed in StaticMethods.DictionaryToList
            return DictionaryToList.MakeItSo(JsonConvert.DeserializeObject<Dictionary<string, string>>(json));

        }

    }
}



