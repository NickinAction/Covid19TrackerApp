using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using CovidApp.Views;
using NetTopologySuite.Operation.Overlay.Validate;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Page = Xamarin.Forms.Page;

namespace CovidApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            On<iOS>().SetModalPresentationStyle(UIModalPresentationStyle.FormSheet);
            InitializeComponent();

            BindingContext = this;   
        }
        
        public string StatisticsTypeText
        {
            get
            {
                string returnValue = StatsValue ? "National" : "Provincial";
                return $"Stats: {returnValue}";
            }
        }

        //public bool statsValue = Preferences.Get("Stats", "National") == "National";

        public bool StatsValue
        {
            get
            {
                return Preferences.Get("Stats", "National") == "National";
            }
            set
            {
                if (value)
                {
                    Preferences.Set("Stats", "National");

                }
                else
                {
                    Preferences.Set("Stats", "Provincial");

                }
                OnPropertyChanged(nameof(StatsValue));

                OnPropertyChanged(nameof(StatisticsTypeText));
            }
        }

        
        public async void BackClicked(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync(true);
            
        }

        protected override async void OnDisappearing()
        {
            base.OnDisappearing();

        }
    }
}