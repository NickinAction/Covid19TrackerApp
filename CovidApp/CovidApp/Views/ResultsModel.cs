using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CovidApp.Views {
    public class ResultsModel {

        public LocationModel[] Results { get; set; }
    }
}
