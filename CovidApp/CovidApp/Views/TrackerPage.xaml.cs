using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http.Headers;
using System;
using System.IO;
using NetTopologySuite.Geometries;
using GeoJSON.Net;
using System.Reflection;

namespace CovidApp.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackerPage : ContentPage {
        public TrackerPage(List<PolyInfo> regions) {
            InitializeComponent();

            BindingContext = this;

            Task.Run(async () => { await systemLoop(regions); });
        }

        List<PolyInfo> regions = new List<PolyInfo>();

        public string latLong;
        public string LatLong {
            get => latLong;
            set {
                if (value == latLong)
                    return;
                latLong = value;
                OnPropertyChanged(nameof(LatLong));
            }
        }

        public string polyName = "default";

        public async Task systemLoop(List<PolyInfo> regions) {

            int tracker = 0;
            while (true) {

                var delayTask = Task.Delay(15000); // 15 seconds
                // request location
                var request = new GeolocationRequest(GeolocationAccuracy.Medium, TimeSpan.FromSeconds(10));
                CancellationTokenSource source = new CancellationTokenSource();
                CancellationToken cancelToken = source.Token;
                var loc = await Geolocation.GetLocationAsync(request, cancelToken);

                // create a point from the lat and lon retrieved
                var curPoint = new NetTopologySuite.Geometries.Point(loc.Latitude, loc.Longitude);


                foreach (var poly in regions) {
                    if (poly.geom.Contains(curPoint)) {
                        polyName = $"{poly.engName} has ID: {poly.hRID}";

                        break;
                    }
                }
                tracker++;
                MainThread.BeginInvokeOnMainThread(() => {
                    DataLabel.Text = polyName;
                    CasesCount.Text = $"{tracker}";
                });
                // wait 15 seconds
                await delayTask;
            }

        }
    }
}