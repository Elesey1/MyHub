using CurrenciesApp.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CurrenciesApp.View
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class PrimaryPage : ContentPage
	{
		public PrimaryPage ()
		{
            InitializeComponent ();
            BindingContext = new PrimaryPageViewModel();
		}
	}
}