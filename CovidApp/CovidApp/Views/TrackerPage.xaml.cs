using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Net.Http;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using Esri.ArcGISRuntime;
using Newtonsoft.Json.Linq;

namespace CovidApp.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TrackerPage : ContentPage {
        public TrackerPage(List<PolyInfo> regions) {
            InitializeComponent();
            BindingContext = this;
            NavigationPage.SetHasNavigationBar(this, false);
            Task.Run(async () => { await systemLoop(regions); });
        }

        List<PolyInfo> regions = new List<PolyInfo>();
        HttpClient client = new HttpClient();

        #region PHU INFO

        public string activeCasesPHUNumber;

        public string newCasesPHUNumber;

        public string totalPHUDeathsNumber;

        public string newPHUDeathsNumber;

        public string totalPHUVaccinatedNumber;

        public string newPHUVaccinatedNumber;

        public string activeCasesPHU
        {
            get
            {
                return activeCasesPHUNumber;
            }
            set
            {
                if (activeCasesPHUNumber == value)
                {
                    return;
                }

                activeCasesPHUNumber = value;
                OnPropertyChanged(nameof(activeCasesPHU));
            }
        }

        public string newCasesPHU
        {
            get
            {
                return newCasesPHUNumber;
            }
            set
            {
                if (newCasesPHUNumber == value)
                {
                    return;
                }

                newCasesPHUNumber = value;
                OnPropertyChanged(nameof(newCasesPHU));
            }
        }

        public string totalPHUDeaths
        {
            get
            {
                return totalPHUDeathsNumber;
            }
            set
            {
                if (totalPHUDeathsNumber == value)
                {
                    return;
                }

                totalPHUDeathsNumber = value;
                OnPropertyChanged(nameof(totalPHUDeaths));
            }
        }

        public string newPHUDeaths
        {
            get
            {
                return newPHUDeathsNumber;
            }
            set
            {
                if (newPHUDeathsNumber == value)
                {
                    return;
                }

                newPHUDeathsNumber = value;
                OnPropertyChanged(nameof(newPHUDeaths));
            }
        }

        public string totalPHUVaccinated
        {
            get
            {
                return totalPHUVaccinatedNumber;
            }
            set
            {
                if (totalPHUVaccinatedNumber == value)
                {
                    return;
                }

                totalPHUVaccinatedNumber = value;
                OnPropertyChanged(nameof(totalPHUVaccinated));
            }

        }

        public string newPHUVaccinated
        {
            get
            {
                return newPHUVaccinatedNumber;
            }
            set
            {
                if (newPHUVaccinatedNumber == value)
                {
                    return;
                }

                newPHUVaccinatedNumber = value;
                OnPropertyChanged(nameof(newPHUVaccinated));
            }
        }


        #endregion

        #region NATIONAL/PROVINCIAL INFO

        //CASES - NATIONAL

        public string activeCasesNationalNumber;

        public string newCasesNationalNumber;

        //DEATHS - NATIONAL

        public string totalDeathsNationalNumber;

        public string newDeathsNationalNumber;

        //VACCINES - NATIONAL

        public string totalVaccinatedNationalNumber;

        public string newVaccinatedNationalNumber;

        //CASES - PROVINCIAL

        public string activeCasesProvincialNumber;

        public string newCasesProvincialNumber;

        //DEATHS - PROVINCIAL

        public string totalDeathsProvincialNumber;

        public string newDeathsProvincialNumber;

        //VACCINES - PROVINCIAL

        public string totalVaccinatedProvincialNumber;

        public string newVaccinatedProvincialNumber;




        public string ActiveCasesRegionalNumber
        {
            get
            {
                if(Preferences.Get("Stats", "National") == "National")
                {
                    return activeCasesNationalNumber;
                }
                else
                {
                    return activeCasesProvincialNumber;
                }
            }
            set
            {
            }
        }
        public string NewCasesRegionalNumber
        {
            get
            {

                if (Preferences.Get("Stats", "National") == "National")
                {
                    return newCasesNationalNumber;
                }
                else
                {
                    return newCasesProvincialNumber;
                }
            }
            set
            {
                
            }
        }
        

        public string TotalDeathsRegionalNumber
        {
            get
            {
                if (Preferences.Get("Stats", "National") == "National")
                {
                    return totalDeathsNationalNumber;
                }
                else
                {
                    return totalDeathsProvincialNumber;
                }
            }
            set
            {
               
            }
        }
        public string NewDeathsRegionalNumber
        {
            get
            {
                if (Preferences.Get("Stats", "National") == "National")
                {
                    return newDeathsNationalNumber;
                }
                else
                {
                    return newDeathsProvincialNumber;
                }
            }
            set
            {
            }
        }

        public string TotalVaccinatedRegionalNumber
        {
            get
            {
                if (Preferences.Get("Stats", "National") == "National")
                {
                    return totalVaccinatedNationalNumber;
                }
                else
                {
                    return totalVaccinatedProvincialNumber;
                }
            }
            set
            {
            }
        }
        public string NewVaccinatedRegionalNumber
        {
            get
            {
                if (Preferences.Get("Stats", "National") == "National")
                {
                    return newVaccinatedNationalNumber;
                }
                else
                {
                    return newVaccinatedProvincialNumber;
                }
            }
            set
            {
            }
        }


        public static string generalStatisticsType;

        public string GeneralStatisticsType
        {
            get
            {
                return generalStatisticsType;
            }
            set
            {
                if (value == generalStatisticsType)
                {
                    return;
                }

                generalStatisticsType = value;

                OnPropertyChanged(nameof(GeneralStatisticsType));

                OnPropertyChanged(nameof(TotalDeathsRegionalNumber));
                OnPropertyChanged(nameof(NewDeathsRegionalNumber));

                OnPropertyChanged(nameof(ActiveCasesRegionalNumber));
                OnPropertyChanged(nameof(NewCasesRegionalNumber));

                OnPropertyChanged(nameof(TotalVaccinatedRegionalNumber));
                OnPropertyChanged(nameof(NewVaccinatedRegionalNumber));
            }
        }

        #endregion
        public async Task systemLoop(List<PolyInfo> regions)
        {
            DateTime today = DateTime.Today;
            DateTime yesterday = today.AddDays(-1);
            string yesterdayDate = yesterday.ToString("dd-MM-yyyy");
            string vaccineYesterday = yesterday.ToString("yyyy-MM-dd");

            string apiLink = "https://api.opencovid.ca/summary?stat=cases&loc=";
            string vaccineLink = "https://api.covid19tracker.ca/reports/regions/";
            string provVaccineLink = "https://api.covid19tracker.ca/reports/province/";
            string vaccineLink1 = "?date=";
            string apiLink1 = "&date=";
            string provLocation = "ON"; //default set to ON

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
                        id = poly.hRID;
                        break;
                    }
                }



                /*
                 * Somewhat fragmented in order to implement provincial location
                 */

                //API requests 
                string localPHU = await client.GetStringAsync($"{apiLink}{id}{apiLink1}{yesterdayDate}");
                string localVaccines = await client.GetStringAsync($"{vaccineLink}{id}{vaccineLink1}{vaccineYesterday}");

                string betterLocalPHU = await client.GetStringAsync($"{vaccineLink}{id}{vaccineLink1}{vaccineYesterday}");


                //Creation of JSON objects for data retrieved from APIs

                JObject localStats = JObject.Parse(localPHU);
                JObject betterLocalStats = JObject.Parse(betterLocalPHU);


                //find provincial code
                switch (localStats["summary"][0]["province"].ToString())
                {
                    case "Ontario":
                        provLocation = "ON";
                        break;
                    case "Quebec":
                        provLocation = "QC";
                        break;
                    case "Alberta":
                        provLocation = "AB";
                        break;
                    case "BC":
                        provLocation = "BC";
                        break;
                    case "Manitoba":
                        provLocation = "MB";
                        break;
                    case "New Brunswick":
                        provLocation = "NB";
                        break;
                    case "NL":
                        provLocation = "NL";
                        break;
                    case "NWT":
                        provLocation = "NWT";
                        break;
                    case "Nova Scotia":
                        provLocation = "NS";
                        break;
                    case "Nunavut":
                        provLocation = "NU";
                        break;
                    case "PEI":
                        provLocation = "PE";
                        break;
                    case "Saskatchewan":
                        provLocation = "SK";
                        break;
                    case "Yukon":
                        provLocation = "YT";
                        break;
                    case "Repatriated":
                        provLocation = "RP";
                        break;
                }

                string provinceWide = await client.GetStringAsync("https://api.covid19tracker.ca/reports/province/" + provLocation + "?date=" + vaccineYesterday);

                string canadaWide = await client.GetStringAsync("https://api.covid19tracker.ca/summary");


                JObject localVaccine = JObject.Parse(localVaccines);
                JObject provinceStats = JObject.Parse(provinceWide);
                JObject canadaStats = JObject.Parse(canadaWide);



                //assign variable for the better data from local PHU    
                #region LOCAL VARIABLES

                bool pm;

                try
                {
                    int LocalActiveCases = Int32.Parse($"{betterLocalStats["data"][0]["total_cases"]}") -
                                       Int32.Parse($"{betterLocalStats["data"][0]["total_recoveries"]}") -
                                       Int32.Parse($"{betterLocalStats["data"][0]["total_fatalities"]}");

                    string LocalActiveCasesDisplay = await ShortenNumber(LocalActiveCases.ToString(), false);

                    activeCasesPHU = LocalActiveCasesDisplay;

                    int LocalChangeActiveCases = Int32.Parse($"{betterLocalStats["data"][0]["change_cases"]}") -
                                                 Int32.Parse($"{betterLocalStats["data"][0]["change_fatalities"]}") -
                                                 Int32.Parse($"{betterLocalStats["data"][0]["change_recoveries"]}");

                    pm = LocalChangeActiveCases >= 0;

                    string LocalChangeActiveCasesDisplay = await ShortenNumber(Math.Abs(LocalChangeActiveCases).ToString(), true);

                    newCasesPHU = pm ? $"+{LocalChangeActiveCasesDisplay}" : $"-{LocalChangeActiveCasesDisplay}";

                    string LocalTotalDeathsDisplay = await ShortenNumber($"{betterLocalStats["data"][0]["total_fatalities"]}", false);

                    totalPHUDeaths = LocalTotalDeathsDisplay;

                    int LocalChangeDeaths = Int32.Parse($"{betterLocalStats["data"][0]["change_fatalities"]}");

                    pm = LocalChangeDeaths >= 0;

                    string LocalChangeDeathsDisplay = await ShortenNumber(Math.Abs(LocalChangeDeaths).ToString(), true);

                    newPHUDeaths = pm ? $"+{LocalChangeDeathsDisplay}" : $"-{LocalChangeDeathsDisplay}";

                    string LocalTotalVaccinatedDisplay =
                        await ShortenNumber($"{betterLocalStats["data"][0]["total_vaccinated"]}", false);

                    totalPHUVaccinated = LocalTotalVaccinatedDisplay;

                    int LocalNewVaccinated = Int32.Parse($"{betterLocalStats["data"][0]["change_vaccinated"]}");

                    pm = LocalNewVaccinated >= 0;

                    string LocalChangeVaccinatedDisplay = await ShortenNumber(Math.Abs(LocalNewVaccinated).ToString(), true);

                    newPHUVaccinated = pm ? $"+{LocalChangeVaccinatedDisplay}" : $"-{LocalChangeVaccinatedDisplay}";
                }
                catch (Exception e)
                {
                    Console.WriteLine("Problem");
                }

                #endregion

                #region CANADA WIDE 


                int NationalActiveCases = Int32.Parse($"{canadaStats["data"][0]["total_cases"]}") -
                                       Int32.Parse($"{canadaStats["data"][0]["total_recoveries"]}") -
                                       Int32.Parse($"{canadaStats["data"][0]["total_fatalities"]}");

                string NationalActiveCasesDisplay = await ShortenNumber(NationalActiveCases.ToString(), false);

                activeCasesNationalNumber = NationalActiveCasesDisplay;

                int NationalChangeActiveCases = Int32.Parse($"{canadaStats["data"][0]["change_cases"]}") -
                                             Int32.Parse($"{canadaStats["data"][0]["change_fatalities"]}") -
                                             Int32.Parse($"{canadaStats["data"][0]["change_recoveries"]}");

                pm = NationalChangeActiveCases >= 0;

                string NationalChangeActiveCasesDisplay = await ShortenNumber(Math.Abs(NationalChangeActiveCases).ToString(), true);

                newCasesNationalNumber = pm ? $"+{NationalChangeActiveCasesDisplay}" : $"-{NationalChangeActiveCasesDisplay}";



                string NationalTotalDeathsDisplay = await ShortenNumber($"{canadaStats["data"][0]["total_fatalities"]}", false);

                totalDeathsNationalNumber = NationalTotalDeathsDisplay;

                int NationalChangeDeaths = Int32.Parse($"{canadaStats["data"][0]["change_fatalities"]}");

                pm = NationalChangeDeaths >= 0;

                string NationalChangeDeathsDisplay = await ShortenNumber(Math.Abs(NationalChangeDeaths).ToString(), true);

                newDeathsNationalNumber = pm ? $"+{NationalChangeDeathsDisplay}" : $"-{NationalChangeDeathsDisplay}";

                string NationalTotalVaccinatedDisplay =
                    await ShortenNumber($"{canadaStats["data"][0]["total_vaccinated"]}", false);

                totalVaccinatedNationalNumber = NationalTotalVaccinatedDisplay;

                int NationalNewVaccinated = Int32.Parse($"{canadaStats["data"][0]["change_vaccinated"]}");

                pm = NationalNewVaccinated >= 0;

                string NationalChangeVaccinatedDisplay = await ShortenNumber(Math.Abs(NationalNewVaccinated).ToString(), true);

                newVaccinatedNationalNumber = pm ? $"+{NationalChangeVaccinatedDisplay}" : $"-{NationalChangeVaccinatedDisplay}";

                #endregion

                #region PROVINCIAL


                int ProvincialActiveCases = Int32.Parse($"{provinceStats["data"][0]["total_cases"]}") -
                                       Int32.Parse($"{provinceStats["data"][0]["total_recoveries"]}") -
                                       Int32.Parse($"{provinceStats["data"][0]["total_fatalities"]}");

                string ProvincialActiveCasesDisplay = await ShortenNumber(ProvincialActiveCases.ToString(), false);

                activeCasesProvincialNumber = ProvincialActiveCasesDisplay;

                int ProvincialChangeActiveCases = Int32.Parse($"{provinceStats["data"][0]["change_cases"]}") -
                                             Int32.Parse($"{provinceStats["data"][0]["change_fatalities"]}") -
                                             Int32.Parse($"{provinceStats["data"][0]["change_recoveries"]}");

                pm = ProvincialChangeActiveCases >= 0;

                string ProvincialChangeActiveCasesDisplay = await ShortenNumber(Math.Abs(ProvincialChangeActiveCases).ToString(), true);

                newCasesProvincialNumber = pm ? $"+{ProvincialChangeActiveCasesDisplay}" : $"-{ProvincialChangeActiveCasesDisplay}";



                string ProvincialTotalDeathsDisplay = await ShortenNumber($"{provinceStats["data"][0]["total_fatalities"]}", false);

                totalDeathsProvincialNumber = ProvincialTotalDeathsDisplay;

                int ProvincialChangeDeaths = Int32.Parse($"{provinceStats["data"][0]["change_fatalities"]}");

                pm = ProvincialChangeDeaths >= 0;

                string ProvincialChangeDeathsDisplay = await ShortenNumber(Math.Abs(ProvincialChangeDeaths).ToString(), true);

                newDeathsProvincialNumber = pm ? $"+{ProvincialChangeDeathsDisplay}" : $"-{ProvincialChangeDeathsDisplay}";

                string ProvincialTotalVaccinatedDisplay =
                    await ShortenNumber($"{provinceStats["data"][0]["total_vaccinated"]}", false);

                totalVaccinatedProvincialNumber = ProvincialTotalVaccinatedDisplay;

                int ProvincialNewVaccinated = Int32.Parse($"{provinceStats["data"][0]["change_vaccinated"]}");

                pm = ProvincialNewVaccinated >= 0;

                string ProvincialChangeVaccinatedDisplay = await ShortenNumber(Math.Abs(ProvincialNewVaccinated).ToString(), true);

                newVaccinatedProvincialNumber = pm ? $"+{ProvincialChangeVaccinatedDisplay}" : $"-{ProvincialChangeVaccinatedDisplay}";

                #endregion

                if (Preferences.Get("Stats", "null") == "null")
                {
                    Preferences.Set("Stats", "National");
                    GeneralStatisticsType = Preferences.Get("Stats", "National");
                }

                // wait 15 seconds
                await delayTask;
            }

        }

        public async void SettingsClicked(object sender, EventArgs e)
        {

            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);
            var modalPage = new SettingsPage();
            modalPage.Disappearing += (sender2, e2) =>
            {
                waitHandle.Set();
            };
            await Navigation.PushModalAsync(modalPage);
            await Task.Run(() => waitHandle.WaitOne());

            GeneralStatisticsType = Preferences.Get("Stats", "National");
        }

        public async Task<string> ShortenNumber(string number, bool pm)
        {
            int curNum = Int32.Parse(number);

            Console.WriteLine(number);

            if (curNum < 1e3) // 0 - 999 (1 - 3 digits)
            {
                return number;
            }
            else if (curNum < 1e4) // 1000 - 9999 (4 digits)
            {
                string returnString = (curNum / 1e3).ToString();

                return pm ? $"{substr(returnString, 0, 4)}K" : $"{substr(returnString, 0, 5)}K";
            }
            else if (curNum < 1e5) // 10,000 - 99,999 (5 digits)
            {
                string returnString = (curNum / 1e3).ToString();

                return pm ? $"{substr(returnString, 0, 4)}K" : $"{substr(returnString, 0, 5)}K";

            }
            else if (curNum < 1e6) // 100,000 - 999,999 (6 digits)
            {
                string returnString = (curNum / 1e3).ToString();

                return pm ? $"{substr(returnString, 0, 3)}K" : $"{substr(returnString, 0, 5)}K";

            }
            else if (curNum < 1e7) // 1,000,000 - 9,999,999 (7 digits)
            {
                string returnString = (curNum / 1e6).ToString();

                return pm ? $"{substr(returnString, 0, 4)}M" : $"{substr(returnString, 0, 5)}M";
            }
            else if (curNum < 1e8) // 10,000,000 - 99,999,999 (8 digits)
            {
                string returnString = (curNum / 1e6).ToString();

                return pm ? $"{substr(returnString, 0, 4)}M" : $"{substr(returnString, 0, 5)}M";
            }
            else if (curNum < 1e9) // 100,000,000 - 999,999,999 (9 digits)
            {
                string returnString = (curNum / 1e6).ToString();

                return pm ? $"{substr(returnString, 0, 3)}M" : $"{substr(returnString, 0, 5)}M";

            }
            else // 1,000,000,000 - infinity
            {
                string returnString = (curNum / 1e9).ToString();

                return pm ? $"{substr(returnString, 0, 4)}B" : $"{substr(returnString, 0, 5)}B";

            }
        }
        
        string substr(string number, int startIndex, int length)
        {
            string retString;
            try
            {
                retString = number.Substring(startIndex, length);
            }
            catch (Exception e)
            {
                retString = number.Substring(startIndex);
            }
            return retString;
        }


    }
}