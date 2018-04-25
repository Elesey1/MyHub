using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


using Xamarin.Forms;
using CurrenciesApp.Helpers;

namespace CurrenciesApp
{
	public class PrimaryPage : ContentPage
	{
        //Объявление переменных объектов, которые будут присутствовать на странице
        //Описание дано НЕ в порядке появления на странице 
        //1. Поле для ввода суммы
        Entry sumToConvert;

        //2. Кнопка для запуска процесса конвертирования
        Button getDataForConversion;

        //3-4. Выпадающие списки для выбора валют 
        //baseCurrency - список для валюты ИЗ которой призводится конверсия
        //conversionCurrency - список для валюты В какую ковертируется
        Picker baseCurrency;
        Picker conversionCurrency;

        //5. Текст для отображения результатов.
        Label conversionResult;

		public PrimaryPage ()
		{
            StackLayout mainStack = new StackLayout();
            mainStack.Spacing = 20;
            
            //6. Строка для вывода информации об отсутствии интернет-соединения.
            //Изначально пустая, но если методы в InitializeDataFromWeb не смогут
            //законнектиться к нету и загрузить данные, то они заполнят эту строку
            //и если она будет заполнена, то ее следует вывести, в ином случае - она не нужна.
            if (CurrenciesData.dataError != null)
            {
                mainStack.Children.Add(new Label { Text = CurrenciesData.dataError, TextColor = Color.Red });
            }

            //Объявление элементов интерфейса с дополнительными текстовыми метками, которые
            //поясняют пользователю за эти самые элементы
            mainStack.Children.Add(new Label { Text = "Choose a currency for conversion:" });
            mainStack.Children.Add(new Label { Text = "FROM", TextColor = Color.DimGray, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) });
            mainStack.Children.Add(baseCurrency = new Picker { ItemsSource = CurrenciesData.list });

            mainStack.Children.Add(new Label { Text = "TO", TextColor = Color.DimGray, FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)) });
            mainStack.Children.Add(conversionCurrency = new Picker { ItemsSource = CurrenciesData.list });

            mainStack.Children.Add(new Label { Text = "Write a Sum to convert:" });
            mainStack.Children.Add(sumToConvert = new Entry());

            mainStack.Children.Add(new Label { Text = "Result:", FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)), TextColor = Color.DimGray });
            mainStack.Children.Add(conversionResult = new Label { Text = "0.0" });

            mainStack.Children.Add(getDataForConversion = new Button { Text = "Convert"});
            getDataForConversion.Clicked += GetDataForConversion_Clicked;

            Content = mainStack;
        
        
		}
        //Метод, срабатывающий при нажатии на кнопку (очевидно) 
        private void GetDataForConversion_Clicked(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            //Тут производится обработка исключений (очевидно)
            //Всего производится отлов трех исключений, возникающих
            //фактически из-за ошибки ввода-вывода.


            try
            {
                //Данный метод производит всю конверсию для приложения. 
                //Он берет три аргумента и возвращает число - конвертированную сумму.
                //Аргументы в порядке очереди - сумма для конверсии, код валюты-цели для коверсии, код базовой валюты
                //
                //Вкратце: 1)сумма берется в виде текста из поля для ввода, переводится в decimal
                //
                //         2)Из выбранных в списках валютах забираются коды (Первые три буквы в скобках)
                //           В dictionary CurrenciesData.ratesToUSD эти коды играют роль ключей для коэффициентов конверсии
                //
                //         3)в теле метода вычисляется коэффициент между двумя валютами, на основе 
                //           полученных коэф-х из шага 2), умножается на сумму из шага 1) 
                //           и получившееся число возвращается
                conversionResult.Text = (Converter.GetConvertedValue(Decimal.Parse(sumToConvert.Text), conversionCurrency.SelectedItem.ToString(), baseCurrency.SelectedItem.ToString()).ToString());
            }

            //1. Если пользователь ввел в поле для суммы не цифры, то их нельзя преобразовать в decimal
            //следовательно, будет ошибка формата - в поле для decimal пойдет string.
            catch (FormatException)
            {
                sumToConvert.Text = null;
                sumToConvert.Placeholder = "Bad entry";
                sumToConvert.PlaceholderColor = Color.Red;
            }
            //2. Если пользователь в поле для суммы ничего не введет - будет нечего считать, очевидно
            //аргумент в методе Converter.GetConvertedValue в поле суммы будет null (не 0.0) следовательно, будет исключение.
            catch (ArgumentNullException)
            {
                sumToConvert.Text = null;
                sumToConvert.Placeholder = "No sum to convert";
                sumToConvert.PlaceholderColor = Color.Red;
            }
            //3. Если не будут выбрана валютная пара, то следовательно, метод будет брать строки с null
            //будет пытаться искать в них код валюты и с помощью этого кода в массиве пытаться найти коэффициэтны
            //Естественно ничего у него не получится.
            
            catch (NullReferenceException)
            {
                sumToConvert.Text = null;
                if (baseCurrency.SelectedItem == null)
                {
                    sumToConvert.Placeholder = "No base currency selected";
                }
                if (conversionCurrency.SelectedItem == null)
                {
                    sumToConvert.Placeholder = "No target currency selected";
                }
                sumToConvert.PlaceholderColor = Color.Red;
            }

        }
    }
}