using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpDX;
using SharpDX.Serialization;

namespace Stratum
{
    [StructLayout(LayoutKind.Sequential, Pack=4)]
    public struct Vector3D // : IEquatable<Vector3D>, IFormattable, IDataSerializable
    {
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(Vector3D));

        public static readonly Vector3D Zero = new Vector3D();

        /// <summary>
        /// The X unit (1, 0, 0).
        /// </summary>
        public static readonly Vector3D UnitX = new Vector3D(1.0d, 0.0d, 0.0d);

        /// <summary>
        /// The Y unit (0, 1, 0).
        /// </summary>
        public static readonly Vector3D UnitY = new Vector3D(0.0d, 1.0d, 0.0d);

        /// <summary>
        /// The Z unit (0, 0, 1).
        /// </summary>
        public static readonly Vector3D UnitZ = new Vector3D(0.0d, 0.0d, 1.0d);

        /// <summary>
        /// A with all of its components set to one.
        /// </summary>
        public static readonly Vector3D One = new Vector3D(1.0d, 1.0d, 1.0d);

        public Vector3D(double val)
        {
            X = val;
            Y = val;
            Z = val;
        }

        public Vector3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X;
        public double Y;
        public double Z;

        public bool IsNormalized
        {
            get { return System.Math.Abs((X * X) + (Y * Y) + (Z * Z) - 1d) < MathUtil.ZeroTolerance; }
        }

        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for Vector3D run from 0 to 2, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for Vector3D run from 0 to 2, inclusive.");
                }
            }
        }

        public double Length()
        {
            return System.Math.Sqrt((X * X) + (Y * Y) + (Z * Z));
        }

        public double LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z);
        }

        public void Normalize()
        {
            double length = Length();
            if (length > MathUtil.ZeroTolerance)
            {
                double inv = 1.0d / length;
                X *= inv;
                Y *= inv;
                Z *= inv;
            }
        }

        public double[] ToArray()
        {
            return new double[] { X, Y, Z };
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)X, (float)Y, (float)Z);
        }

        public static void Negate(ref Vector3D value, out Vector3D result)
        {
            result = new Vector3D(-value.X, -value.Y, -value.Z);
        }

        public static Vector3D Negate(Vector3D value)
        {
            return new Vector3D(-value.X, -value.Y, -value.Z);
        }

        public static void Barycentric(ref Vector3D value1, ref Vector3D value2, ref Vector3D value3, double amount1, double amount2, out Vector3D result)
        {
            result = new Vector3D((value1.X + (amount1 * (value2.X - value1.X))) + (amount2 * (value3.X - value1.X)),
                (value1.Y + (amount1 * (value2.Y - value1.Y))) + (amount2 * (value3.Y - value1.Y)),
                (value1.Z + (amount1 * (value2.Z - value1.Z))) + (amount2 * (value3.Z - value1.Z)));
        }

        public static Vector3D Barycentric(Vector3D value1, Vector3D value2, Vector3D value3, double amount1, double amount2)
        {
            Vector3D result;
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out result);
            return result;
        }

        public static void Clamp(ref Vector3D value, ref Vector3D min, ref Vector3D max, out Vector3D result)
        {
            double x = value.X;
            x = (x > max.X) ? max.X : x;
            x = (x < min.X) ? min.X : x;

            double y = value.Y;
            y = (y > max.Y) ? max.Y : y;
            y = (y < min.Y) ? min.Y : y;

            double z = value.Z;
            z = (z > max.Z) ? max.Z : z;
            z = (z < min.Z) ? min.Z : z;

            result = new Vector3D(x, y, z);
        }

        public static Vector3D Clamp(Vector3D value, Vector3D min, Vector3D max)
        {
            Vector3D result;
            Clamp(ref value, ref min, ref max, out result);
            return result;
        }

        public static void Cross(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            result = new Vector3D(
                (left.Y * right.Z) - (left.Z * right.Y),
                (left.Z * right.X) - (left.X * right.Z),
                (left.X * right.Y) - (left.Y * right.X));
        }

        public static Vector3D Cross(Vector3D left, Vector3D right)
        {
            Vector3D result;
            Cross(ref left, ref right, out result);
            return result;
        }

        public static void Distance(ref Vector3D value1, ref Vector3D value2, out double result)
        {
            double x = value1.X - value2.X;
            double y = value1.Y - value2.Y;
            double z = value1.Z - value2.Z;

            result = System.Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        public static double Distance(Vector3D value1, Vector3D value2)
        {
            double x = value1.X - value2.X;
            double y = value1.Y - value2.Y;
            double z = value1.Z - value2.Z;

            return System.Math.Sqrt((x * x) + (y * y) + (z * z));
        }

        public static void DistanceSquared(ref Vector3D value1, ref Vector3D value2, out double result)
        {
            double x = value1.X - value2.X;
            double y = value1.Y - value2.Y;
            double z = value1.Z - value2.Z;

            result = (x * x) + (y * y) + (z * z);
        }

        public static double DistanceSquared(Vector3D value1, Vector3D value2)
        {
            double x = value1.X - value2.X;
            double y = value1.Y - value2.Y;
            double z = value1.Z - value2.Z;

            return (x * x) + (y * y) + (z * z);
        }

        public static void Dot(ref Vector3D left, ref Vector3D right, out double result)
        {
            result = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

        public static double Dot(Vector3D left, Vector3D right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z);
        }

        public static void Normalize(ref Vector3D value, out Vector3D result)
        {
            result = value;
            result.Normalize();
        }

        public static Vector3D Normalize(Vector3D value)
        {
            value.Normalize();
            return value;
        }

        public static void Lerp(ref Vector3D start, ref Vector3D end, double amount, out Vector3D result)
        {
            result.X = start.X + ((end.X - start.X) * amount);
            result.Y = start.Y + ((end.Y - start.Y) * amount);
            result.Z = start.Z + ((end.Z - start.Z) * amount);
        }

        public static Vector3D Lerp(Vector3D start, Vector3D end, float amount)
        {
            Vector3D result;
            Lerp(ref start, ref end, amount, out result);
            return result;
        }

        public static void SmoothStep(ref Vector3D start, ref Vector3D end, double amount, out Vector3D result)
        {
            amount = (amount > 1.0d) ? 1.0d : ((amount < 0.0d) ? 0.0d : amount);
            amount = (amount * amount) * (3.0d - (2.0d * amount));

            result.X = start.X + ((end.X - start.X) * amount);
            result.Y = start.Y + ((end.Y - start.Y) * amount);
            result.Z = start.Z + ((end.Z - start.Z) * amount);
        }

        public static Vector3D SmoothStep(Vector3D start, Vector3D end, double amount)
        {
            Vector3D result;
            SmoothStep(ref start, ref end, amount, out result);
            return result;
        }

        public static void Max(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            result.X = (left.X > right.X) ? left.X : right.X;
            result.Y = (left.Y > right.Y) ? left.Y : right.Y;
            result.Z = (left.Z > right.Z) ? left.Z : right.Z;
        }

        public static Vector3D Max(Vector3D left, Vector3D right)
        {
            Vector3D result;
            Max(ref left, ref right, out result);
            return result;
        }

        public static void Min(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            result.X = (left.X < right.X) ? left.X : right.X;
            result.Y = (left.Y < right.Y) ? left.Y : right.Y;
            result.Z = (left.Z < right.Z) ? left.Z : right.Z;
        }

        public static Vector3D Min(Vector3D left, Vector3D right)
        {
            Vector3D result;
            Min(ref left, ref right, out result);
            return result;
        }

        public static void Transform(ref Vector3D vector, ref Quaternion rotation, out Vector3D result)
        {
            float x = rotation.X + rotation.X;
            float y = rotation.Y + rotation.Y;
            float z = rotation.Z + rotation.Z;
            float wx = rotation.W * x;
            float wy = rotation.W * y;
            float wz = rotation.W * z;
            float xx = rotation.X * x;
            float xy = rotation.X * y;
            float xz = rotation.X * z;
            float yy = rotation.Y * y;
            float yz = rotation.Y * z;
            float zz = rotation.Z * z;

            result = new Vector3D(
                ((vector.X * ((1.0d - yy) - zz)) + (vector.Y * (xy - wz))) + (vector.Z * (xz + wy)),
                ((vector.X * (xy + wz)) + (vector.Y * ((1.0d - xx) - zz))) + (vector.Z * (yz - wx)),
                ((vector.X * (xz - wy)) + (vector.Y * (yz + wx))) + (vector.Z * ((1.0d - xx) - yy)));
        }

        public static Vector3D Transform(Vector3D vector, Quaternion rotation)
        {
            Vector3D result;
            Transform(ref vector, ref rotation, out result);
            return result;
        }

        public static void Add(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            result = new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static Vector3D Add(Vector3D left, Vector3D right)
        {
            return new Vector3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
        }

        public static void Subtract(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            result = new Vector3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static Vector3D Subtract(Vector3D left, Vector3D right)
        {
            return new Vector3D(left.X - right.X, left.Y - right.Y, left.Z - right.Z);
        }

        public static void Multiply(ref Vector3D left, ref Vector3D right, out Vector3D result)
        {
            result = new Vector3D(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        public static Vector3D Multiply(Vector3D left, Vector3D right)
        {
            return new Vector3D(left.X * right.X, left.Y * right.Y, left.Z * right.Z);
        }

        public static Vector3D operator *(Vector3D vec, double d)
        {
            return new Vector3D(vec.X * d, vec.Y * d, vec.Z * d);
        }

        public static Vector3D operator *(double d, Vector3D vec)
        {
            return new Vector3D(vec.X * d, vec.Y * d, vec.Z * d);
        }

        public static Vector3D operator *(Vector3D vec1, Vector3D vec2)
        {
            return new Vector3D(vec1.X * vec2.X, vec1.Y * vec2.Y, vec1.Z * vec2.Z);
        }

        public static Vector3D operator +(Vector3D vec, double d)
        {
            return new Vector3D(vec.X + d, vec.Y + d, vec.Z + d);
        }

        public static Vector3D operator +(Vector3D vec, Vector3D vec2)
        {
            return new Vector3D(vec.X + vec2.X, vec.Y + vec2.Y, vec.Z + vec2.Z);
        }

        public static Vector3D operator -(Vector3D vec, double d)
        {
            return new Vector3D(vec.X - d, vec.Y - d, vec.Z - d);
        }

        public static Vector3D operator -(Vector3D vec, Vector3D vec2)
        {
            return new Vector3D(vec.X - vec2.X, vec.Y - vec2.Y, vec.Z - vec2.Z);
        }

        public static Vector3D operator -(Vector3D value)
        {
            return new Vector3D(-value.X, -value.Y, -value.Z);
        }

        public static Vector3D operator /(Vector3D vec, double d)
        {
            return new Vector3D(vec.X / d, vec.Y / d, vec.Z / d);
        }

        public static Vector3D operator /(Vector3D vec1, Vector3D vec2)
        {
            return new Vector3D(vec1.X / vec2.X, vec1.Y / vec2.Y, vec1.Z / vec2.Z);
        }

        public static Vector3D Average(params Vector3D[] vecs)
        {
            Vector3D sum = Vector3D.Zero;
            for (int i = 0; i < vecs.Length; i++)
                sum += vecs[i];

            return sum / vecs.Length;
        }
    }
}
