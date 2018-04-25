using CurrenciesApp.StaticMethods;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace CurrenciesApp.ViewModel
{
    class PrimaryPageViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        ////It's a view-model. It contains three partts:
        //1)Backup variables and auto-generated properties
        //2)Propeties that fires an event in setter
        //3)A few handy methods
        //Here we have a backup variables for propertis down below. 
        private List<string> currenciesList;
        private static string statusString = "";
        private Dictionary<string, decimal> currenciesRates;
        private string conversionResult;
        private Color statusColor;


        //Here the auto-generated properties
        public string Entry { set; get; }
        public string FromSelected { set; get; }
        public string ToSelected { set; get; }
        public ICommand Convert { set; get; }


        //Here we have properties described in 2)
        public Color StatusColor
        {
            get { return statusColor; }
            set
            {
                if (statusColor != value)
                {
                    statusColor = value;
                    OnPropertyChanged("StatusColor");
                }
            }
        }

        public string ConversionResult
        {
            get { return conversionResult; }
            set
            {
                if (conversionResult != value)
                {
                    conversionResult = value;
                    OnPropertyChanged("ConversionResult");
                }
            }
        }



        public List<string> CurrenciesList
        {
            get { return currenciesList; }

            set
            {
                if (currenciesList != value)
                {
                    currenciesList = value;
                    OnPropertyChanged("CurrenciesList");

                }
            }
        }
        //...Lots of them
        public Dictionary<string,decimal> CurrenciesRates
        {
            get { return currenciesRates; }

            set
            {
                if (currenciesRates != value)
                {
                    currenciesRates = value;
                    OnPropertyChanged("CurrenciesRates");
                }
            }
        }

        public string StatusString

        {
            get { return statusString;  }
            set
            {
                if (statusString != value)
                {
                    statusString = value;
                    OnPropertyChanged("StatusString");
                }
            }
        }


        //And here goes the third part: - methods, including class constructor
        public PrimaryPageViewModel()
        {
            //When view-model initializes, first it tries to download data from intenet via method "DownloadData"
            //that will be described a bit further, but before that, it informs user, that it's actually trying to
            //download data.
            StatusString = "Trying to download currencies data";
            StatusColor = Color.Orange;
            DownloadData();
            //Now it initialises command for a button in View
            Convert = new Command(GetConvertedValue);
        }


        //Here we have the DownloadDtata Method
        public async void DownloadData()
        {
            //Basically it calls an async dowload method for CurrenciesList and Currencies rates
            //While doing so, it also informing use about download process and possible errors during this
            //Both methods described in StaticMethods.DataDownloadAsync
            //Here we downloading currencies List that will be used as source for picker in PrimaryPage
            try
            {
                //Downloading
                CurrenciesList = await DataDownloadAsync.GetCurrenciesList();
            }
            catch (WebException)
            {
                //If something goes wrong - cut down the process and inform user
                StatusString = "Cannot download data";
                StatusColor = Color.Red;
                return;
            }
            
            //If list has downloaded, then tell user about that
            StatusString = "Data paritially Downloaded";
            StatusColor = Color.YellowGreen;

            try
            {
                //Here we go again, but with rates
                CurrenciesRates = await DataDownloadAsync.GetCurrenciesRatiosToUSD();
            }
            catch (WebException)
            {
                //And again handling possible errors
                StatusString = "Cannot download data";
                StatusColor = Color.Red;
                return;
            }

            //Finally, if everything is working, then inform user 
            StatusString = "All data downloaded";
            StatusColor = Color.ForestGreen; 

        }


        //Here goes a methods that actually converts user sum from one currency to other
        public void GetConvertedValue()
        {
            //As I cannot describe variables in try section, I do it here. But if those variables
            //are not initialised with value, visual studio will make a warning, so I've set them to zero
            decimal convertedValue=0;
            decimal baseValue = 0;
            try
            {
                //Trying to get data from currencies list. List itself contains strings like
                // (Currency code) Currency full name. Code is described by three chars, so we
                //take substring eith length of three excluding the first symbol - "(".
                //As we've gotten code, we seek through the Dictionary CurrenciesRates with it
                //To find conversion rate of given currency.
                //
                //Rigth here we trying to get rate of target currency
                convertedValue = CurrenciesRates[ToSelected.Substring(1, 3)];
            }
            catch (NullReferenceException)
            {
                //Here we trying to catch a possible exception, while informig user of course
                StatusString = "Conversion currency not chosen";
                StatusColor = Color.Red;
                return;
            }

            try
            {
                //Same as before - getting rate of given currency, but we're looking for base one.
                baseValue = CurrenciesRates[FromSelected.Substring(1, 3)];
            }
            catch (NullReferenceException)
            {
                //Catchin exception
                StatusString = "Base currency not chosen";
                StatusColor = Color.Red;
                return;

            }

            try
            {
                //Here the Machine Spirit does all the math. As I only have rates of all currencies to dollar (dollar/curr), then
                //to get conversio rate between two random currencies would be (1/(dollar/baseCurr))/(1/(dollar/targetCurr)) so
                //it goes simply as (targetCurr/baseCurr).
                //Here we get value in (Decimal.Parse(Entry) * (convertedValue / baseValue))
                //After that we round-up that value down to two digits after decimal separator with Math.Round(value, 2)
                //And then, we convert result to string with .ToString()
                ConversionResult = Math.Round((Decimal.Parse(Entry) * (convertedValue / baseValue)), 2).ToString();
            }

            catch (Exception)
            {
                //In case of errors - use this!
                StatusString = "Bad Entry";
                StatusColor = Color.Red;
                return;

            }
            //Inform use about conversion for some reason
            StatusString = "Conversion Done";
            StatusColor = Color.ForestGreen;
        }


        //Here we fire an event stating that value of given property has changed for page to update page's content
        private void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }




}
