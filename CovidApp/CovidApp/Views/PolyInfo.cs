using NetTopologySuite.Geometries;

namespace CovidApp.Views {
    public class PolyInfo {
        public int hRID; // health region id
        public string freName; // french name
        public string engName; // english name
        public Geometry geom;

        public PolyInfo(int hrid, string frename, string engname) {
            this.hRID = hrid;
            this.freName = frename;
            this.engName = engname;
        }
    }
}