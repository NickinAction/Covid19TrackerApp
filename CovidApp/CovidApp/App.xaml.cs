using Xamarin.Forms;

namespace CovidApp {
    public partial class App : Application {

        public App() {
            InitializeComponent();
            MainPage = new AppShell();
        }

        protected override async void OnStart() {
           
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
