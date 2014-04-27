using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Stratum
{
    public static class MathUtilD
    {
        public static bool WithinEpsilon(double a, double b)
        {
            double num = a - b;
            return ((-double.Epsilon <= num) && (num <= double.Epsilon));
        }

        public static bool IsZero(double d)
        {
            return -double.Epsilon <= d && d <= double.Epsilon;
        }

        public static double SmoothStep(double d)
        {
            double cd = Math.Min(Math.Max(d, 0.0), 1.0);
            return cd * cd * (3 - 2 * cd);
        }

        public static double RadiansToDegrees(double radians)
        {
            return (180.0 / Math.PI) * radians;
        }

        public static double DegreesToRadians(double degrees)
        {
            return (Math.PI / 180.0) * degrees;
        }
    }
}
