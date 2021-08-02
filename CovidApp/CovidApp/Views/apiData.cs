using System;
using System.Collections.Generic;
using System.Text;

namespace CovidApp.Views
{
    class apiData
    {

        public class Rootobject
        {
            public Summary[] summary { get; set; }
        }

        public class Summary
        {
            public int active_cases { get; set; }
            public int active_cases_change { get; set; }
            public float avaccine { get; set; }
            public int cases { get; set; }
            public float cumulative_avaccine { get; set; }
            public int cumulative_cases { get; set; }
            public float cumulative_cvaccine { get; set; }
            public float cumulative_deaths { get; set; }
            public float cumulative_dvaccine { get; set; }
            public float cumulative_recovered { get; set; }
            public float cumulative_testing { get; set; }
            public float cvaccine { get; set; }
            public string date { get; set; }
            public float deaths { get; set; }
            public float dvaccine { get; set; }
            public string province { get; set; }
            public float recovered { get; set; }
            public float testing { get; set; }
            public string testing_info { get; set; }
        }

    }
}
