using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum
{
    public static class FPPrecisionHelper
    {
        public static double D_RELATIVE_TOLERANCE = 0.000001;
        public static double D_ABSOLUTE_TOLERANCE = 0.000001;

        public static float EPSILON = 0.0000001f;
        public static float F_RELATIVE_TOLERANCE = 0.000001f;
        public static float F_ABSOLUTE_TOLERANCE = 0.000001f;

        public static bool Equal(float f1, float f2)
        {
            return System.Math.Abs(f1 - f2) <= F_ABSOLUTE_TOLERANCE || System.Math.Abs(f1 - f2) <= F_RELATIVE_TOLERANCE * System.Math.Max(System.Math.Abs(f1), System.Math.Abs(f2));
        }

        public static bool Equal(double d1, double d2)
        {
            return System.Math.Abs(d1 - d2) <= D_ABSOLUTE_TOLERANCE || System.Math.Abs(d1 - d2) <= D_RELATIVE_TOLERANCE * System.Math.Max(System.Math.Abs(d1), System.Math.Abs(d2));
        }
    }
}
