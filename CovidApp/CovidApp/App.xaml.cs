using CovidApp.Views;
using System;
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

            //Task.Run(async () => { await permissionRequest(); }).Wait();

            var status = await RequestAsync_Fixed<Permissions.LocationWhenInUse>();

            MainPage = new NavigationPage(new TrackerPage(regions));

        }

        public async Task<PermissionStatus> RequestAsync_Fixed<TPermission>()
        where TPermission : Permissions.BasePermission, new() {
            // temporary fix for https://github.com/xamarin/Essentials/issues/1390
            if (Xamarin.Forms.Device.RuntimePlatform == Xamarin.Forms.Device.iOS &&
                DeviceInfo.Version.Major >= 14) {
                Permissions.RequestAsync<TPermission>(); // don't await as it won't return on iOS 14 until https://github.com/xamarin/Essentials/issues/1390 is fixed
                PermissionStatus status;
                while ((status = await Permissions.CheckStatusAsync<TPermission>()) == PermissionStatus.Unknown)
                    await Task.Delay(50);
                return status;
            }

            return await Permissions.RequestAsync<TPermission>();
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
