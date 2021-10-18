using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.IO;
using NetTopologySuite.Geometries;
using GeoJSON.Net;

namespace CovidApp.Views
{
    class GeoJsonHandling {


        public static async Task<List<PolyInfo>> InitGeoJSON() {


            var regions = new List<PolyInfo>();

            //reading json from resources
            string jsonData;
            using (var stream = await FileSystem.OpenAppPackageFileAsync("file.geojson")) {
                using (var fileReader = new StreamReader(stream)) {
                    jsonData = await fileReader.ReadToEndAsync();
                }
            }

            // creating an NTS geojson reader
            var reader = new NetTopologySuite.IO.GeoJsonReader();

            // read FeatureCollection from geojson
            var featureCollection = reader.Read<GeoJSON.Net.Feature.FeatureCollection>(jsonData);

            // loop through and convert all features
            for (int featureIndex = 0; featureIndex < featureCollection.Features.Count; featureIndex++) {
                // get feature
                var jsonFeature = featureCollection.Features[featureIndex];
                PolyInfo curr;

                //apparently "unversal code", although we wont need anything except MultiPolygon and Polygon
                switch (jsonFeature.Geometry.Type) {
                    case GeoJSONObjectType.Point:
                        break;
                    case GeoJSONObjectType.MultiPoint:
                        break;
                    case GeoJSONObjectType.LineString:
                        break;
                    case GeoJSONObjectType.MultiLineString:
                        break;
                    case GeoJSONObjectType.MultiPolygon: {

                            var multiPolygon = jsonFeature.Geometry as GeoJSON.Net.Geometry.MultiPolygon;

                            var curProperties = new Dictionary<string, object>(jsonFeature.Properties);

                            curr = new PolyInfo(Convert.ToInt32(curProperties["HR_UID"]), curProperties["FRENAME"].ToString(),
                                curProperties["ENGNAME"].ToString());

                            //list of polygons that will be filled 
                            var polys = new List<Polygon>();

                            //coordinate array for outer shell only
                            var lRing = new List<Coordinate>();

                            //creating an array of linear rings
                            var rings = new List<LinearRing>();

                            LinearRing shell;

                            foreach (var poly in multiPolygon.Coordinates) {
                                foreach (var ring in poly.Coordinates) {
                                    if (ring.IsLinearRing()) {
                                        //building an array of coordinates per ring, a
                                        foreach (var coordinate in ring.Coordinates) {
                                            var location = coordinate as GeoJSON.Net.Geometry.Position;

                                            if (location == null) continue;

                                            lRing.Add(new Coordinate(location.Latitude, location.Longitude));
                                        }

                                        // adding that array into the list of LinearRings
                                        rings.Add(new LinearRing(lRing.ToArray()));
                                        lRing.Clear();
                                    }
                                }

                                shell = rings[0];
                                rings.RemoveAt(0);

                                polys.Add(rings.Count == 0 ? new Polygon(shell) : new Polygon(shell, rings.ToArray()));

                                shell = null;
                                rings.Clear();
                                lRing.Clear();
                            }

                            curr.geom = new NetTopologySuite.Geometries.MultiPolygon(polys.ToArray());

                            regions.Add(curr);
                        }
                        break;
                    case GeoJSONObjectType.Polygon: {
                            //creating a dictionary for all the properties of the current Feature
                            var curProperties = new Dictionary<string, object>(jsonFeature.Properties);

                            curr = new PolyInfo(Convert.ToInt32(curProperties["HR_UID"]), curProperties["FRENAME"].ToString(),
                                curProperties["ENGNAME"].ToString());

                            //convert the polygon to GeoJSON Polygon type 
                            var polygon = jsonFeature.Geometry as GeoJSON.Net.Geometry.Polygon;

                            //output number of linear rings in the polygon
                            // Console.WriteLine($"Number of linear rings in polygon: {polygon.Coordinates.Count}");

                            //coordinate array for outer shell only
                            var lRing = new List<Coordinate>();

                            var rings = new List<LinearRing>();

                            LinearRing shell;

                            foreach (var ring in polygon.Coordinates) {
                                if (ring.IsLinearRing()) {
                                    //building an array of coordinates per ring, a
                                    foreach (var coordinate in ring.Coordinates) {
                                        var location = coordinate as GeoJSON.Net.Geometry.Position;

                                        if (location == null) continue;

                                        lRing.Add(new Coordinate(location.Latitude, location.Longitude));
                                    }

                                    // adding that array into the list of LinearRings
                                    rings.Add(new LinearRing(lRing.ToArray()));
                                    lRing.Clear();
                                }
                            }

                            // separate shell from others 
                            shell = rings[0];
                            rings.RemoveAt(0);

                            // create final geometry piece (polygon)
                            curr.geom = rings.Count == 0 ? new Polygon(shell) : new Polygon(shell, rings.ToArray());

                            regions.Add(curr);
                        }

                        break;
                    case GeoJSONObjectType.GeometryCollection:
                        break;
                    case GeoJSONObjectType.Feature:
                        break;
                    case GeoJSONObjectType.FeatureCollection:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();

                }
            }

            return regions;
        }
    }
}
