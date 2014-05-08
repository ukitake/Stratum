using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.GIS
{
    public struct LatLonAlt
    {
        public LatLonAlt(double lat, double lon, double alt = 0.0)
            : this()
        {
            Latitude = lat;
            Longitude = lon;
            Altitude = alt;
        }

        public double Latitude { get; private set; }
        public double Longitude { get; private set; }

        /// <summary>
        /// In Km
        /// </summary>
        public double Altitude { get; private set; }
    }
}
