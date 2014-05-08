using SharpDX.Serialization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stratum
{
    /// <summary>
    /// Represents a four dimensional mathematical QuaternionD.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct QuaternionD : IEquatable<QuaternionD>, IFormattable, IDataSerializable
    {
        /// <summary>
        /// The size of the <see cref="SharpDX.QuaternionD"/> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(QuaternionD));

        /// <summary>
        /// A <see cref="SharpDX.QuaternionD"/> with all of its components set to zero.
        /// </summary>
        public static readonly QuaternionD Zero = new QuaternionD();

        /// <summary>
        /// A <see cref="SharpDX.QuaternionD"/> with all of its components set to one.
        /// </summary>
        public static readonly QuaternionD One = new QuaternionD(1.0, 1.0, 1.0, 1.0);

        /// <summary>
        /// The identity <see cref="SharpDX.QuaternionD"/> (0, 0, 0, 1).
        /// </summary>
        public static readonly QuaternionD Identity = new QuaternionD(0.0, 0.0, 0.0, 1.0);

        /// <summary>
        /// The X component of the QuaternionD.
        /// </summary>
        public double X;

        /// <summary>
        /// The Y component of the QuaternionD.
        /// </summary>
        public double Y;

        /// <summary>
        /// The Z component of the QuaternionD.
        /// </summary>
        public double Z;

        /// <summary>
        /// The W component of the QuaternionD.
        /// </summary>
        public double W;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDX.QuaternionD"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public QuaternionD(double value)
        {
            X = value;
            Y = value;
            Z = value;
            W = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDX.QuaternionD"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the components.</param>
        public QuaternionD(Vector4D value)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = value.W;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDX.QuaternionD"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the X, Y, and Z components.</param>
        /// <param name="w">Initial value for the W component of the QuaternionD.</param>
        public QuaternionD(Vector3D value, double w)
        {
            X = value.X;
            Y = value.Y;
            Z = value.Z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDX.QuaternionD"/> struct.
        /// </summary>
        /// <param name="value">A vector containing the values with which to initialize the X and Y components.</param>
        /// <param name="z">Initial value for the Z component of the QuaternionD.</param>
        /// <param name="w">Initial value for the W component of the QuaternionD.</param>
        public QuaternionD(Vector2D value, double z, double w)
        {
            X = value.X;
            Y = value.Y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDX.QuaternionD"/> struct.
        /// </summary>
        /// <param name="x">Initial value for the X component of the QuaternionD.</param>
        /// <param name="y">Initial value for the Y component of the QuaternionD.</param>
        /// <param name="z">Initial value for the Z component of the QuaternionD.</param>
        /// <param name="w">Initial value for the W component of the QuaternionD.</param>
        public QuaternionD(double x, double y, double z, double w)
        {
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDX.QuaternionD"/> struct.
        /// </summary>
        /// <param name="values">The values to assign to the X, Y, Z, and W components of the QuaternionD. This must be an array with four elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than four elements.</exception>
        public QuaternionD(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 4)
                throw new ArgumentOutOfRangeException("values", "There must be four and only four input values for QuaternionD.");

            X = values[0];
            Y = values[1];
            Z = values[2];
            W = values[3];
        }

        /// <summary>
        /// Gets a value indicating whether this instance is equivalent to the identity QuaternionD.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an identity QuaternionD; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdentity
        {
            get { return this.Equals(Identity); }
        }

        /// <summary>
        /// Gets a value indicting whether this instance is normalized.
        /// </summary>
        //public bool IsNormalized
        //{
        //    get { return MathUtilD.IsOne((X * X) + (Y * Y) + (Z * Z) + (W * W)); }
        //}

        /// <summary>
        /// Gets the angle of the QuaternionD.
        /// </summary>
        /// <value>The QuaternionD's angle.</value>
        public double Angle
        {
            get
            {
                double length = (X * X) + (Y * Y) + (Z * Z);
                if (MathUtilD.IsZero(length))
                    return 0.0;

                return (double)(2.0 * System.Math.Acos(MathUtilD.Clamp(W, -1.0, 1.0)));
            }
        }

        /// <summary>
        /// Gets the axis components of the QuaternionD.
        /// </summary>
        /// <value>The axis components of the QuaternionD.</value>
        public Vector3D Axis
        {
            get
            {
                double length = (X * X) + (Y * Y) + (Z * Z);
                if (MathUtilD.IsZero(length))
                    return Vector3D.UnitX;

                double inv = 1.0 / System.Math.Sqrt(length);
                return new Vector3D(X * inv, Y * inv, Z * inv);
            }
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the X, Y, Z, or W component, depending on the index.</value>
        /// <param name="index">The index of the component to access. Use 0 for the X component, 1 for the Y component, 2 for the Z component, and 3 for the W component.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 3].</exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return X;
                    case 1: return Y;
                    case 2: return Z;
                    case 3: return W;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for QuaternionD run from 0 to 3, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: X = value; break;
                    case 1: Y = value; break;
                    case 2: Z = value; break;
                    case 3: W = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for QuaternionD run from 0 to 3, inclusive.");
                }
            }
        }

        /// <summary>
        /// Conjugates the QuaternionD.
        /// </summary>
        public void Conjugate()
        {
            X = -X;
            Y = -Y;
            Z = -Z;
        }

        /// <summary>
        /// Conjugates and renormalizes the QuaternionD.
        /// </summary>
        public void Invert()
        {
            double lengthSq = LengthSquared();
            if (!MathUtilD.IsZero(lengthSq))
            {
                lengthSq = 1.0 / lengthSq;

                X = -X * lengthSq;
                Y = -Y * lengthSq;
                Z = -Z * lengthSq;
                W = W * lengthSq;
            }
        }

        /// <summary>
        /// Calculates the length of the QuaternionD.
        /// </summary>
        /// <returns>The length of the QuaternionD.</returns>
        /// <remarks>
        /// <see cref="SharpDX.QuaternionD.LengthSquared"/> may be preferred when only the relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public double Length()
        {
            return System.Math.Sqrt((X * X) + (Y * Y) + (Z * Z) + (W * W));
        }

        /// <summary>
        /// Calculates the squared length of the QuaternionD.
        /// </summary>
        /// <returns>The squared length of the QuaternionD.</returns>
        /// <remarks>
        /// This method may be preferred to <see cref="SharpDX.QuaternionD.Length"/> when only a relative length is needed
        /// and speed is of the essence.
        /// </remarks>
        public double LengthSquared()
        {
            return (X * X) + (Y * Y) + (Z * Z) + (W * W);
        }

        /// <summary>
        /// Converts the QuaternionD into a unit QuaternionD.
        /// </summary>
        public void Normalize()
        {
            double length = Length();
            if (!MathUtilD.IsZero(length))
            {
                double inverse = 1.0 / length;
                X *= inverse;
                Y *= inverse;
                Z *= inverse;
                W *= inverse;
            }
        }

        /// <summary>
        /// Creates an array containing the elements of the QuaternionD.
        /// </summary>
        /// <returns>A four-element array containing the components of the QuaternionD.</returns>
        public double[] ToArray()
        {
            return new double[] { X, Y, Z, W };
        }

        /// <summary>
        /// Adds two QuaternionDs.
        /// </summary>
        /// <param name="left">The first QuaternionD to add.</param>
        /// <param name="right">The second QuaternionD to add.</param>
        /// <param name="result">When the method completes, contains the sum of the two QuaternionDs.</param>
        public static void Add(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
        {
            result.X = left.X + right.X;
            result.Y = left.Y + right.Y;
            result.Z = left.Z + right.Z;
            result.W = left.W + right.W;
        }

        /// <summary>
        /// Adds two QuaternionDs.
        /// </summary>
        /// <param name="left">The first QuaternionD to add.</param>
        /// <param name="right">The second QuaternionD to add.</param>
        /// <returns>The sum of the two QuaternionDs.</returns>
        public static QuaternionD Add(QuaternionD left, QuaternionD right)
        {
            QuaternionD result;
            Add(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Subtracts two QuaternionDs.
        /// </summary>
        /// <param name="left">The first QuaternionD to subtract.</param>
        /// <param name="right">The second QuaternionD to subtract.</param>
        /// <param name="result">When the method completes, contains the difference of the two QuaternionDs.</param>
        public static void Subtract(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
        {
            result.X = left.X - right.X;
            result.Y = left.Y - right.Y;
            result.Z = left.Z - right.Z;
            result.W = left.W - right.W;
        }

        /// <summary>
        /// Subtracts two QuaternionDs.
        /// </summary>
        /// <param name="left">The first QuaternionD to subtract.</param>
        /// <param name="right">The second QuaternionD to subtract.</param>
        /// <returns>The difference of the two QuaternionDs.</returns>
        public static QuaternionD Subtract(QuaternionD left, QuaternionD right)
        {
            QuaternionD result;
            Subtract(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Scales a QuaternionD by the given value.
        /// </summary>
        /// <param name="value">The QuaternionD to scale.</param>
        /// <param name="scale">The amount by which to scale the QuaternionD.</param>
        /// <param name="result">When the method completes, contains the scaled QuaternionD.</param>
        public static void Multiply(ref QuaternionD value, double scale, out QuaternionD result)
        {
            result.X = value.X * scale;
            result.Y = value.Y * scale;
            result.Z = value.Z * scale;
            result.W = value.W * scale;
        }

        /// <summary>
        /// Scales a QuaternionD by the given value.
        /// </summary>
        /// <param name="value">The QuaternionD to scale.</param>
        /// <param name="scale">The amount by which to scale the QuaternionD.</param>
        /// <returns>The scaled QuaternionD.</returns>
        public static QuaternionD Multiply(QuaternionD value, double scale)
        {
            QuaternionD result;
            Multiply(ref value, scale, out result);
            return result;
        }

        /// <summary>
        /// Multiplies a QuaternionD by another.
        /// </summary>
        /// <param name="left">The first QuaternionD to multiply.</param>
        /// <param name="right">The second QuaternionD to multiply.</param>
        /// <param name="result">When the method completes, contains the multiplied QuaternionD.</param>
        public static void Multiply(ref QuaternionD left, ref QuaternionD right, out QuaternionD result)
        {
            double lx = left.X;
            double ly = left.Y;
            double lz = left.Z;
            double lw = left.W;
            double rx = right.X;
            double ry = right.Y;
            double rz = right.Z;
            double rw = right.W;
            double a = (ly * rz - lz * ry);
            double b = (lz * rx - lx * rz);
            double c = (lx * ry - ly * rx);
            double d = (lx * rx + ly * ry + lz * rz);
            result.X = (lx * rw + rx * lw) + a;
            result.Y = (ly * rw + ry * lw) + b;
            result.Z = (lz * rw + rz * lw) + c;
            result.W = lw * rw - d;
        }

        /// <summary>
        /// Multiplies a QuaternionD by another.
        /// </summary>
        /// <param name="left">The first QuaternionD to multiply.</param>
        /// <param name="right">The second QuaternionD to multiply.</param>
        /// <returns>The multiplied QuaternionD.</returns>
        public static QuaternionD Multiply(QuaternionD left, QuaternionD right)
        {
            QuaternionD result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Reverses the direction of a given QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to negate.</param>
        /// <param name="result">When the method completes, contains a QuaternionD facing in the opposite direction.</param>
        public static void Negate(ref QuaternionD value, out QuaternionD result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = -value.W;
        }

        /// <summary>
        /// Reverses the direction of a given QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to negate.</param>
        /// <returns>A QuaternionD facing in the opposite direction.</returns>
        public static QuaternionD Negate(QuaternionD value)
        {
            QuaternionD result;
            Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Returns a <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <param name="result">When the method completes, contains a new <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of the specified point.</param>
        public static void Barycentric(ref QuaternionD value1, ref QuaternionD value2, ref QuaternionD value3, double amount1, double amount2, out QuaternionD result)
        {
            QuaternionD start, end;
            Slerp(ref value1, ref value2, amount1 + amount2, out start);
            Slerp(ref value1, ref value3, amount1 + amount2, out end);
            Slerp(ref start, ref end, amount2 / (amount1 + amount2), out result);
        }

        /// <summary>
        /// Returns a <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of a point specified in Barycentric coordinates relative to a 2D triangle.
        /// </summary>
        /// <param name="value1">A <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in <paramref name="value2"/>).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in <paramref name="value3"/>).</param>
        /// <returns>A new <see cref="SharpDX.QuaternionD"/> containing the 4D Cartesian coordinates of the specified point.</returns>
        public static QuaternionD Barycentric(QuaternionD value1, QuaternionD value2, QuaternionD value3, double amount1, double amount2)
        {
            QuaternionD result;
            Barycentric(ref value1, ref value2, ref value3, amount1, amount2, out result);
            return result;
        }

        /// <summary>
        /// Conjugates a QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to conjugate.</param>
        /// <param name="result">When the method completes, contains the conjugated QuaternionD.</param>
        public static void Conjugate(ref QuaternionD value, out QuaternionD result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            result.W = value.W;
        }

        /// <summary>
        /// Conjugates a QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to conjugate.</param>
        /// <returns>The conjugated QuaternionD.</returns>
        public static QuaternionD Conjugate(QuaternionD value)
        {
            QuaternionD result;
            Conjugate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Calculates the dot product of two QuaternionDs.
        /// </summary>
        /// <param name="left">First source QuaternionD.</param>
        /// <param name="right">Second source QuaternionD.</param>
        /// <param name="result">When the method completes, contains the dot product of the two QuaternionDs.</param>
        public static void Dot(ref QuaternionD left, ref QuaternionD right, out double result)
        {
            result = (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        }

        /// <summary>
        /// Calculates the dot product of two QuaternionDs.
        /// </summary>
        /// <param name="left">First source QuaternionD.</param>
        /// <param name="right">Second source QuaternionD.</param>
        /// <returns>The dot product of the two QuaternionDs.</returns>
        public static double Dot(QuaternionD left, QuaternionD right)
        {
            return (left.X * right.X) + (left.Y * right.Y) + (left.Z * right.Z) + (left.W * right.W);
        }

        /// <summary>
        /// Exponentiates a QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to exponentiate.</param>
        /// <param name="result">When the method completes, contains the exponentiated QuaternionD.</param>
        public static void Exponential(ref QuaternionD value, out QuaternionD result)
        {
            double angle = System.Math.Sqrt((value.X * value.X) + (value.Y * value.Y) + (value.Z * value.Z));
            double sin = System.Math.Sin(angle);

            if (!MathUtilD.IsZero(sin))
            {
                double coeff = sin / angle;
                result.X = coeff * value.X;
                result.Y = coeff * value.Y;
                result.Z = coeff * value.Z;
            }
            else
            {
                result = value;
            }

            result.W = (double)Math.Cos(angle);
        }

        /// <summary>
        /// Exponentiates a QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to exponentiate.</param>
        /// <returns>The exponentiated QuaternionD.</returns>
        public static QuaternionD Exponential(QuaternionD value)
        {
            QuaternionD result;
            Exponential(ref value, out result);
            return result;
        }

        /// <summary>
        /// Conjugates and renormalizes the QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to conjugate and renormalize.</param>
        /// <param name="result">When the method completes, contains the conjugated and renormalized QuaternionD.</param>
        public static void Invert(ref QuaternionD value, out QuaternionD result)
        {
            result = value;
            result.Invert();
        }

        /// <summary>
        /// Conjugates and renormalizes the QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to conjugate and renormalize.</param>
        /// <returns>The conjugated and renormalized QuaternionD.</returns>
        public static QuaternionD Invert(QuaternionD value)
        {
            QuaternionD result;
            Invert(ref value, out result);
            return result;
        }

        /// <summary>
        /// Performs a linear interpolation between two QuaternionDs.
        /// </summary>
        /// <param name="start">Start QuaternionD.</param>
        /// <param name="end">End QuaternionD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the linear interpolation of the two QuaternionDs.</param>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>start + (end - start) * amount</code>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref QuaternionD start, ref QuaternionD end, double amount, out QuaternionD result)
        {
            double inverse = 1.0 - amount;

            if (Dot(start, end) >= 0.0)
            {
                result.X = (inverse * start.X) + (amount * end.X);
                result.Y = (inverse * start.Y) + (amount * end.Y);
                result.Z = (inverse * start.Z) + (amount * end.Z);
                result.W = (inverse * start.W) + (amount * end.W);
            }
            else
            {
                result.X = (inverse * start.X) - (amount * end.X);
                result.Y = (inverse * start.Y) - (amount * end.Y);
                result.Z = (inverse * start.Z) - (amount * end.Z);
                result.W = (inverse * start.W) - (amount * end.W);
            }

            result.Normalize();
        }

        /// <summary>
        /// Performs a linear interpolation between two QuaternionD.
        /// </summary>
        /// <param name="start">Start QuaternionD.</param>
        /// <param name="end">End QuaternionD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The linear interpolation of the two QuaternionDs.</returns>
        /// <remarks>
        /// This method performs the linear interpolation based on the following formula.
        /// <code>start + (end - start) * amount</code>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static QuaternionD Lerp(QuaternionD start, QuaternionD end, double amount)
        {
            QuaternionD result;
            Lerp(ref start, ref end, amount, out result);
            return result;
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD whose logarithm will be calculated.</param>
        /// <param name="result">When the method completes, contains the natural logarithm of the QuaternionD.</param>
        public static void Logarithm(ref QuaternionD value, out QuaternionD result)
        {
            if (System.Math.Abs(value.W) < 1.0)
            {
                double angle = System.Math.Acos(value.W);
                double sin = System.Math.Sin(angle);

                if (!MathUtilD.IsZero(sin))
                {
                    double coeff = angle / sin;
                    result.X = value.X * coeff;
                    result.Y = value.Y * coeff;
                    result.Z = value.Z * coeff;
                }
                else
                {
                    result = value;
                }
            }
            else
            {
                result = value;
            }

            result.W = 0.0;
        }

        /// <summary>
        /// Calculates the natural logarithm of the specified QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD whose logarithm will be calculated.</param>
        /// <returns>The natural logarithm of the QuaternionD.</returns>
        public static QuaternionD Logarithm(QuaternionD value)
        {
            QuaternionD result;
            Logarithm(ref value, out result);
            return result;
        }

        /// <summary>
        /// Converts the QuaternionD into a unit QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to normalize.</param>
        /// <param name="result">When the method completes, contains the normalized QuaternionD.</param>
        public static void Normalize(ref QuaternionD value, out QuaternionD result)
        {
            QuaternionD temp = value;
            result = temp;
            result.Normalize();
        }

        /// <summary>
        /// Converts the QuaternionD into a unit QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to normalize.</param>
        /// <returns>The normalized QuaternionD.</returns>
        public static QuaternionD Normalize(QuaternionD value)
        {
            value.Normalize();
            return value;
        }

        /// <summary>
        /// Creates a QuaternionD given a rotation and an axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <param name="result">When the method completes, contains the newly created QuaternionD.</param>
        public static void RotationAxis(ref Vector3D axis, double angle, out QuaternionD result)
        {
            Vector3D normalized;
            Vector3D.Normalize(ref axis, out normalized);

            double half = angle * 0.5;
            double sin = System.Math.Sin(half);
            double cos = System.Math.Cos(half);

            result.X = normalized.X * sin;
            result.Y = normalized.Y * sin;
            result.Z = normalized.Z * sin;
            result.W = cos;
        }

        /// <summary>
        /// Creates a QuaternionD given a rotation and an axis.
        /// </summary>
        /// <param name="axis">The axis of rotation.</param>
        /// <param name="angle">The angle of rotation.</param>
        /// <returns>The newly created QuaternionD.</returns>
        public static QuaternionD RotationAxis(Vector3D axis, double angle)
        {
            QuaternionD result;
            RotationAxis(ref axis, angle, out result);
            return result;
        }

        /// <summary>
        /// Creates a QuaternionD given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <param name="result">When the method completes, contains the newly created QuaternionD.</param>
        public static void RotationMatrix(ref MatrixD matrix, out QuaternionD result)
        {
            double sqrt;
            double half;
            double scale = matrix.M11 + matrix.M22 + matrix.M33;

            if (scale > 0.0)
            {
                sqrt = System.Math.Sqrt(scale + 1.0);
                result.W = sqrt * 0.5;
                sqrt = 0.5 / sqrt;

                result.X = (matrix.M23 - matrix.M32) * sqrt;
                result.Y = (matrix.M31 - matrix.M13) * sqrt;
                result.Z = (matrix.M12 - matrix.M21) * sqrt;
            }
            else if ((matrix.M11 >= matrix.M22) && (matrix.M11 >= matrix.M33))
            {
                sqrt = System.Math.Sqrt(1.0 + matrix.M11 - matrix.M22 - matrix.M33);
                half = 0.5 / sqrt;

                result.X = 0.5 * sqrt;
                result.Y = (matrix.M12 + matrix.M21) * half;
                result.Z = (matrix.M13 + matrix.M31) * half;
                result.W = (matrix.M23 - matrix.M32) * half;
            }
            else if (matrix.M22 > matrix.M33)
            {
                sqrt = System.Math.Sqrt(1.0 + matrix.M22 - matrix.M11 - matrix.M33);
                half = 0.5 / sqrt;

                result.X = (matrix.M21 + matrix.M12) * half;
                result.Y = 0.5 * sqrt;
                result.Z = (matrix.M32 + matrix.M23) * half;
                result.W = (matrix.M31 - matrix.M13) * half;
            }
            else
            {
                sqrt = System.Math.Sqrt(1.0 + matrix.M33 - matrix.M11 - matrix.M22);
                half = 0.5 / sqrt;

                result.X = (matrix.M31 + matrix.M13) * half;
                result.Y = (matrix.M32 + matrix.M23) * half;
                result.Z = 0.5 * sqrt;
                result.W = (matrix.M12 - matrix.M21) * half;
            }
        }

        /// <summary>
        /// Creates a QuaternionD given forward and up vectors
        /// </summary>
        /// <param name="forward">The forward vector the QuaternionD should look at</param>
        /// <param name="up">The up vector of the QuaternionD (must be perpendicular to forward vector)</param>
        /// <param name="right">The right vector of the QuaternionD (must be perpendicular to forward vector)</param>
        /// <param name="result">The newly created QuaternionD</param>
        public static void RotationLookAt(ref Vector3D forward, ref Vector3D up, ref Vector3D right, out QuaternionD result)
        {
            //normalize input
            forward.Normalize();
            up.Normalize();
            right.Normalize();
            //fill the 3x3 matrix with the bases for the system
            MatrixD m = new MatrixD();
            m.M11 = right.X;
            m.M12 = right.Y;
            m.M13 = right.Z;
            m.M21 = up.X;
            m.M22 = up.Y;
            m.M23 = up.Z;
            m.M31 = forward.X;
            m.M32 = forward.Y;
            m.M33 = forward.Z;
            //create new QuaternionD from matrix
            RotationMatrix(ref m, out result);
        }

        /// <summary>
        /// Creates a QuaternionD given forward and up vectors
        /// </summary>
        /// <param name="forward">The forward vector the QuaternionD should look at</param>
        /// <param name="up">The up vector of the QuaternionD (must be perpendicular to forward vector)</param>
        /// <param name="right">The right vector of the QuaternionD</param>
        /// <returns>The newly created QuaternionD</returns>
        public static QuaternionD RotationLookAt(Vector3D forward, Vector3D up, Vector3D right)
        {
            QuaternionD result;
            RotationLookAt(ref forward, ref up, ref right, out result);
            return result;
        }

        /// <summary>
        /// Creates a QuaternionD given left-handed forward and up vectors
        /// </summary>
        /// <param name="forward">The forward vector the QuaternionD should look at</param>
        /// <param name="up">The up vector of the QuaternionD (must be perpendicular to forward vector)</param>
        /// <param name="result">The newly created QuaternionD</param>
        public static void RotationLookAtLH(ref Vector3D forward, ref Vector3D up, out QuaternionD result)
        {
            Vector3D right;
            Vector3D.Cross(ref up, ref forward, out right);
            RotationLookAt(ref forward, ref up, ref right, out result);
        }

        /// <summary>
        /// Creates a QuaternionD given left-handed forward and up vectors
        /// </summary>
        /// <param name="forward">The forward vector the QuaternionD should look at</param>
        /// <param name="up">The up vector of the QuaternionD (must be perpendicular to forward vector)</param>
        /// <returns>The newly created QuaternionD</returns>
        public static QuaternionD RotationLookAtLH(Vector3D forward, Vector3D up)
        {
            QuaternionD result;
            RotationLookAtLH(ref forward, ref up, out result);
            return result;
        }

        /// <summary>
        /// Creates a QuaternionD given right-handed forward and up vectors
        /// </summary>
        /// <param name="forward">The forward vector the QuaternionD should look at</param>
        /// <param name="up">The up vector of the QuaternionD (must be perpendicular to forward vector)</param>
        /// <param name="result">The newly created QuaternionD</param>
        public static void RotationLookAtRH(ref Vector3D forward, ref Vector3D up, out QuaternionD result)
        {
            Vector3D right;
            Vector3D.Cross(ref forward, ref up, out right);
            RotationLookAt(ref forward, ref up, ref right, out result);
        }

        /// <summary>
        /// Creates a QuaternionD given right-handed forward and up vectors
        /// </summary>
        /// <param name="forward">The forward vector the QuaternionD should look at</param>
        /// <param name="up">The up vector of the QuaternionD (must be perpendicular to forward vector)</param>
        /// <returns>The newly created QuaternionD</returns>
        public static QuaternionD RotationLookAtRH(Vector3D forward, Vector3D up)
        {
            QuaternionD result;
            RotationLookAtRH(ref forward, ref up, out result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <param name="result">When the method completes, contains the created billboard QuaternionD.</param>
        public static void BillboardLH(ref Vector3D objectPosition, ref Vector3D cameraPosition, ref Vector3D cameraUpVector, ref Vector3D cameraForwardVector, out QuaternionD result)
        {
            Vector3D right;
            Vector3D up;
            Vector3D difference = objectPosition - cameraPosition;

            double lengthSq = difference.LengthSquared();
            if (MathUtilD.IsZero(lengthSq))
                difference = -cameraForwardVector;

            Vector3D.Cross(ref cameraUpVector, ref difference, out right);
            Vector3D.Cross(ref difference, ref right, out up);

            RotationLookAt(ref difference, ref up, ref right, out result);
        }

        /// <summary>
        /// Creates a left-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <returns>When the method completes, contains the created billboard QuaternionD.</returns>
        public static QuaternionD BillboardLH(Vector3D objectPosition, Vector3D cameraPosition, Vector3D cameraUpVector, Vector3D cameraForwardVector)
        {
            QuaternionD result;
            BillboardLH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <param name="result">When the method completes, contains the created billboard QuaternionD.</param>
        public static void BillboardRH(ref Vector3D objectPosition, ref Vector3D cameraPosition, ref Vector3D cameraUpVector, ref Vector3D cameraForwardVector, out QuaternionD result)
        {
            Vector3D right;
            Vector3D up;
            Vector3D difference = cameraPosition - objectPosition;

            double lengthSq = difference.LengthSquared();
            if (MathUtilD.IsZero(lengthSq))
                difference = cameraForwardVector;

            Vector3D.Cross(ref cameraUpVector, ref difference, out right);
            Vector3D.Cross(ref difference, ref right, out up);

            RotationLookAt(ref difference, ref up, ref right, out result);
        }

        /// <summary>
        /// Creates a left-handed spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <returns>When the method completes, contains the created billboard QuaternionD.</returns>
        public static QuaternionD BillboardRH(Vector3D objectPosition, Vector3D cameraPosition, Vector3D cameraUpVector, Vector3D cameraForwardVector)
        {
            QuaternionD result;
            BillboardRH(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out result);
            return result;
        }

        /// <summary>
        /// Creates a QuaternionD given a rotation matrix.
        /// </summary>
        /// <param name="matrix">The rotation matrix.</param>
        /// <returns>The newly created QuaternionD.</returns>
        public static QuaternionD RotationMatrix(MatrixD matrix)
        {
            QuaternionD result;
            RotationMatrix(ref matrix, out result);
            return result;
        }

        /// <summary>
        /// Creates a QuaternionD given a yaw, pitch, and roll value.
        /// </summary>
        /// <param name="yaw">The yaw of rotation.</param>
        /// <param name="pitch">The pitch of rotation.</param>
        /// <param name="roll">The roll of rotation.</param>
        /// <param name="result">When the method completes, contains the newly created QuaternionD.</param>
        public static void RotationYawPitchRoll(double yaw, double pitch, double roll, out QuaternionD result)
        {
            double halfRoll = roll * 0.5;
            double halfPitch = pitch * 0.5;
            double halfYaw = yaw * 0.5;

            double sinRoll =  System.Math.Sin(halfRoll);
            double cosRoll =  System.Math.Cos(halfRoll);
            double sinPitch = System.Math.Sin(halfPitch);
            double cosPitch = System.Math.Cos(halfPitch);
            double sinYaw =   System.Math.Sin(halfYaw);
            double cosYaw =   System.Math.Cos(halfYaw);

            result.X = (cosYaw * sinPitch * cosRoll) + (sinYaw * cosPitch * sinRoll);
            result.Y = (sinYaw * cosPitch * cosRoll) - (cosYaw * sinPitch * sinRoll);
            result.Z = (cosYaw * cosPitch * sinRoll) - (sinYaw * sinPitch * cosRoll);
            result.W = (cosYaw * cosPitch * cosRoll) + (sinYaw * sinPitch * sinRoll);
        }

        /// <summary>
        /// Creates a QuaternionD given a yaw, pitch, and roll value.
        /// </summary>
        /// <param name="yaw">The yaw of rotation.</param>
        /// <param name="pitch">The pitch of rotation.</param>
        /// <param name="roll">The roll of rotation.</param>
        /// <returns>The newly created QuaternionD.</returns>
        public static QuaternionD RotationYawPitchRoll(double yaw, double pitch, double roll)
        {
            QuaternionD result;
            RotationYawPitchRoll(yaw, pitch, roll, out result);
            return result;
        }

        /// <summary>
        /// Interpolates between two QuaternionDs, using spherical linear interpolation.
        /// </summary>
        /// <param name="start">Start QuaternionD.</param>
        /// <param name="end">End QuaternionD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the spherical linear interpolation of the two QuaternionDs.</param>
        public static void Slerp(ref QuaternionD start, ref QuaternionD end, double amount, out QuaternionD result)
        {
            double opposite;
            double inverse;
            double dot = Dot(start, end);

            if (System.Math.Abs(dot) > 1.0 - FPPrecisionHelper.D_ABSOLUTE_TOLERANCE)
            {
                inverse = 1.0 - amount;
                opposite = amount * System.Math.Sign(dot);
            }
            else
            {
                double acos = System.Math.Acos(System.Math.Abs(dot));
                double invSin = (1.0 / System.Math.Sin(acos));

                inverse = System.Math.Sin((1.0 - amount) * acos) * invSin;
                opposite = System.Math.Sin(amount * acos) * invSin * System.Math.Sign(dot);
            }

            result.X = (inverse * start.X) + (opposite * end.X);
            result.Y = (inverse * start.Y) + (opposite * end.Y);
            result.Z = (inverse * start.Z) + (opposite * end.Z);
            result.W = (inverse * start.W) + (opposite * end.W);
        }

        /// <summary>
        /// Interpolates between two QuaternionDs, using spherical linear interpolation.
        /// </summary>
        /// <param name="start">Start QuaternionD.</param>
        /// <param name="end">End QuaternionD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The spherical linear interpolation of the two QuaternionDs.</returns>
        public static QuaternionD Slerp(QuaternionD start, QuaternionD end, double amount)
        {
            QuaternionD result;
            Slerp(ref start, ref end, amount, out result);
            return result;
        }

        /// <summary>
        /// Interpolates between QuaternionDs, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source QuaternionD.</param>
        /// <param name="value2">Second source QuaternionD.</param>
        /// <param name="value3">Third source QuaternionD.</param>
        /// <param name="value4">Fourth source QuaternionD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <param name="result">When the method completes, contains the spherical quadrangle interpolation of the QuaternionDs.</param>
        public static void Squad(ref QuaternionD value1, ref QuaternionD value2, ref QuaternionD value3, ref QuaternionD value4, double amount, out QuaternionD result)
        {
            QuaternionD start, end;
            Slerp(ref value1, ref value4, amount, out start);
            Slerp(ref value2, ref value3, amount, out end);
            Slerp(ref start, ref end, 2.0 * amount * (1.0 - amount), out result);
        }

        /// <summary>
        /// Interpolates between QuaternionDs, using spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source QuaternionD.</param>
        /// <param name="value2">Second source QuaternionD.</param>
        /// <param name="value3">Third source QuaternionD.</param>
        /// <param name="value4">Fourth source QuaternionD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of interpolation.</param>
        /// <returns>The spherical quadrangle interpolation of the QuaternionDs.</returns>
        public static QuaternionD Squad(QuaternionD value1, QuaternionD value2, QuaternionD value3, QuaternionD value4, double amount)
        {
            QuaternionD result;
            Squad(ref value1, ref value2, ref value3, ref value4, amount, out result);
            return result;
        }

        /// <summary>
        /// Sets up control points for spherical quadrangle interpolation.
        /// </summary>
        /// <param name="value1">First source QuaternionD.</param>
        /// <param name="value2">Second source QuaternionD.</param>
        /// <param name="value3">Third source QuaternionD.</param>
        /// <param name="value4">Fourth source QuaternionD.</param>
        /// <returns>An array of three QuaternionDs that represent control points for spherical quadrangle interpolation.</returns>
        public static QuaternionD[] SquadSetup(QuaternionD value1, QuaternionD value2, QuaternionD value3, QuaternionD value4)
        {
            QuaternionD q0 = (value1 + value2).LengthSquared() < (value1 - value2).LengthSquared() ? -value1 : value1;
            QuaternionD q2 = (value2 + value3).LengthSquared() < (value2 - value3).LengthSquared() ? -value3 : value3;
            QuaternionD q3 = (value3 + value4).LengthSquared() < (value3 - value4).LengthSquared() ? -value4 : value4;
            QuaternionD q1 = value2;

            QuaternionD q1Exp, q2Exp;
            Exponential(ref q1, out q1Exp);
            Exponential(ref q2, out q2Exp);

            QuaternionD[] results = new QuaternionD[3];
            results[0] = q1 * Exponential(-0.25f * (Logarithm(q1Exp * q2) + Logarithm(q1Exp * q0)));
            results[1] = q2 * Exponential(-0.25f * (Logarithm(q2Exp * q3) + Logarithm(q2Exp * q1)));
            results[2] = q2;

            return results;
        }

        /// <summary>
        /// Adds two QuaternionDs.
        /// </summary>
        /// <param name="left">The first QuaternionD to add.</param>
        /// <param name="right">The second QuaternionD to add.</param>
        /// <returns>The sum of the two QuaternionDs.</returns>
        public static QuaternionD operator +(QuaternionD left, QuaternionD right)
        {
            QuaternionD result;
            Add(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Subtracts two QuaternionDs.
        /// </summary>
        /// <param name="left">The first QuaternionD to subtract.</param>
        /// <param name="right">The second QuaternionD to subtract.</param>
        /// <returns>The difference of the two QuaternionDs.</returns>
        public static QuaternionD operator -(QuaternionD left, QuaternionD right)
        {
            QuaternionD result;
            Subtract(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Reverses the direction of a given QuaternionD.
        /// </summary>
        /// <param name="value">The QuaternionD to negate.</param>
        /// <returns>A QuaternionD facing in the opposite direction.</returns>
        public static QuaternionD operator -(QuaternionD value)
        {
            QuaternionD result;
            Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Scales a QuaternionD by the given value.
        /// </summary>
        /// <param name="value">The QuaternionD to scale.</param>
        /// <param name="scale">The amount by which to scale the QuaternionD.</param>
        /// <returns>The scaled QuaternionD.</returns>
        public static QuaternionD operator *(double scale, QuaternionD value)
        {
            QuaternionD result;
            Multiply(ref value, scale, out result);
            return result;
        }

        /// <summary>
        /// Scales a QuaternionD by the given value.
        /// </summary>
        /// <param name="value">The QuaternionD to scale.</param>
        /// <param name="scale">The amount by which to scale the QuaternionD.</param>
        /// <returns>The scaled QuaternionD.</returns>
        public static QuaternionD operator *(QuaternionD value, double scale)
        {
            QuaternionD result;
            Multiply(ref value, scale, out result);
            return result;
        }

        /// <summary>
        /// Multiplies a QuaternionD by another.
        /// </summary>
        /// <param name="left">The first QuaternionD to multiply.</param>
        /// <param name="right">The second QuaternionD to multiply.</param>
        /// <returns>The multiplied QuaternionD.</returns>
        public static QuaternionD operator *(QuaternionD left, QuaternionD right)
        {
            QuaternionD result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(QuaternionD left, QuaternionD right)
        {
            return left.Equals(ref right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(QuaternionD left, QuaternionD right)
        {
            return !left.Equals(ref right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format)
        {
            if (format == null)
                return ToString();

            return string.Format(CultureInfo.CurrentCulture, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, CultureInfo.CurrentCulture),
                Y.ToString(format, CultureInfo.CurrentCulture), Z.ToString(format, CultureInfo.CurrentCulture), W.ToString(format, CultureInfo.CurrentCulture));
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(IFormatProvider formatProvider)
        {
            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X, Y, Z, W);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="formatProvider">The format provider.</param>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (format == null)
                return ToString(formatProvider);

            return string.Format(formatProvider, "X:{0} Y:{1} Z:{2} W:{3}", X.ToString(format, formatProvider),
                Y.ToString(format, formatProvider), Z.ToString(format, formatProvider), W.ToString(format, formatProvider));
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = X.GetHashCode();
                hashCode = (hashCode * 397) ^ Y.GetHashCode();
                hashCode = (hashCode * 397) ^ Z.GetHashCode();
                hashCode = (hashCode * 397) ^ W.GetHashCode();
                return hashCode;
            }
        }

        /// <inheritdoc/>
        void IDataSerializable.Serialize(BinarySerializer serializer)
        {
            // Write optimized version without using Serialize methods
            if (serializer.Mode == SerializerMode.Write)
            {
                serializer.Writer.Write(X);
                serializer.Writer.Write(Y);
                serializer.Writer.Write(Z);
                serializer.Writer.Write(W);
            }
            else
            {
                X = serializer.Reader.ReadSingle();
                Y = serializer.Reader.ReadSingle();
                Z = serializer.Reader.ReadSingle();
                W = serializer.Reader.ReadSingle();
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="SharpDX.QuaternionD"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SharpDX.QuaternionD"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="SharpDX.QuaternionD"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ref QuaternionD other)
        {
            return FPPrecisionHelper.Equal(other.X, X) && FPPrecisionHelper.Equal(other.Y, Y) && FPPrecisionHelper.Equal(other.Z, Z) && FPPrecisionHelper.Equal(other.W, W);
        }

        /// <summary>
        /// Determines whether the specified <see cref="SharpDX.QuaternionD"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SharpDX.QuaternionD"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="SharpDX.QuaternionD"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(QuaternionD other)
        {
            return Equals(ref other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="value">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object value)
        {
            if (!(value is QuaternionD))
                return false;

            var strongValue = (QuaternionD)value;
            return Equals(ref strongValue);
        }

    }
}
