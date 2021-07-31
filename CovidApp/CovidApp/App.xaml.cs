using CovidApp.Views;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

[assembly: ExportFont("Montserrat-Bold.ttf",Alias="Montserrat-Bold")]
     [assembly: ExportFont("Montserrat-Medium.ttf", Alias = "Montserrat-Medium")]
     [assembly: ExportFont("Montserrat-Regular.ttf", Alias = "Montserrat-Regular")]
     [assembly: ExportFont("Montserrat-SemiBold.ttf", Alias = "Montserrat-SemiBold")]
     [assembly: ExportFont("UIFontIcons.ttf", Alias = "FontIcons")]
namespace CovidApp {
    public partial class App : Application {

        public App() {
            InitializeComponent();
            MainPage = new LoadingPage();
            //MainPage = new NavigationPage(new TrackerPage());
        }

        protected override async void OnStart() {
            List<PolyInfo> regions = await GeoJsonHandling.InitGeoJSON();

            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

            if(status != PermissionStatus.Granted) {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            }

            MainPage = new NavigationPage(new TrackerPage(regions));


        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
