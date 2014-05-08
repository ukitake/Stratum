using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.GIS
{
    public static class WGS84
    {
        public const double EarthRadius = 6371.0;

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
    }
}
