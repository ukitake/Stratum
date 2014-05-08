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
            double cd = System.Math.Min(System.Math.Max(d, 0.0), 1.0);
            return cd * cd * (3 - 2 * cd);
        }

        /// <summary>
        /// Clamps the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="min">The min.</param>
        /// <param name="max">The max.</param>
        /// <returns>The result of clamping a value between min and max</returns>
        public static double Clamp(double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }

        public static double RadiansToDegrees(double radians)
        {
            return (180.0 / System.Math.PI) * radians;
        }

        public static double DegreesToRadians(double degrees)
        {
            return (System.Math.PI / 180.0) * degrees;
        }
    }
}
