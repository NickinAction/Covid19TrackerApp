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
using Newtonsoft.Json.Linq;

namespace CovidApp.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackerPage : ContentPage {
        public TrackerPage(List<PolyInfo> regions) {
            InitializeComponent();
            BindingContext = this;
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);
            string yesterdayDate = yesterday.ToString("dd-MM-yyyy");
            string vaccineYesterday = yesterday.ToString("yyyy-MM-dd");
            Task.Run(async () => { await systemLoop(regions, yesterdayDate, vaccineYesterday); });
        }

        List<PolyInfo> regions = new List<PolyInfo>();
        //needed values for API requests
        HttpClient client = new HttpClient();

        string apiLink = "https://api.opencovid.ca/summary?stat=cases&loc=";
        string vaccineLink = "https://api.covid19tracker.ca/reports/regions/";
        string provVaccineLink = "https://api.covid19tracker.ca/reports/province/";
        string vaccineLink1 = "?date=";
        string apiLink1 = "&date=";

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

        public async Task systemLoop(List<PolyInfo> regions, string yesterdayDate, string vaccineYesterday) {

            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

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

                int id = -1;

                foreach (var poly in regions) {
                    if (poly.geom.Contains(curPoint)) {
                        polyName = $"{poly.engName} has ID: {poly.hRID}";
                        id = poly.hRID;
                        break;
                    }
                }

                //API requests 
                string localPHU = await client.GetStringAsync($"{apiLink}{id}{apiLink1}{yesterdayDate}");
                string testString1 = $"{vaccineLink}{id}{vaccineLink1}{yesterdayDate}";
                string localVaccines = await client.GetStringAsync($"{vaccineLink}{id}{vaccineLink1}{vaccineYesterday}");
                string testString2 = $"{vaccineLink}{id}{vaccineLink1}{vaccineYesterday}";
                //string provinceWide = await client.GetStringAsync(apiLink + provLocation + apiLink1 + yesterdayDate);
                //string provinceVaccines = await client.GetStringAsync(provVaccineLink + provLocation + vaccineLink1 + vaccineYesterday);
                string canadaWide = await client.GetStringAsync("https://api.opencovid.ca/summary?stat=cases&loc=canada&date=" + yesterdayDate);
                string testString3 = $"https://api.opencovid.ca/summary?stat=cases&loc=canada&date={yesterdayDate}";
                string canadaVaccines = await client.GetStringAsync("https://api.covid19tracker.ca/reports?date=" + vaccineYesterday);
                string testString4 = $"https://api.covid19tracker.ca/reports?date={vaccineYesterday}";

                //Creation of JSON objects for data retrieved from APIs
                JObject localStats = JObject.Parse(localPHU);
                JObject localVaccine = JObject.Parse(localVaccines);
                //JObject provinceStats = JObject.Parse(provinceWide);
                //JObject vaccinesProv = JObject.Parse(provinceVaccines);
                JObject canadaStats = JObject.Parse(canadaWide);
                JObject vaccineCanada = JObject.Parse(canadaVaccines);

               /*
                *  Local Data 
               */

                //assign variables for data from local PHU
                var localDailyCases = localStats["summary"][0]["cases"];
                var localTotalCases = localStats["summary"][0]["cumulative_cases"];
                var localDeaths = localStats["summary"][0]["deaths"];
                var localTotalDeaths = localStats["summary"][0]["cumulative_deaths"];

                //assign variables for local vaccine data
                var localDailyVaccines = localVaccine["data"][0]["change_vaccinations"];
                var localTotalVaccinated = localVaccine["data"][0]["total_vaccinated"];
                var localVaccinesAdmin = localVaccine["data"][0]["total_vaccinations"];

                /*
                *  Provincial Data 
                *

                //assign variables for provincial data
                var provinceDailyCases = provinceStats["summary"][0]["cases"];
                var provinceTotalCases = provinceStats["summary"][0]["cumulative_cases"];
                var provinceDeaths = provinceStats["summary"][0]["deaths"];
                var provinceTotalDeaths = provinceStats["summary"][0]["cumulative_deaths"];

                //assign variables for provincial vaccine data
                var provinceDailyVaccines = vaccinesProv["data"][0]["change_vaccinations"];
                var provinceTotalVaccinated = vaccinesProv["data"][0]["total_vaccinated"];
                var provinceVaccinesAdmin = vaccinesProv["data"][0]["total_vaccinations"];
                */

                /*
                 * National Data
                 */

                //assign variables for national data
                var canadaCases = canadaStats["summary"][0]["cases"];
                var canadaTotalCases = canadaStats["summary"][0]["cumulative_cases"];
                var canadaDeaths = canadaStats["summary"][0]["deaths"];
                var canadaTotalDeaths = canadaStats["summary"][0]["cumulative_deaths"];

                //assign variables for national vaccine data
                var canadaDailyVaccines = vaccineCanada["data"][0]["change_vaccinations"];
                var canadaTotalVaccinated = vaccineCanada["data"][0]["total_vaccinated"];
                var canadaVaccinesAdmin = vaccineCanada["data"][0]["total_vaccinations"];

                tracker++;
                MainThread.BeginInvokeOnMainThread(() => {
                    DataLabel.Text = polyName;
                    CasesCount.Text = $"{localDailyCases}";
                });
                // wait 15 seconds
                await delayTask;
            }

        }
    }
}