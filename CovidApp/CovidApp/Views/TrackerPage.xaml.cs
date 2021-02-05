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

namespace CovidApp.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackerPage : ContentPage {
        public TrackerPage() {
            InitializeComponent();
            BindingContext = this;
            InitialiseClient();
            Task.Run(async () => { systemLoop(); });

        }

        HttpClient client;

        public void InitialiseClient () {

            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async void systemLoop() {
            int n = 1;

            while (true) {
                var delayTask = Task.Delay(15000);
                displayCurLoc(n);
                await delayTask;
                n++;
            }

        }
        
        string latLong = "Latitude and Longitude go here.";
        public string LatLong {
            get => latLong;
            set {
                if (value == latLong)
                    return;
                latLong = value;
                OnPropertyChanged(nameof(LatLong));
            }
        }



        public async void displayCurLoc(int val) {
            try {
                var location = await Geolocation.GetLastKnownLocationAsync();

                if (location != null) {
                    /*
                    Console.WriteLine($"{val}, Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");
                    LatLong = $"{val}, Latitude: {location.Latitude}, Longitude: {location.Longitude}";
                    Console.WriteLine("Finished doing async task sucessfully.");
                    */

                    string apiKey = "A0dRkTpbURILssAZWP5NFdQ3TLAfBlxN";

                    string url = "https://www.mapquestapi.com/geocoding/v1/reverse?key="
                        + apiKey + "&location="
                        + location.Latitude + ","
                        + location.Longitude;

                    using (HttpResponseMessage response = await client.GetAsync(url)) {
                        if(response.IsSuccessStatusCode) {
                            Rootobject displayLocation = await response.Content.ReadAsAsync<Rootobject>();

                            LatLong = displayLocation.results[0].locations[0].adminArea3;
                      
                        }
                        else {
                            throw new Exception("not working...");
                        }
                    }

                }
            }
            catch (FeatureNotSupportedException fnsEx) {
                // Handle not supported on device exception
            }
            catch (FeatureNotEnabledException fneEx) {
                // Handle not enabled on device exception
            }
            catch (PermissionException pEx) {
                // Handle permission exception
            }
            catch (Exception ex) {
                // Unable to get location
                Console.WriteLine(($"not working... Message: {ex.Message}"));
            }
        }
    }
}