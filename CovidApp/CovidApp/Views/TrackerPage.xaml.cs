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
            //Task.Run(async () => { systemLoop(); });

        }

        HttpClient client;
        apiLocation deviceLocation;
        RootobjectGeoJson healthUnits;

        async public void InitialiseClient() {

            //will store our device location, and be updated if the device is more that ~2km away the stored location
            deviceLocation = new apiLocation();

            client = new HttpClient();
            //making the client request JSON data
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));


            string url = "https://opendata.arcgis.com/datasets/c57833f0b7fb482e91c5de7b7b283a3a_0.geojson";

            
            using (HttpResponseMessage response = await client.GetAsync(url)) {
                if (response.IsSuccessStatusCode) {

                    healthUnits = await response.Content.ReadAsAsync<RootobjectGeoJson>();
                }
                else {
                    throw new Exception("not working...");
                }
            }
            
            //Console.WriteLine(healthUnits.features[0].geometry);

        }
        string latLong = "Latitude and Longitude go here."; //useless but why not
        public string LatLong {
            get => latLong;
            set {
                if (value == latLong)
                    return;
                latLong = value;
                OnPropertyChanged(nameof(LatLong));
            }
        }
          
        public async void systemLoop() {
            bool runApi = false;
            int n = 1; // counts the the loop number (potentially useful)

            while (true) {
                var delayTask = Task.Delay(15000);
                var newLocation = await Geolocation.GetLastKnownLocationAsync();
                if (!deviceLocation.IsInitialized()) {
                    //if the location is not initialized, we will initialize
                    deviceLocation.latitude = newLocation.Latitude;
                    deviceLocation.longitude = newLocation.Longitude;
                    runApi = true;
                }
                else { 

                    var distance = deviceLocation.DistanceFrom(newLocation.Latitude, newLocation.Longitude);

                    //update the display with the new distance number 
                    LatLong = distance.ToString();

                    // if more that 2km away
                    if (distance >= 2.0) runApi = true;



                    //new code starting

                    string url = "https://opendata.arcgis.com/datasets/c57833f0b7fb482e91c5de7b7b283a3a_0.geojson";

                    using (HttpResponseMessage response = await client.GetAsync(url)) {
                        if (response.IsSuccessStatusCode) {

                            RootobjectGeoJson returnJSON = await response.Content.ReadAsAsync<RootobjectGeoJson>();

                        }
                        else {
                            throw new Exception("not working...");
                        }
                    }

                }

                if(runApi) {
                    //displayCurLoc();
                }

                //run the covid API


                //displayCurLoc(n);
                await delayTask;
                n++;
            }

        }
        

        public async void displayCurLoc() {
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
                Console.WriteLine($"not working... Message: {ex.Message}");
            }
        }
    }
}