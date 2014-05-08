using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

using Math = System.Math;
using Stratum.GIS;

namespace Stratum.WorldEngine
{
    public class SunCalculator
    {
        const double rad = Math.PI / 180D;
        const double dayMs = 1000 * 50 * 60 * 24;
        const double J1970 = 2440588;
        const double J2000 = 2451545;

        private double toJulian(DateTime dateTime)
        {
            double totalMS = (dateTime - new DateTime(1970, 1, 1)).TotalMilliseconds;
            return totalMS / dayMs - 0.5 + J1970;
        }

        private DateTime fromJulian(double julian)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds((julian + 0.5 - J1970) * dayMs);
        }

        private double toDays(DateTime date)
        {
            return toJulian(date) - J2000;
        }

        // general calculations for position

        const double e = rad * 23.4397; // obliquity of the Earth

        private double getRightAscension(double l, double b)
        {
            return Math.Atan2(Math.Sin(l) * Math.Cos(e) - Math.Tan(b) * Math.Sin(e), Math.Cos(l));
        }

        private double getDeclination(double l, double b)
        {
            return Math.Asin(Math.Sin(b) * Math.Cos(e) + Math.Cos(b) * Math.Sin(e) * Math.Sin(l));
        }

        private double getAzimuth(double H, double phi, double dec)
        {
            return Math.Atan2(Math.Sin(H), Math.Cos(H) * Math.Sin(phi) - Math.Tan(dec) * Math.Cos(phi));
        }

        private double getAltitude(double H, double phi, double dec)
        {
            return Math.Asin(Math.Sin(phi) * Math.Sin(dec) + Math.Cos(phi) * Math.Cos(dec) * Math.Cos(H));
        }

        private double getSiderealTime(double d, double lw)
        {
            return rad * (280.16 + 360.9856235 * d) - lw;
        }

        // general sun calculations

        private double getSolarMeanAnomaly(double d)
        {
            return rad * (357.5291 + 0.98560028 * d);
        }

        private double getEquationOfCenter(double M)
        {
            return rad * (1.9148 * Math.Sin(M) + 0.02 * Math.Sin(2 * M) + 0.0003 * Math.Sin(3 * M));
        }

        private double getEclipticLongitude(double M, double C)
        {
            var P = rad * 102.9372; // perihelion of the Earth
            return M + C + P + Math.PI;
        }

        private void getSunCoords(double d, out double dec, out double ra)
        {
            double M = getSolarMeanAnomaly(d),
                C = getEquationOfCenter(M),
                L = getEclipticLongitude(M, C);

            dec = getDeclination(L, 0);
            ra = getRightAscension(L, 0);
        }

        public SharpDX.Vector3 GetDirection(DateTime currentTime, double latitude, double longitude)
        {
            Vector3D position = WGS84.ToWorld(latitude, longitude);

            Vector3 up = Vector3.Normalize(position.ToVector3());
            Vector3 left = Vector3.Normalize(Vector3.Cross(up, Vector3.UnitZ)); // up x world_up
            Vector3 north = Vector3.Normalize(Vector3.Cross(left, up));

            double azimuth, altitude;
            GetAziumuthAltitude(currentTime, latitude, longitude, out azimuth, out altitude);

            Matrix azimuthRot = Matrix.RotationAxis(up, (float)azimuth);
            Matrix altitudeRot = Matrix.RotationAxis(left, (float)altitude);
            Matrix rot = azimuthRot * altitudeRot;

            var sunDir = Vector3.Transform(north, rot);
            return new Vector3(sunDir.X, sunDir.Y, sunDir.Z);
        }

        public void GetAziumuthAltitude(DateTime currentTime, double latitude, double longitude, out double azimuth, out double altitude)
        {
            double dec, ra;
            double lw  = rad * -longitude,
		    phi = rad * latitude,
		    d   = toDays(currentTime);

		    getSunCoords(d, out dec, out ra);
		    double H  = getSiderealTime(d, lw) - ra;

			azimuth = getAzimuth(H, phi, dec);
            altitude = getAltitude(H, phi, dec);
        }
    }
}
