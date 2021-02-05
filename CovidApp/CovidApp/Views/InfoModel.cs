using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CovidApp.Views {
    public class InfoModel {
        public string AdminArea3 { get; set; }
    }
    public class Rootobject {
        public Result[] results { get; set; }
    }
    public class Result {
        public Location[] locations { get; set; }
    }

    public class Location {
        public string adminArea3 { get; set; }
    }

}

