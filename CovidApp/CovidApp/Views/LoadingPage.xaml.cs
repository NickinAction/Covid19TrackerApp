using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CovidApp.Views;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace CovidApp.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoadingPage : ContentPage {
        public LoadingPage() {
            InitializeComponent();
        }
    }
}