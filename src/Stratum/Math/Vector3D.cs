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
    public struct Vector3D
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

        public Vector3D(Vector3 v)
        {
            X = v.X;
            Y = v.Y;
            Z = v.Z;
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

        //public void Split(out Vector3 low, out Vector3 high)
        //{
        //    float lowx = split_low(X), highx = split_high(X);
        //    float lowy = split_low(Y), highy = split_high(Y);
        //    float lowz = split_low(Z), highz = split_high(Z);

        //    low = new Vector3(lowx, lowy, lowz);
        //    high = new Vector3(highx, highy, highz);
        //}

        public void Split(out Vector3 low, out Vector3 high)
        {
            float lowx, highx, lowy, highy, lowz, highz;
            ds_split(X, out highx, out lowx);
            ds_split(Y, out highy, out lowy);
            ds_split(Z, out highz, out lowz);

            low = new Vector3(lowx, lowy, lowz);
            high = new Vector3(highx, highy, highz);
        }

        private static void ds_split(double d, out float high, out float low)
        {
            high = (float)d;
            low = (float)(d - (double)high);
        }

        private static unsafe float split_high(double d)
        {
            // reinterpret the double as a ulong
            ulong num = *((ulong*)&d);

            if (BitConverter.IsLittleEndian)
                return *(((float*)(&num)) + 1);
            else
                return *(float*)&num;
        }

        private static unsafe float split_low(double d)
        {
            ulong num = *((ulong*)&d);

            if (!BitConverter.IsLittleEndian)
                return *(((float*)(&num)) + 1);
            else
                return *(float*)&num;
        }

        private static unsafe double merge(float high, float low)
        {
            uint l = *(uint*)&low;
            uint h = *(uint*)&high;
            ulong sum = ((ulong)h << 32) | l;
            return *(double*)&sum;
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

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="SharpDX.Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="SharpDX.Quaternion"/> rotation to apply.</param>
        /// <param name="result">When the method completes, contains the transformed <see cref="SharpDX.Vector4"/>.</param>
        public static void Transform(ref Vector3D vector, ref QuaternionD rotation, out Vector3D result)
        {
            double x = rotation.X + rotation.X;
            double y = rotation.Y + rotation.Y;
            double z = rotation.Z + rotation.Z;
            double wx = rotation.W * x;
            double wy = rotation.W * y;
            double wz = rotation.W * z;
            double xx = rotation.X * x;
            double xy = rotation.X * y;
            double xz = rotation.X * z;
            double yy = rotation.Y * y;
            double yz = rotation.Y * z;
            double zz = rotation.Z * z;

            result = new Vector3D(
                ((vector.X * ((1.0 - yy) - zz)) + (vector.Y * (xy - wz))) + (vector.Z * (xz + wy)),
                ((vector.X * (xy + wz)) + (vector.Y * ((1.0 - xx) - zz))) + (vector.Z * (yz - wx)),
                ((vector.X * (xz - wy)) + (vector.Y * (yz + wx))) + (vector.Z * ((1.0 - xx) - yy)));
        }

        /// <summary>
        /// Transforms a 3D vector by the given <see cref="SharpDX.Quaternion"/> rotation.
        /// </summary>
        /// <param name="vector">The vector to rotate.</param>
        /// <param name="rotation">The <see cref="SharpDX.Quaternion"/> rotation to apply.</param>
        /// <returns>The transformed <see cref="SharpDX.Vector4"/>.</returns>
        public static Vector3D Transform(Vector3D vector, QuaternionD rotation)
        {
            Vector3D result;
            Transform(ref vector, ref rotation, out result);
            return result;
        }

        public static void Transform3x3(ref Vector3D vector, ref MatrixD transform, out Vector3D result)
        {
            result = new Vector3D((vector.X * transform.M11) + (vector.Y * transform.M21) + (vector.Z * transform.M31),
                                    (vector.X * transform.M12) + (vector.Y * transform.M22) + (vector.Z * transform.M32),
                                    (vector.X * transform.M13) + (vector.Y * transform.M23) + (vector.Z * transform.M33)
                                );
        }

        public static Vector3D Transform3x3(Vector3D vector, MatrixD transform)
        {
            Vector3D result;
            Transform3x3(ref vector, ref transform, out result);
            return result;
        }

        public static void Transform(ref Vector3D vector, ref MatrixD transform, out Vector3D result)
        {
            Vector4D intermediate;
            Transform(ref vector, ref transform, out intermediate);
            result = (Vector3D)intermediate;
        }

        public static void Transform(ref Vector3D vector, ref MatrixD transform, out Vector4D result)
        {
            result = new Vector4D(
                (vector.X * transform.M11) + (vector.Y * transform.M21) + (vector.Z * transform.M31) + transform.M41,
                (vector.X * transform.M12) + (vector.Y * transform.M22) + (vector.Z * transform.M32) + transform.M42,
                (vector.X * transform.M13) + (vector.Y * transform.M23) + (vector.Z * transform.M33) + transform.M43,
                (vector.X * transform.M14) + (vector.Y * transform.M24) + (vector.Z * transform.M34) + transform.M44);
        }

        public static Vector4D Transform(Vector3D vector, MatrixD transform)
        {
            Vector4D result;
            Transform(ref vector, ref transform, out result);
            return result;
        }

        public static void Transform(Vector3D[] source, ref MatrixD transform, Vector4D[] destination)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException("destination", "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                Transform(ref source[i], ref transform, out destination[i]);
            }
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="SharpDX.Matrix"/>.
        /// </summary>
        /// <param name="coordinate">The coordinate vector to transform.</param>
        /// <param name="transform">The transformation <see cref="SharpDX.Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed coordinates.</param>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the w component to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        public static void TransformCoordinate(ref Vector3D coordinate, ref MatrixD transform, out Vector3D result)
        {
            Vector4D vector = new Vector4D();
            vector.X = (coordinate.X * transform.M11) + (coordinate.Y * transform.M21) + (coordinate.Z * transform.M31) + transform.M41;
            vector.Y = (coordinate.X * transform.M12) + (coordinate.Y * transform.M22) + (coordinate.Z * transform.M32) + transform.M42;
            vector.Z = (coordinate.X * transform.M13) + (coordinate.Y * transform.M23) + (coordinate.Z * transform.M33) + transform.M43;
            vector.W = 1f / ((coordinate.X * transform.M14) + (coordinate.Y * transform.M24) + (coordinate.Z * transform.M34) + transform.M44);

            result = new Vector3D(vector.X * vector.W, vector.Y * vector.W, vector.Z * vector.W);
        }

        /// <summary>
        /// Performs a coordinate transformation using the given <see cref="SharpDX.Matrix"/>.
        /// </summary>
        /// <param name="coordinate">The coordinate vector to transform.</param>
        /// <param name="transform">The transformation <see cref="SharpDX.Matrix"/>.</param>
        /// <returns>The transformed coordinates.</returns>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the w component to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        public static Vector3D TransformCoordinate(Vector3D coordinate, MatrixD transform)
        {
            Vector3D result;
            TransformCoordinate(ref coordinate, ref transform, out result);
            return result;
        }

        /// <summary>
        /// Performs a coordinate transformation on an array of vectors using the given <see cref="SharpDX.Matrix"/>.
        /// </summary>
        /// <param name="source">The array of coordinate vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="SharpDX.Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        /// <remarks>
        /// A coordinate transform performs the transformation with the assumption that the w component
        /// is one. The four dimensional vector obtained from the transformation operation has each
        /// component in the vector divided by the w component. This forces the w component to be one and
        /// therefore makes the vector homogeneous. The homogeneous vector is often preferred when working
        /// with coordinates as the w component can safely be ignored.
        /// </remarks>
        public static void TransformCoordinate(Vector3D[] source, ref MatrixD transform, Vector3D[] destination)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException("destination", "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                TransformCoordinate(ref source[i], ref transform, out destination[i]);
            }
        }

        /// <summary>
        /// Performs a normal transformation using the given <see cref="SharpDX.Matrix"/>.
        /// </summary>
        /// <param name="normal">The normal vector to transform.</param>
        /// <param name="transform">The transformation <see cref="SharpDX.Matrix"/>.</param>
        /// <param name="result">When the method completes, contains the transformed normal.</param>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth column of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often preferred for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        public static void TransformNormal(ref Vector3D normal, ref MatrixD transform, out Vector3D result)
        {
            result = new Vector3D(
                (normal.X * transform.M11) + (normal.Y * transform.M21) + (normal.Z * transform.M31),
                (normal.X * transform.M12) + (normal.Y * transform.M22) + (normal.Z * transform.M32),
                (normal.X * transform.M13) + (normal.Y * transform.M23) + (normal.Z * transform.M33));
        }

        /// <summary>
        /// Performs a normal transformation using the given <see cref="SharpDX.Matrix"/>.
        /// </summary>
        /// <param name="normal">The normal vector to transform.</param>
        /// <param name="transform">The transformation <see cref="SharpDX.Matrix"/>.</param>
        /// <returns>The transformed normal.</returns>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth column of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often preferred for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        public static Vector3D TransformNormal(Vector3D normal, MatrixD transform)
        {
            Vector3D result;
            TransformNormal(ref normal, ref transform, out result);
            return result;
        }

        /// <summary>
        /// Performs a normal transformation on an array of vectors using the given <see cref="SharpDX.Matrix"/>.
        /// </summary>
        /// <param name="source">The array of normal vectors to transform.</param>
        /// <param name="transform">The transformation <see cref="SharpDX.Matrix"/>.</param>
        /// <param name="destination">The array for which the transformed vectors are stored.
        /// This array may be the same array as <paramref name="source"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source"/> or <paramref name="destination"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="destination"/> is shorter in length than <paramref name="source"/>.</exception>
        /// <remarks>
        /// A normal transform performs the transformation with the assumption that the w component
        /// is zero. This causes the fourth row and fourth column of the matrix to be unused. The
        /// end result is a vector that is not translated, but all other transformation properties
        /// apply. This is often preferred for normal vectors as normals purely represent direction
        /// rather than location because normal vectors should not be translated.
        /// </remarks>
        public static void TransformNormal(Vector3D[] source, ref MatrixD transform, Vector3D[] destination)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (destination == null)
                throw new ArgumentNullException("destination");
            if (destination.Length < source.Length)
                throw new ArgumentOutOfRangeException("destination", "The destination array must be of same length or larger length than the source array.");

            for (int i = 0; i < source.Length; ++i)
            {
                TransformNormal(ref source[i], ref transform, out destination[i]);
            }
        }
    }
}
