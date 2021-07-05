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
        public provinceLocation[] locations { get; set; }
    }

    public class provinceLocation {
        public string adminArea3 { get; set; }
    }

}

public class RootobjectGeoJson {
    public Feature[] features { get; set; }
}

public class Feature {
    public string type { get; set; }
    public Properties1 properties { get; set; }

    public string geometry { get; set; }
    //public Geometry1 geometry { get; set; }
}

public class Properties1 {
    public int FID { get; set; }
    public string HR_UID { get; set; }
    public string ENGNAME { get; set; }
    public string FRENAME { get; set; }
    public float Shape__Area { get; set; }
    public float Shape__Length { get; set; }
}

public class Geometry1 {
    public string type { get; set; }
    public object[][][] coordinates { get; set; }
}

