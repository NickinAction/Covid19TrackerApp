using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace CovidApp.Views {
    class apiLocation {

        public double latitude;
        public double longitude;
        string provinceCode;

        public apiLocation() {
            //random default values that are outside the scope of possible lat/long values
            this.latitude = 200.0;
            this.longitude = 200.0;
            this.provinceCode = "##";
        }

        public apiLocation(double lat, double lon) {
            this.latitude = lat;
            this.longitude = lon;
        }

        public apiLocation(Location location) {
            this.latitude = location.Latitude;
            this.longitude = location.Longitude;
        }

        public bool IsInitialized() { // a function to check whether the lat/long are inside the allowed scope
            //therefore it is checking whether they have been initialized
            return (this.latitude <= 90.0 && this.latitude >= -90.0
                && (this.longitude <= 180.0 && this.longitude >= -180.0));
        }

        public double DistanceFrom(double otherLat, double otherLon) {
            return (Location.CalculateDistance(this.latitude, this.longitude, otherLat, otherLon, DistanceUnits.Kilometers));
        }
    }
}


