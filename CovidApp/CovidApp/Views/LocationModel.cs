using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CovidApp.Views {
    public class LocationModel {
        public InfoModel[] Locations { get; set; }
    }
}
