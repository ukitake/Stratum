using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.GIS
{
    public static class RenderWGS84
    {
        /// <summary>
        /// This radius should be used for computation of coordinates used to render things... this radius 
        /// isn't exactly accurate as most GIS systems use 6371 km as the geoid radius, but the Atmospheric
        /// Scattering seems to require this radius for now.
        /// </summary>
        public const double EarthRadius = 6360.0;

        public static Vector3D ToWorld(double latitude, double longitude)
        {
            Vector3D ret = new Vector3D();
            double lat = MathUtilD.DegreesToRadians(latitude);
            double lon = MathUtilD.DegreesToRadians(longitude);

            ret.X = EarthRadius * Math.Cos(lat) * Math.Cos(lon);
            ret.Y = EarthRadius * Math.Sin(lat);
            ret.Z = EarthRadius * Math.Cos(lat) * Math.Sin(lon);

            return ret;
        }

        public static Vector3D ToGoogleBing(double lon, double lat)
        {
            double x = lon * 20037508.34 / 180;
            double y = Math.Log(Math.Tan((90 + lat) * Math.PI / 360)) / (Math.PI / 180);
            y = y * 20037508.34 / 180;
            return new Vector3D(x, y, 0.0);
        }

        public static LatLon FromGoogleBing(double x, double y)
        {
            double lon = (x / 20037508.34) * 180;
            double lat = (y / 20037508.34) * 180;

            lat = 180 / Math.PI * (2 * Math.Atan(Math.Exp(lat * Math.PI / 180)) - Math.PI / 2);
            return new LatLon(lat, lon);
        }
    }
}
