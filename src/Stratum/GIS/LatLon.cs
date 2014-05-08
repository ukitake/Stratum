using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.GIS
{
    public struct LatLon
    {
        public LatLon(double lat, double lon)
            : this()
        {
            Latitude = lat;
            Longitude = lon;
        }

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        public static LatLon Average(params LatLon[] coords)
        {
            double lat = 0d, lon = 0d;
            for (int i = 0; i < coords.Length; i++)
            {
                lat += coords[i].Latitude;
                lon += coords[i].Longitude;
            }

            lat /= coords.Length;
            lon /= coords.Length;

            return new LatLon(lat, lon);
        }
    }
}
