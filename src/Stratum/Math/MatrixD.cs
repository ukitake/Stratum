using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SharpDX;

namespace Stratum
{
    public struct MatrixD
    {
        /// <summary>
        /// The size of the <see cref="SharpDX.MatrixD"/> type, in bytes.
        /// </summary>
        public static readonly int SizeInBytes = Marshal.SizeOf(typeof(MatrixD));

        /// <summary>
        /// A <see cref="SharpDX.MatrixD"/> with all of its components set to zero.
        /// </summary>
        public static readonly MatrixD Zero = new MatrixD();

        /// <summary>
        /// The identity <see cref="SharpDX.MatrixD"/>.
        /// </summary>
        public static readonly MatrixD Identity = new MatrixD() { M11 = 1.0d, M22 = 1.0d, M33 = 1.0d, M44 = 1.0d };

        /// <summary>
        /// Value at row 1 column 1 of the MatrixD.
        /// </summary>
        public double M11;

        /// <summary>
        /// Value at row 1 column 2 of the MatrixD.
        /// </summary>
        public double M12;

        /// <summary>
        /// Value at row 1 column 3 of the MatrixD.
        /// </summary>
        public double M13;

        /// <summary>
        /// Value at row 1 column 4 of the MatrixD.
        /// </summary>
        public double M14;

        /// <summary>
        /// Value at row 2 column 1 of the MatrixD.
        /// </summary>
        public double M21;

        /// <summary>
        /// Value at row 2 column 2 of the MatrixD.
        /// </summary>
        public double M22;

        /// <summary>
        /// Value at row 2 column 3 of the MatrixD.
        /// </summary>
        public double M23;

        /// <summary>
        /// Value at row 2 column 4 of the MatrixD.
        /// </summary>
        public double M24;

        /// <summary>
        /// Value at row 3 column 1 of the MatrixD.
        /// </summary>
        public double M31;

        /// <summary>
        /// Value at row 3 column 2 of the MatrixD.
        /// </summary>
        public double M32;

        /// <summary>
        /// Value at row 3 column 3 of the MatrixD.
        /// </summary>
        public double M33;

        /// <summary>
        /// Value at row 3 column 4 of the MatrixD.
        /// </summary>
        public double M34;

        /// <summary>
        /// Value at row 4 column 1 of the MatrixD.
        /// </summary>
        public double M41;

        /// <summary>
        /// Value at row 4 column 2 of the MatrixD.
        /// </summary>
        public double M42;

        /// <summary>
        /// Value at row 4 column 3 of the MatrixD.
        /// </summary>
        public double M43;

        /// <summary>
        /// Value at row 4 column 4 of the MatrixD.
        /// </summary>
        public double M44;
     
        /// <summary>
        /// Gets or sets the up <see cref="SharpDX.Vector3"/> of the MatrixD; that is M21, M22, and M23.
        /// </summary>
        public Vector3D Up
        {
          get
          {
            Vector3D vector3;
            vector3.X = this.M21;
            vector3.Y = this.M22;
            vector3.Z = this.M23;
            return vector3;
          }
          set
          {
            this.M21 = value.X;
            this.M22 = value.Y;
            this.M23 = value.Z;
          }
        }
    
        /// <summary>
        /// Gets or sets the down <see cref="SharpDX.Vector3"/> of the MatrixD; that is -M21, -M22, and -M23.
        /// </summary>
        public Vector3D Down
        {
          get
          {
            Vector3D vector3;
            vector3.X = -this.M21;
            vector3.Y = -this.M22;
            vector3.Z = -this.M23;
            return vector3;
          }
          set
          {
            this.M21 = -value.X;
            this.M22 = -value.Y;
            this.M23 = -value.Z;
          }
        }
    
        /// <summary>
        /// Gets or sets the right <see cref="SharpDX.Vector3"/> of the MatrixD; that is M11, M12, and M13.
        /// </summary>
        public Vector3D Right
        {
          get
          {
            Vector3D vector3;
            vector3.X = this.M11;
            vector3.Y = this.M12;
            vector3.Z = this.M13;
            return vector3;
          }
          set
          {
            this.M11 = value.X;
            this.M12 = value.Y;
            this.M13 = value.Z;
          }
        }
    
        /// <summary>
        /// Gets or sets the left <see cref="SharpDX.Vector3"/> of the MatrixD; that is -M11, -M12, and -M13.
        /// </summary>
        public Vector3D Left
        {
          get
          {
            Vector3D vector3;
            vector3.X = -this.M11;
            vector3.Y = -this.M12;
            vector3.Z = -this.M13;
            return vector3;
          }
          set
          {
            this.M11 = -value.X;
            this.M12 = -value.Y;
            this.M13 = -value.Z;
          }
        }
        
        /// <summary>
        /// Gets or sets the forward <see cref="SharpDX.Vector3"/> of the MatrixD; that is -M31, -M32, and -M33.
        /// </summary>
        public Vector3D Forward
        {
          get
          {
            Vector3D vector3;
            vector3.X = -this.M31;
            vector3.Y = -this.M32;
            vector3.Z = -this.M33;
            return vector3;
          }
          set
          {
            this.M31 = -value.X;
            this.M32 = -value.Y;
            this.M33 = -value.Z;
          }
        }
        
        /// <summary>
        /// Gets or sets the backward <see cref="SharpDX.Vector3"/> of the MatrixD; that is M31, M32, and M33.
        /// </summary>
        public Vector3D Backward
        {
          get
          {
            Vector3D vector3;
            vector3.X = this.M31;
            vector3.Y = this.M32;
            vector3.Z = this.M33;
            return vector3;
          }
          set
          {
            this.M31 = value.X;
            this.M32 = value.Y;
            this.M33 = value.Z;
          }
        }

        public MatrixD(Matrix m)
        {
            this.M11 = m.M11; this.M12 = m.M12; this.M13 = m.M13; this.M14 = m.M14;
            this.M21 = m.M21; this.M22 = m.M22; this.M23 = m.M23; this.M24 = m.M24;
            this.M31 = m.M31; this.M32 = m.M32; this.M33 = m.M33; this.M34 = m.M34;
            this.M41 = m.M41; this.M42 = m.M42; this.M43 = m.M43; this.M44 = m.M44;
        }
        
        /// <summary>
        /// Initializes a new instance of the struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public MatrixD(double value)
        {
            M11 = M12 = M13 = M14 =
            M21 = M22 = M23 = M24 =
            M31 = M32 = M33 = M34 =
            M41 = M42 = M43 = M44 = value;
        }

        /// <summary>
        /// Initializes a new instance of the struct.
        /// </summary>
        /// <param name="M11">The value to assign at row 1 column 1 of the MatrixD.</param>
        /// <param name="M12">The value to assign at row 1 column 2 of the MatrixD.</param>
        /// <param name="M13">The value to assign at row 1 column 3 of the MatrixD.</param>
        /// <param name="M14">The value to assign at row 1 column 4 of the MatrixD.</param>
        /// <param name="M21">The value to assign at row 2 column 1 of the MatrixD.</param>
        /// <param name="M22">The value to assign at row 2 column 2 of the MatrixD.</param>
        /// <param name="M23">The value to assign at row 2 column 3 of the MatrixD.</param>
        /// <param name="M24">The value to assign at row 2 column 4 of the MatrixD.</param>
        /// <param name="M31">The value to assign at row 3 column 1 of the MatrixD.</param>
        /// <param name="M32">The value to assign at row 3 column 2 of the MatrixD.</param>
        /// <param name="M33">The value to assign at row 3 column 3 of the MatrixD.</param>
        /// <param name="M34">The value to assign at row 3 column 4 of the MatrixD.</param>
        /// <param name="M41">The value to assign at row 4 column 1 of the MatrixD.</param>
        /// <param name="M42">The value to assign at row 4 column 2 of the MatrixD.</param>
        /// <param name="M43">The value to assign at row 4 column 3 of the MatrixD.</param>
        /// <param name="M44">The value to assign at row 4 column 4 of the MatrixD.</param>
        public MatrixD(double M11, double M12, double M13, double M14,
            double M21, double M22, double M23, double M24,
            double M31, double M32, double M33, double M34,
            double M41, double M42, double M43, double M44)
        {
            this.M11 = M11; this.M12 = M12; this.M13 = M13; this.M14 = M14;
            this.M21 = M21; this.M22 = M22; this.M23 = M23; this.M24 = M24;
            this.M31 = M31; this.M32 = M32; this.M33 = M33; this.M34 = M34;
            this.M41 = M41; this.M42 = M42; this.M43 = M43; this.M44 = M44;
        }

        /// <summary>
        /// Initializes a new instance of the struct.
        /// </summary>
        /// <param name="values">The values to assign to the components of the MatrixD. This must be an array with sixteen elements.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="values"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="values"/> contains more or less than sixteen elements.</exception>
        public MatrixD(double[] values)
        {
            if (values == null)
                throw new ArgumentNullException("values");
            if (values.Length != 16)
                throw new ArgumentOutOfRangeException("values", "There must be sixteen and only sixteen input values for MatrixD.");

            M11 = values[0];
            M12 = values[1];
            M13 = values[2];
            M14 = values[3];

            M21 = values[4];
            M22 = values[5];
            M23 = values[6];
            M24 = values[7];

            M31 = values[8];
            M32 = values[9];
            M33 = values[10];
            M34 = values[11];

            M41 = values[12];
            M42 = values[13];
            M43 = values[14];
            M44 = values[15];
        }

        /// <summary>
        /// Gets or sets the first row in the MatrixD; that is M11, M12, M13, and M14.
        /// </summary>
        public Vector4D Row1
        {
            get { return new Vector4D(M11, M12, M13, M14); }
            set { M11 = value.X; M12 = value.Y; M13 = value.Z; M14 = value.W; }
        }

        /// <summary>
        /// Gets or sets the second row in the MatrixD; that is M21, M22, M23, and M24.
        /// </summary>
        public Vector4D Row2
        {
            get { return new Vector4D(M21, M22, M23, M24); }
            set { M21 = value.X; M22 = value.Y; M23 = value.Z; M24 = value.W; }
        }

        /// <summary>
        /// Gets or sets the third row in the MatrixD; that is M31, M32, M33, and M34.
        /// </summary>
        public Vector4D Row3
        {
            get { return new Vector4D(M31, M32, M33, M34); }
            set { M31 = value.X; M32 = value.Y; M33 = value.Z; M34 = value.W; }
        }

        /// <summary>
        /// Gets or sets the fourth row in the MatrixD; that is M41, M42, M43, and M44.
        /// </summary>
        public Vector4D Row4
        {
            get { return new Vector4D(M41, M42, M43, M44); }
            set { M41 = value.X; M42 = value.Y; M43 = value.Z; M44 = value.W; }
        }

        /// <summary>
        /// Gets or sets the first column in the MatrixD; that is M11, M21, M31, and M41.
        /// </summary>
        public Vector4D Column1
        {
            get { return new Vector4D(M11, M21, M31, M41); }
            set { M11 = value.X; M21 = value.Y; M31 = value.Z; M41 = value.W; }
        }

        /// <summary>
        /// Gets or sets the second column in the MatrixD; that is M12, M22, M32, and M42.
        /// </summary>
        public Vector4D Column2
        {
            get { return new Vector4D(M12, M22, M32, M42); }
            set { M12 = value.X; M22 = value.Y; M32 = value.Z; M42 = value.W; }
        }

        /// <summary>
        /// Gets or sets the third column in the MatrixD; that is M13, M23, M33, and M43.
        /// </summary>
        public Vector4D Column3
        {
            get { return new Vector4D(M13, M23, M33, M43); }
            set { M13 = value.X; M23 = value.Y; M33 = value.Z; M43 = value.W; }
        }

        /// <summary>
        /// Gets or sets the fourth column in the MatrixD; that is M14, M24, M34, and M44.
        /// </summary>
        public Vector4D Column4
        {
            get { return new Vector4D(M14, M24, M34, M44); }
            set { M14 = value.X; M24 = value.Y; M34 = value.Z; M44 = value.W; }
        }

        /// <summary>
        /// Gets or sets the translation of the MatrixD; that is M41, M42, and M43.
        /// </summary>
        public Vector3D TranslationVector
        {
            get { return new Vector3D(M41, M42, M43); }
            set { M41 = value.X; M42 = value.Y; M43 = value.Z; }
        }

        /// <summary>
        /// Gets or sets the scale of the MatrixD; that is M11, M22, and M33.
        /// </summary>
        public Vector3D ScaleVector
        {
            get { return new Vector3D(M11, M22, M33); }
            set { M11 = value.X; M22 = value.Y; M33 = value.Z; }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is an identity MatrixD.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is an identity MatrixD; otherwise, <c>false</c>.
        /// </value>
        public bool IsIdentity
        {
            get { return this.Equals(Identity); }
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the MatrixD component, depending on the index.</value>
        /// <param name="index">The zero-based index of the component to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="index"/> is out of the range [0, 15].</exception>
        public double this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0:  return M11;
                    case 1:  return M12;
                    case 2:  return M13;
                    case 3:  return M14;
                    case 4:  return M21;
                    case 5:  return M22;
                    case 6:  return M23;
                    case 7:  return M24;
                    case 8:  return M31;
                    case 9:  return M32;
                    case 10: return M33;
                    case 11: return M34;
                    case 12: return M41;
                    case 13: return M42;
                    case 14: return M43;
                    case 15: return M44;
                }

                throw new ArgumentOutOfRangeException("index", "Indices for MatrixD run from 0 to 15, inclusive.");
            }

            set
            {
                switch (index)
                {
                    case 0: M11 = value; break;
                    case 1: M12 = value; break;
                    case 2: M13 = value; break;
                    case 3: M14 = value; break;
                    case 4: M21 = value; break;
                    case 5: M22 = value; break;
                    case 6: M23 = value; break;
                    case 7: M24 = value; break;
                    case 8: M31 = value; break;
                    case 9: M32 = value; break;
                    case 10: M33 = value; break;
                    case 11: M34 = value; break;
                    case 12: M41 = value; break;
                    case 13: M42 = value; break;
                    case 14: M43 = value; break;
                    case 15: M44 = value; break;
                    default: throw new ArgumentOutOfRangeException("index", "Indices for MatrixD run from 0 to 15, inclusive.");
                }
            }
        }

        /// <summary>
        /// Gets or sets the component at the specified index.
        /// </summary>
        /// <value>The value of the MatrixD component, depending on the index.</value>
        /// <param name="row">The row of the MatrixD to access.</param>
        /// <param name="column">The column of the MatrixD to access.</param>
        /// <returns>The value of the component at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="row"/> or <paramref name="column"/>is out of the range [0, 3].</exception>
        public double this[int row, int column]
        {
            get
            {
                if (row < 0 || row > 3)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 3, inclusive.");
                if (column < 0 || column > 3)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 3, inclusive.");

                return this[(row * 4) + column];
            }

            set
            {
                if (row < 0 || row > 3)
                    throw new ArgumentOutOfRangeException("row", "Rows and columns for matrices run from 0 to 3, inclusive.");
                if (column < 0 || column > 3)
                    throw new ArgumentOutOfRangeException("column", "Rows and columns for matrices run from 0 to 3, inclusive.");

                this[(row * 4) + column] = value;
            }
        }

        /// <summary>
        /// Calculates the determinant of the MatrixD.
        /// </summary>
        /// <returns>The determinant of the MatrixD.</returns>
        public double Determinant()
        {
            double temp1 = (M33 * M44) - (M34 * M43);
            double temp2 = (M32 * M44) - (M34 * M42);
            double temp3 = (M32 * M43) - (M33 * M42);
            double temp4 = (M31 * M44) - (M34 * M41);
            double temp5 = (M31 * M43) - (M33 * M41);
            double temp6 = (M31 * M42) - (M32 * M41);

            return ((((M11 * (((M22 * temp1) - (M23 * temp2)) + (M24 * temp3))) - (M12 * (((M21 * temp1) -
                (M23 * temp4)) + (M24 * temp5)))) + (M13 * (((M21 * temp2) - (M22 * temp4)) + (M24 * temp6)))) -
                (M14 * (((M21 * temp3) - (M22 * temp5)) + (M23 * temp6))));
        }

        /// <summary>
        /// Inverts the MatrixD.
        /// </summary>
        public void Invert()
        {
            Invert(ref this, out this);
        }

        /// <summary>
        /// Transposes the MatrixD.
        /// </summary>
        public void Transpose()
        {
            Transpose(ref this, out this);
        }

        /// <summary>
        /// Orthogonalizes the specified MatrixD.
        /// </summary>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the MatrixD will be orthogonal to any other given row in the
        /// MatrixD.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting MatrixD
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the MatrixD rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public void Orthogonalize()
        {
            Orthogonalize(ref this, out this);
        }

        /// <summary>
        /// Orthonormalizes the specified MatrixD.
        /// </summary>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting MatrixD
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the MatrixD rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public void Orthonormalize()
        {
            Orthonormalize(ref this, out this);
        }

        /// <summary>
        /// Decomposes a MatrixD into an orthonormalized MatrixD Q and a right triangular MatrixD R.
        /// </summary>
        /// <param name="Q">When the method completes, contains the orthonormalized MatrixD of the decomposition.</param>
        /// <param name="R">When the method completes, contains the right triangular MatrixD of the decomposition.</param>
        public void DecomposeQR(out MatrixD Q, out MatrixD R)
        {
            MatrixD temp = this;
            temp.Transpose();
            Orthonormalize(ref temp, out Q);
            Q.Transpose();

            R = new MatrixD();
            R.M11 = Vector4D.Dot(Q.Column1, Column1);
            R.M12 = Vector4D.Dot(Q.Column1, Column2);
            R.M13 = Vector4D.Dot(Q.Column1, Column3);
            R.M14 = Vector4D.Dot(Q.Column1, Column4);

            R.M22 = Vector4D.Dot(Q.Column2, Column2);
            R.M23 = Vector4D.Dot(Q.Column2, Column3);
            R.M24 = Vector4D.Dot(Q.Column2, Column4);

            R.M33 = Vector4D.Dot(Q.Column3, Column3);
            R.M34 = Vector4D.Dot(Q.Column3, Column4);

            R.M44 = Vector4D.Dot(Q.Column4, Column4);
        }

        /// <summary>
        /// Decomposes a MatrixD into a lower triangular MatrixD L and an orthonormalized MatrixD Q.
        /// </summary>
        /// <param name="L">When the method completes, contains the lower triangular MatrixD of the decomposition.</param>
        /// <param name="Q">When the method completes, contains the orthonormalized MatrixD of the decomposition.</param>
        public void DecomposeLQ(out MatrixD L, out MatrixD Q)
        {
            Orthonormalize(ref this, out Q);

            L = new MatrixD();
            L.M11 = Vector4D.Dot(Q.Row1, Row1);
            
            L.M21 = Vector4D.Dot(Q.Row1, Row2);
            L.M22 = Vector4D.Dot(Q.Row2, Row2);
            
            L.M31 = Vector4D.Dot(Q.Row1, Row3);
            L.M32 = Vector4D.Dot(Q.Row2, Row3);
            L.M33 = Vector4D.Dot(Q.Row3, Row3);
            
            L.M41 = Vector4D.Dot(Q.Row1, Row4);
            L.M42 = Vector4D.Dot(Q.Row2, Row4);
            L.M43 = Vector4D.Dot(Q.Row3, Row4);
            L.M44 = Vector4D.Dot(Q.Row4, Row4);
        }

        /// <summary>
        /// Exchanges two rows in the MatrixD.
        /// </summary>
        /// <param name="firstRow">The first row to exchange. This is an index of the row starting at zero.</param>
        /// <param name="secondRow">The second row to exchange. This is an index of the row starting at zero.</param>
        public void ExchangeRows(int firstRow, int secondRow)
        {
            if (firstRow < 0)
                throw new ArgumentOutOfRangeException("firstRow", "The parameter firstRow must be greater than or equal to zero.");
            if (firstRow > 3)
                throw new ArgumentOutOfRangeException("firstRow", "The parameter firstRow must be less than or equal to three.");
            if (secondRow < 0)
                throw new ArgumentOutOfRangeException("secondRow", "The parameter secondRow must be greater than or equal to zero.");
            if (secondRow > 3)
                throw new ArgumentOutOfRangeException("secondRow", "The parameter secondRow must be less than or equal to three.");

            if (firstRow == secondRow)
                return;

            double temp0 = this[secondRow, 0];
            double temp1 = this[secondRow, 1];
            double temp2 = this[secondRow, 2];
            double temp3 = this[secondRow, 3];

            this[secondRow, 0] = this[firstRow, 0];
            this[secondRow, 1] = this[firstRow, 1];
            this[secondRow, 2] = this[firstRow, 2];
            this[secondRow, 3] = this[firstRow, 3];

            this[firstRow, 0] = temp0;
            this[firstRow, 1] = temp1;
            this[firstRow, 2] = temp2;
            this[firstRow, 3] = temp3;
        }

        /// <summary>
        /// Exchanges two columns in the MatrixD.
        /// </summary>
        /// <param name="firstColumn">The first column to exchange. This is an index of the column starting at zero.</param>
        /// <param name="secondColumn">The second column to exchange. This is an index of the column starting at zero.</param>
        public void ExchangeColumns(int firstColumn, int secondColumn)
        {
            if (firstColumn < 0)
                throw new ArgumentOutOfRangeException("firstColumn", "The parameter firstColumn must be greater than or equal to zero.");
            if (firstColumn > 3)
                throw new ArgumentOutOfRangeException("firstColumn", "The parameter firstColumn must be less than or equal to three.");
            if (secondColumn < 0)
                throw new ArgumentOutOfRangeException("secondColumn", "The parameter secondColumn must be greater than or equal to zero.");
            if (secondColumn > 3)
                throw new ArgumentOutOfRangeException("secondColumn", "The parameter secondColumn must be less than or equal to three.");

            if (firstColumn == secondColumn)
                return;

            double temp0 = this[0, secondColumn];
            double temp1 = this[1, secondColumn];
            double temp2 = this[2, secondColumn];
            double temp3 = this[3, secondColumn];

            this[0, secondColumn] = this[0, firstColumn];
            this[1, secondColumn] = this[1, firstColumn];
            this[2, secondColumn] = this[2, firstColumn];
            this[3, secondColumn] = this[3, firstColumn];

            this[0, firstColumn] = temp0;
            this[1, firstColumn] = temp1;
            this[2, firstColumn] = temp2;
            this[3, firstColumn] = temp3;
        }

        /// <summary>
        /// Creates an array containing the elements of the MatrixD.
        /// </summary>
        /// <returns>A sixteen-element array containing the components of the MatrixD.</returns>
        public double[] ToArray()
        {
            return new[] { M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44 };
        }

        /// <summary>
        /// Determines the sum of two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to add.</param>
        /// <param name="right">The second MatrixD to add.</param>
        /// <param name="result">When the method completes, contains the sum of the two matrices.</param>
        public static void Add(ref MatrixD left, ref MatrixD right, out MatrixD result)
        {
            result.M11 = left.M11 + right.M11;
            result.M12 = left.M12 + right.M12;
            result.M13 = left.M13 + right.M13;
            result.M14 = left.M14 + right.M14;
            result.M21 = left.M21 + right.M21;
            result.M22 = left.M22 + right.M22;
            result.M23 = left.M23 + right.M23;
            result.M24 = left.M24 + right.M24;
            result.M31 = left.M31 + right.M31;
            result.M32 = left.M32 + right.M32;
            result.M33 = left.M33 + right.M33;
            result.M34 = left.M34 + right.M34;
            result.M41 = left.M41 + right.M41;
            result.M42 = left.M42 + right.M42;
            result.M43 = left.M43 + right.M43;
            result.M44 = left.M44 + right.M44;
        }

        /// <summary>
        /// Determines the sum of two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to add.</param>
        /// <param name="right">The second MatrixD to add.</param>
        /// <returns>The sum of the two matrices.</returns>
        public static MatrixD Add(MatrixD left, MatrixD right)
        {
            MatrixD result;
            Add(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Determines the difference between two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to subtract.</param>
        /// <param name="right">The second MatrixD to subtract.</param>
        /// <param name="result">When the method completes, contains the difference between the two matrices.</param>
        public static void Subtract(ref MatrixD left, ref MatrixD right, out MatrixD result)
        {
            result.M11 = left.M11 - right.M11;
            result.M12 = left.M12 - right.M12;
            result.M13 = left.M13 - right.M13;
            result.M14 = left.M14 - right.M14;
            result.M21 = left.M21 - right.M21;
            result.M22 = left.M22 - right.M22;
            result.M23 = left.M23 - right.M23;
            result.M24 = left.M24 - right.M24;
            result.M31 = left.M31 - right.M31;
            result.M32 = left.M32 - right.M32;
            result.M33 = left.M33 - right.M33;
            result.M34 = left.M34 - right.M34;
            result.M41 = left.M41 - right.M41;
            result.M42 = left.M42 - right.M42;
            result.M43 = left.M43 - right.M43;
            result.M44 = left.M44 - right.M44;
        }

        /// <summary>
        /// Determines the difference between two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to subtract.</param>
        /// <param name="right">The second MatrixD to subtract.</param>
        /// <returns>The difference between the two matrices.</returns>
        public static MatrixD Subtract(MatrixD left, MatrixD right)
        {
            MatrixD result;
            Subtract(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Scales a MatrixD by the given value.
        /// </summary>
        /// <param name="left">The MatrixD to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <param name="result">When the method completes, contains the scaled MatrixD.</param>
        public static void Multiply(ref MatrixD left, double right, out MatrixD result)
        {
            result.M11 = left.M11 * right;
            result.M12 = left.M12 * right;
            result.M13 = left.M13 * right;
            result.M14 = left.M14 * right;
            result.M21 = left.M21 * right;
            result.M22 = left.M22 * right;
            result.M23 = left.M23 * right;
            result.M24 = left.M24 * right;
            result.M31 = left.M31 * right;
            result.M32 = left.M32 * right;
            result.M33 = left.M33 * right;
            result.M34 = left.M34 * right;
            result.M41 = left.M41 * right;
            result.M42 = left.M42 * right;
            result.M43 = left.M43 * right;
            result.M44 = left.M44 * right;
        }

        /// <summary>
        /// Scales a MatrixD by the given value.
        /// </summary>
        /// <param name="left">The MatrixD to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled MatrixD.</returns>
        public static MatrixD Multiply(MatrixD left, double right)
        {
            MatrixD result;
            Multiply(ref left, right, out result);
            return result;
        }

        /// <summary>
        /// Determines the product of two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to multiply.</param>
        /// <param name="right">The second MatrixD to multiply.</param>
        /// <param name="result">The product of the two matrices.</param>
        public static void Multiply(ref MatrixD left, ref MatrixD right, out MatrixD result)
        {
            MatrixD temp = new MatrixD();
            temp.M11 = (left.M11 * right.M11) + (left.M12 * right.M21) + (left.M13 * right.M31) + (left.M14 * right.M41);
            temp.M12 = (left.M11 * right.M12) + (left.M12 * right.M22) + (left.M13 * right.M32) + (left.M14 * right.M42);
            temp.M13 = (left.M11 * right.M13) + (left.M12 * right.M23) + (left.M13 * right.M33) + (left.M14 * right.M43);
            temp.M14 = (left.M11 * right.M14) + (left.M12 * right.M24) + (left.M13 * right.M34) + (left.M14 * right.M44);
            temp.M21 = (left.M21 * right.M11) + (left.M22 * right.M21) + (left.M23 * right.M31) + (left.M24 * right.M41);
            temp.M22 = (left.M21 * right.M12) + (left.M22 * right.M22) + (left.M23 * right.M32) + (left.M24 * right.M42);
            temp.M23 = (left.M21 * right.M13) + (left.M22 * right.M23) + (left.M23 * right.M33) + (left.M24 * right.M43);
            temp.M24 = (left.M21 * right.M14) + (left.M22 * right.M24) + (left.M23 * right.M34) + (left.M24 * right.M44);
            temp.M31 = (left.M31 * right.M11) + (left.M32 * right.M21) + (left.M33 * right.M31) + (left.M34 * right.M41);
            temp.M32 = (left.M31 * right.M12) + (left.M32 * right.M22) + (left.M33 * right.M32) + (left.M34 * right.M42);
            temp.M33 = (left.M31 * right.M13) + (left.M32 * right.M23) + (left.M33 * right.M33) + (left.M34 * right.M43);
            temp.M34 = (left.M31 * right.M14) + (left.M32 * right.M24) + (left.M33 * right.M34) + (left.M34 * right.M44);
            temp.M41 = (left.M41 * right.M11) + (left.M42 * right.M21) + (left.M43 * right.M31) + (left.M44 * right.M41);
            temp.M42 = (left.M41 * right.M12) + (left.M42 * right.M22) + (left.M43 * right.M32) + (left.M44 * right.M42);
            temp.M43 = (left.M41 * right.M13) + (left.M42 * right.M23) + (left.M43 * right.M33) + (left.M44 * right.M43);
            temp.M44 = (left.M41 * right.M14) + (left.M42 * right.M24) + (left.M43 * right.M34) + (left.M44 * right.M44);
            result = temp;
        }

        /// <summary>
        /// Determines the product of two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to multiply.</param>
        /// <param name="right">The second MatrixD to multiply.</param>
        /// <returns>The product of the two matrices.</returns>
        public static MatrixD Multiply(MatrixD left, MatrixD right)
        {
            MatrixD result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Scales a MatrixD by the given value.
        /// </summary>
        /// <param name="left">The MatrixD to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <param name="result">When the method completes, contains the scaled MatrixD.</param>
        public static void Divide(ref MatrixD left, double right, out MatrixD result)
        {
            double inv = 1.0f / right;

            result.M11 = left.M11 * inv;
            result.M12 = left.M12 * inv;
            result.M13 = left.M13 * inv;
            result.M14 = left.M14 * inv;
            result.M21 = left.M21 * inv;
            result.M22 = left.M22 * inv;
            result.M23 = left.M23 * inv;
            result.M24 = left.M24 * inv;
            result.M31 = left.M31 * inv;
            result.M32 = left.M32 * inv;
            result.M33 = left.M33 * inv;
            result.M34 = left.M34 * inv;
            result.M41 = left.M41 * inv;
            result.M42 = left.M42 * inv;
            result.M43 = left.M43 * inv;
            result.M44 = left.M44 * inv;
        }

        /// <summary>
        /// Scales a MatrixD by the given value.
        /// </summary>
        /// <param name="left">The MatrixD to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled MatrixD.</returns>
        public static MatrixD Divide(MatrixD left, double right)
        {
            MatrixD result;
            Divide(ref left, right, out result);
            return result;
        }

        /// <summary>
        /// Determines the quotient of two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to divide.</param>
        /// <param name="right">The second MatrixD to divide.</param>
        /// <param name="result">When the method completes, contains the quotient of the two matrices.</param>
        public static void Divide(ref MatrixD left, ref MatrixD right, out MatrixD result)
        {
            result.M11 = left.M11 / right.M11;
            result.M12 = left.M12 / right.M12;
            result.M13 = left.M13 / right.M13;
            result.M14 = left.M14 / right.M14;
            result.M21 = left.M21 / right.M21;
            result.M22 = left.M22 / right.M22;
            result.M23 = left.M23 / right.M23;
            result.M24 = left.M24 / right.M24;
            result.M31 = left.M31 / right.M31;
            result.M32 = left.M32 / right.M32;
            result.M33 = left.M33 / right.M33;
            result.M34 = left.M34 / right.M34;
            result.M41 = left.M41 / right.M41;
            result.M42 = left.M42 / right.M42;
            result.M43 = left.M43 / right.M43;
            result.M44 = left.M44 / right.M44;
        }

        /// <summary>
        /// Determines the quotient of two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to divide.</param>
        /// <param name="right">The second MatrixD to divide.</param>
        /// <returns>The quotient of the two matrices.</returns>
        public static MatrixD Divide(MatrixD left, MatrixD right)
        {
            MatrixD result;
            Divide(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Performs the exponential operation on a MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to perform the operation on.</param>
        /// <param name="exponent">The exponent to raise the MatrixD to.</param>
        /// <param name="result">When the method completes, contains the exponential MatrixD.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="exponent"/> is negative.</exception>
        public static void Exponent(ref MatrixD value, int exponent, out MatrixD result)
        {
            //Source: http://rosettacode.org
            //Reference: http://rosettacode.org/wiki/MatrixD-exponentiation_operator

            if (exponent < 0)
                throw new ArgumentOutOfRangeException("exponent", "The exponent can not be negative.");

            if (exponent == 0)
            {
                result = MatrixD.Identity;
                return;
            }

            if (exponent == 1)
            {
                result = value;
                return;
            }

            MatrixD identity = MatrixD.Identity;
            MatrixD temp = value;

            while (true)
            {
                if ((exponent & 1) != 0)
                    identity = identity * temp;

                exponent /= 2;

                if (exponent > 0)
                    temp *= temp;
                else
                    break;
            }

            result = identity;
        }

        /// <summary>
        /// Performs the exponential operation on a MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to perform the operation on.</param>
        /// <param name="exponent">The exponent to raise the MatrixD to.</param>
        /// <returns>The exponential MatrixD.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">Thrown when the <paramref name="exponent"/> is negative.</exception>
        public static MatrixD Exponent(MatrixD value, int exponent)
        {
            MatrixD result;
            Exponent(ref value, exponent, out result);
            return result;
        }

        /// <summary>
        /// Negates a MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to be negated.</param>
        /// <param name="result">When the method completes, contains the negated MatrixD.</param>
        public static void Negate(ref MatrixD value, out MatrixD result)
        {
            result.M11 = -value.M11;
            result.M12 = -value.M12;
            result.M13 = -value.M13;
            result.M14 = -value.M14;
            result.M21 = -value.M21;
            result.M22 = -value.M22;
            result.M23 = -value.M23;
            result.M24 = -value.M24;
            result.M31 = -value.M31;
            result.M32 = -value.M32;
            result.M33 = -value.M33;
            result.M34 = -value.M34;
            result.M41 = -value.M41;
            result.M42 = -value.M42;
            result.M43 = -value.M43;
            result.M44 = -value.M44;
        }

        /// <summary>
        /// Negates a MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to be negated.</param>
        /// <returns>The negated MatrixD.</returns>
        public static MatrixD Negate(MatrixD value)
        {
            MatrixD result;
            Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Performs a linear interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start MatrixD.</param>
        /// <param name="end">End MatrixD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the linear interpolation of the two matrices.</param>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static void Lerp(ref MatrixD start, ref MatrixD end, double amount, out MatrixD result)
        {
            result.M11 = MathUtil.Lerp(start.M11, end.M11, amount);
            result.M12 = MathUtil.Lerp(start.M12, end.M12, amount);
            result.M13 = MathUtil.Lerp(start.M13, end.M13, amount);
            result.M14 = MathUtil.Lerp(start.M14, end.M14, amount);
            result.M21 = MathUtil.Lerp(start.M21, end.M21, amount);
            result.M22 = MathUtil.Lerp(start.M22, end.M22, amount);
            result.M23 = MathUtil.Lerp(start.M23, end.M23, amount);
            result.M24 = MathUtil.Lerp(start.M24, end.M24, amount);
            result.M31 = MathUtil.Lerp(start.M31, end.M31, amount);
            result.M32 = MathUtil.Lerp(start.M32, end.M32, amount);
            result.M33 = MathUtil.Lerp(start.M33, end.M33, amount);
            result.M34 = MathUtil.Lerp(start.M34, end.M34, amount);
            result.M41 = MathUtil.Lerp(start.M41, end.M41, amount);
            result.M42 = MathUtil.Lerp(start.M42, end.M42, amount);
            result.M43 = MathUtil.Lerp(start.M43, end.M43, amount);
            result.M44 = MathUtil.Lerp(start.M44, end.M44, amount);
        }

        /// <summary>
        /// Performs a linear interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start MatrixD.</param>
        /// <param name="end">End MatrixD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The linear interpolation of the two matrices.</returns>
        /// <remarks>
        /// Passing <paramref name="amount"/> a value of 0 will cause <paramref name="start"/> to be returned; a value of 1 will cause <paramref name="end"/> to be returned. 
        /// </remarks>
        public static MatrixD Lerp(MatrixD start, MatrixD end, double amount)
        {
            MatrixD result;
            Lerp(ref start, ref end, amount, out result);
            return result;
        }

        /// <summary>
        /// Performs a cubic interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start MatrixD.</param>
        /// <param name="end">End MatrixD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <param name="result">When the method completes, contains the cubic interpolation of the two matrices.</param>
        public static void SmoothStep(ref MatrixD start, ref MatrixD end, double amount, out MatrixD result)
        {
            amount = MathUtilD.SmoothStep(amount);
            Lerp(ref start, ref end, amount, out result);
        }

        /// <summary>
        /// Performs a cubic interpolation between two matrices.
        /// </summary>
        /// <param name="start">Start MatrixD.</param>
        /// <param name="end">End MatrixD.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of <paramref name="end"/>.</param>
        /// <returns>The cubic interpolation of the two matrices.</returns>
        public static MatrixD SmoothStep(MatrixD start, MatrixD end, double amount)
        {
            MatrixD result;
            SmoothStep(ref start, ref end, amount, out result);
            return result;
        }

        /// <summary>
        /// Calculates the transpose of the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD whose transpose is to be calculated.</param>
        /// <param name="result">When the method completes, contains the transpose of the specified MatrixD.</param>
        public static void Transpose(ref MatrixD value, out MatrixD result)
        {
            MatrixD temp = new MatrixD();
            temp.M11 = value.M11;
            temp.M12 = value.M21;
            temp.M13 = value.M31;
            temp.M14 = value.M41;
            temp.M21 = value.M12;
            temp.M22 = value.M22;
            temp.M23 = value.M32;
            temp.M24 = value.M42;
            temp.M31 = value.M13;
            temp.M32 = value.M23;
            temp.M33 = value.M33;
            temp.M34 = value.M43;
            temp.M41 = value.M14;
            temp.M42 = value.M24;
            temp.M43 = value.M34;
            temp.M44 = value.M44;

            result = temp;
        }

        /// <summary>
        /// Calculates the transpose of the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD whose transpose is to be calculated.</param>
        /// <param name="result">When the method completes, contains the transpose of the specified MatrixD.</param>
        public static void TransposeByRef(ref MatrixD value, ref MatrixD result)
        {
            result.M11 = value.M11;
            result.M12 = value.M21;
            result.M13 = value.M31;
            result.M14 = value.M41;
            result.M21 = value.M12;
            result.M22 = value.M22;
            result.M23 = value.M32;
            result.M24 = value.M42;
            result.M31 = value.M13;
            result.M32 = value.M23;
            result.M33 = value.M33;
            result.M34 = value.M43;
            result.M41 = value.M14;
            result.M42 = value.M24;
            result.M43 = value.M34;
            result.M44 = value.M44;
        }

        /// <summary>
        /// Calculates the transpose of the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD whose transpose is to be calculated.</param>
        /// <returns>The transpose of the specified MatrixD.</returns>
        public static MatrixD Transpose(MatrixD value)
        {
            MatrixD result;
            Transpose(ref value, out result);
            return result;
        }

        /// <summary>
        /// Calculates the inverse of the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD whose inverse is to be calculated.</param>
        /// <param name="result">When the method completes, contains the inverse of the specified MatrixD.</param>
        public static void Invert(ref MatrixD value, out MatrixD result)
        {
            double b0 = (value.M31 * value.M42) - (value.M32 * value.M41);
            double b1 = (value.M31 * value.M43) - (value.M33 * value.M41);
            double b2 = (value.M34 * value.M41) - (value.M31 * value.M44);
            double b3 = (value.M32 * value.M43) - (value.M33 * value.M42);
            double b4 = (value.M34 * value.M42) - (value.M32 * value.M44);
            double b5 = (value.M33 * value.M44) - (value.M34 * value.M43);

            double d11 = value.M22 * b5 + value.M23 * b4 + value.M24 * b3;
            double d12 = value.M21 * b5 + value.M23 * b2 + value.M24 * b1;
            double d13 = value.M21 * -b4 + value.M22 * b2 + value.M24 * b0;
            double d14 = value.M21 * b3 + value.M22 * -b1 + value.M23 * b0;

            double det = value.M11 * d11 - value.M12 * d12 + value.M13 * d13 - value.M14 * d14;
            if (System.Math.Abs(det) == 0.0f)
            {
                result = MatrixD.Zero;
                return;
            }

            det = 1f / det;

            double a0 = (value.M11 * value.M22) - (value.M12 * value.M21);
            double a1 = (value.M11 * value.M23) - (value.M13 * value.M21);
            double a2 = (value.M14 * value.M21) - (value.M11 * value.M24);
            double a3 = (value.M12 * value.M23) - (value.M13 * value.M22);
            double a4 = (value.M14 * value.M22) - (value.M12 * value.M24);
            double a5 = (value.M13 * value.M24) - (value.M14 * value.M23);

            double d21 = value.M12 * b5 + value.M13 * b4 + value.M14 * b3;
            double d22 = value.M11 * b5 + value.M13 * b2 + value.M14 * b1;
            double d23 = value.M11 * -b4 + value.M12 * b2 + value.M14 * b0;
            double d24 = value.M11 * b3 + value.M12 * -b1 + value.M13 * b0;
            
            double d31 = value.M42 * a5 + value.M43 * a4 + value.M44 * a3;
            double d32 = value.M41 * a5 + value.M43 * a2 + value.M44 * a1;
            double d33 = value.M41 * -a4 + value.M42 * a2 + value.M44 * a0;
            double d34 = value.M41 * a3 + value.M42 * -a1 + value.M43 * a0;
            
            double d41 = value.M32 * a5 + value.M33 * a4 + value.M34 * a3;
            double d42 = value.M31 * a5 + value.M33 * a2 + value.M34 * a1;
            double d43 = value.M31 * -a4 + value.M32 * a2 + value.M34 * a0;
            double d44 = value.M31 * a3 + value.M32 * -a1 + value.M33 * a0;

            result.M11 = +d11 * det; result.M12 = -d21 * det; result.M13 = +d31 * det; result.M14 = -d41 * det;
            result.M21 = -d12 * det; result.M22 = +d22 * det; result.M23 = -d32 * det; result.M24 = +d42 * det;
            result.M31 = +d13 * det; result.M32 = -d23 * det; result.M33 = +d33 * det; result.M34 = -d43 * det;
            result.M41 = -d14 * det; result.M42 = +d24 * det; result.M43 = -d34 * det; result.M44 = +d44 * det;
        }

        /// <summary>
        /// Calculates the inverse of the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD whose inverse is to be calculated.</param>
        /// <returns>The inverse of the specified MatrixD.</returns>
        public static MatrixD Invert(MatrixD value)
        {
            value.Invert();
            return value;
        }

        /// <summary>
        /// Orthogonalizes the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to orthogonalize.</param>
        /// <param name="result">When the method completes, contains the orthogonalized MatrixD.</param>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the MatrixD will be orthogonal to any other given row in the
        /// MatrixD.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting MatrixD
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the MatrixD rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static void Orthogonalize(ref MatrixD value, out MatrixD result)
        {
            //Uses the modified Gram-Schmidt process.
            //q1 = m1
            //q2 = m2 - ((q1 ⋅ m2) / (q1 ⋅ q1)) * q1
            //q3 = m3 - ((q1 ⋅ m3) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m3) / (q2 ⋅ q2)) * q2
            //q4 = m4 - ((q1 ⋅ m4) / (q1 ⋅ q1)) * q1 - ((q2 ⋅ m4) / (q2 ⋅ q2)) * q2 - ((q3 ⋅ m4) / (q3 ⋅ q3)) * q3

            //By separating the above algorithm into multiple lines, we actually increase accuracy.
            result = value;

            result.Row2 = result.Row2 - (Vector4D.Dot(result.Row1, result.Row2) / Vector4D.Dot(result.Row1, result.Row1)) * result.Row1;

            result.Row3 = result.Row3 - (Vector4D.Dot(result.Row1, result.Row3) / Vector4D.Dot(result.Row1, result.Row1)) * result.Row1;
            result.Row3 = result.Row3 - (Vector4D.Dot(result.Row2, result.Row3) / Vector4D.Dot(result.Row2, result.Row2)) * result.Row2;

            result.Row4 = result.Row4 - (Vector4D.Dot(result.Row1, result.Row4) / Vector4D.Dot(result.Row1, result.Row1)) * result.Row1;
            result.Row4 = result.Row4 - (Vector4D.Dot(result.Row2, result.Row4) / Vector4D.Dot(result.Row2, result.Row2)) * result.Row2;
            result.Row4 = result.Row4 - (Vector4D.Dot(result.Row3, result.Row4) / Vector4D.Dot(result.Row3, result.Row3)) * result.Row3;
        }

        /// <summary>
        /// Orthogonalizes the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to orthogonalize.</param>
        /// <returns>The orthogonalized MatrixD.</returns>
        /// <remarks>
        /// <para>Orthogonalization is the process of making all rows orthogonal to each other. This
        /// means that any given row in the MatrixD will be orthogonal to any other given row in the
        /// MatrixD.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting MatrixD
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the MatrixD rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static MatrixD Orthogonalize(MatrixD value)
        {
            MatrixD result;
            Orthogonalize(ref value, out result);
            return result;
        }

        /// <summary>
        /// Orthonormalizes the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to orthonormalize.</param>
        /// <param name="result">When the method completes, contains the orthonormalized MatrixD.</param>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting MatrixD
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the MatrixD rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static void Orthonormalize(ref MatrixD value, out MatrixD result)
        {
            //Uses the modified Gram-Schmidt process.
            //Because we are making unit vectors, we can optimize the math for orthonormalization
            //and simplify the projection operation to remove the division.
            //q1 = m1 / |m1|
            //q2 = (m2 - (q1 ⋅ m2) * q1) / |m2 - (q1 ⋅ m2) * q1|
            //q3 = (m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2) / |m3 - (q1 ⋅ m3) * q1 - (q2 ⋅ m3) * q2|
            //q4 = (m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3) / |m4 - (q1 ⋅ m4) * q1 - (q2 ⋅ m4) * q2 - (q3 ⋅ m4) * q3|

            //By separating the above algorithm into multiple lines, we actually increase accuracy.
            result = value;

            result.Row1 = Vector4D.Normalize(result.Row1);

            result.Row2 = result.Row2 - Vector4D.Dot(result.Row1, result.Row2) * result.Row1;
            result.Row2 = Vector4D.Normalize(result.Row2);

            result.Row3 = result.Row3 - Vector4D.Dot(result.Row1, result.Row3) * result.Row1;
            result.Row3 = result.Row3 - Vector4D.Dot(result.Row2, result.Row3) * result.Row2;
            result.Row3 = Vector4D.Normalize(result.Row3);

            result.Row4 = result.Row4 - Vector4D.Dot(result.Row1, result.Row4) * result.Row1;
            result.Row4 = result.Row4 - Vector4D.Dot(result.Row2, result.Row4) * result.Row2;
            result.Row4 = result.Row4 - Vector4D.Dot(result.Row3, result.Row4) * result.Row3;
            result.Row4 = Vector4D.Normalize(result.Row4);
        }

        /// <summary>
        /// Orthonormalizes the specified MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to orthonormalize.</param>
        /// <returns>The orthonormalized MatrixD.</returns>
        /// <remarks>
        /// <para>Orthonormalization is the process of making all rows and columns orthogonal to each
        /// other and making all rows and columns of unit length. This means that any given row will
        /// be orthogonal to any other given row and any given column will be orthogonal to any other
        /// given column. Any given row will not be orthogonal to any given column. Every row and every
        /// column will be of unit length.</para>
        /// <para>Because this method uses the modified Gram-Schmidt process, the resulting MatrixD
        /// tends to be numerically unstable. The numeric stability decreases according to the rows
        /// so that the first row is the most stable and the last row is the least stable.</para>
        /// <para>This operation is performed on the rows of the MatrixD rather than the columns.
        /// If you wish for this operation to be performed on the columns, first transpose the
        /// input and than transpose the output.</para>
        /// </remarks>
        public static MatrixD Orthonormalize(MatrixD value)
        {
            MatrixD result;
            Orthonormalize(ref value, out result);
            return result;
        }

        /// <summary>
        /// Brings the MatrixD into upper triangular form using elementary row operations.
        /// </summary>
        /// <param name="value">The MatrixD to put into upper triangular form.</param>
        /// <param name="result">When the method completes, contains the upper triangular MatrixD.</param>
        /// <remarks>
        /// If the MatrixD is not invertible (i.e. its determinant is zero) than the result of this
        /// method may produce Single.Nan and Single.Inf values. When the MatrixD represents a system
        /// of linear equations, than this often means that either no solution exists or an infinite
        /// number of solutions exist.
        /// </remarks>
        public static void UpperTriangularForm(ref MatrixD value, out MatrixD result)
        {
            //Adapted from the row echelon code.
            result = value;
            int lead = 0;
            int rowcount = 4;
            int columncount = 4;

            for (int r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                    return;

                int i = r;

                while (MathUtilD.IsZero(result[i, lead]))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                            return;
                    }
                }

                if (i != r)
                {
                    result.ExchangeRows(i, r);
                }

                double multiplier = 1d / result[r, lead];

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        result[i, 0] -= result[r, 0] * multiplier * result[i, lead];
                        result[i, 1] -= result[r, 1] * multiplier * result[i, lead];
                        result[i, 2] -= result[r, 2] * multiplier * result[i, lead];
                        result[i, 3] -= result[r, 3] * multiplier * result[i, lead];
                    }
                }

                lead++;
            }
        }

        /// <summary>
        /// Brings the MatrixD into upper triangular form using elementary row operations.
        /// </summary>
        /// <param name="value">The MatrixD to put into upper triangular form.</param>
        /// <returns>The upper triangular MatrixD.</returns>
        /// <remarks>
        /// If the MatrixD is not invertible (i.e. its determinant is zero) than the result of this
        /// method may produce Single.Nan and Single.Inf values. When the MatrixD represents a system
        /// of linear equations, than this often means that either no solution exists or an infinite
        /// number of solutions exist.
        /// </remarks>
        public static MatrixD UpperTriangularForm(MatrixD value)
        {
            MatrixD result;
            UpperTriangularForm(ref value, out result);
            return result;
        }

        /// <summary>
        /// Brings the MatrixD into lower triangular form using elementary row operations.
        /// </summary>
        /// <param name="value">The MatrixD to put into lower triangular form.</param>
        /// <param name="result">When the method completes, contains the lower triangular MatrixD.</param>
        /// <remarks>
        /// If the MatrixD is not invertible (i.e. its determinant is zero) than the result of this
        /// method may produce Single.Nan and Single.Inf values. When the MatrixD represents a system
        /// of linear equations, than this often means that either no solution exists or an infinite
        /// number of solutions exist.
        /// </remarks>
        public static void LowerTriangularForm(ref MatrixD value, out MatrixD result)
        {
            //Adapted from the row echelon code.
            MatrixD temp = value;
            MatrixD.Transpose(ref temp, out result);

            int lead = 0;
            int rowcount = 4;
            int columncount = 4;

            for (int r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                    return;

                int i = r;

                while (MathUtilD.IsZero(result[i, lead]))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                            return;
                    }
                }

                if (i != r)
                {
                    result.ExchangeRows(i, r);
                }

                double multiplier = 1d / result[r, lead];

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        result[i, 0] -= result[r, 0] * multiplier * result[i, lead];
                        result[i, 1] -= result[r, 1] * multiplier * result[i, lead];
                        result[i, 2] -= result[r, 2] * multiplier * result[i, lead];
                        result[i, 3] -= result[r, 3] * multiplier * result[i, lead];
                    }
                }

                lead++;
            }

            MatrixD.Transpose(ref result, out result);
        }

        /// <summary>
        /// Brings the MatrixD into lower triangular form using elementary row operations.
        /// </summary>
        /// <param name="value">The MatrixD to put into lower triangular form.</param>
        /// <returns>The lower triangular MatrixD.</returns>
        /// <remarks>
        /// If the MatrixD is not invertible (i.e. its determinant is zero) than the result of this
        /// method may produce Single.Nan and Single.Inf values. When the MatrixD represents a system
        /// of linear equations, than this often means that either no solution exists or an infinite
        /// number of solutions exist.
        /// </remarks>
        public static MatrixD LowerTriangularForm(MatrixD value)
        {
            MatrixD result;
            LowerTriangularForm(ref value, out result);
            return result;
        }

        /// <summary>
        /// Brings the MatrixD into row echelon form using elementary row operations;
        /// </summary>
        /// <param name="value">The MatrixD to put into row echelon form.</param>
        /// <param name="result">When the method completes, contains the row echelon form of the MatrixD.</param>
        public static void RowEchelonForm(ref MatrixD value, out MatrixD result)
        {
            //Source: Wikipedia pseudo code
            //Reference: http://en.wikipedia.org/wiki/Row_echelon_form#Pseudocode

            result = value;
            int lead = 0;
            int rowcount = 4;
            int columncount = 4;

            for (int r = 0; r < rowcount; ++r)
            {
                if (columncount <= lead)
                    return;

                int i = r;

                while (MathUtilD.IsZero(result[i, lead]))
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (lead == columncount)
                            return;
                    }
                }

                if (i != r)
                {
                    result.ExchangeRows(i, r);
                }

                double multiplier = 1d / result[r, lead];
                result[r, 0] *= multiplier;
                result[r, 1] *= multiplier;
                result[r, 2] *= multiplier;
                result[r, 3] *= multiplier;

                for (; i < rowcount; ++i)
                {
                    if (i != r)
                    {
                        result[i, 0] -= result[r, 0] * result[i, lead];
                        result[i, 1] -= result[r, 1] * result[i, lead];
                        result[i, 2] -= result[r, 2] * result[i, lead];
                        result[i, 3] -= result[r, 3] * result[i, lead];
                    }
                }

                lead++;
            }
        }

        /// <summary>
        /// Brings the MatrixD into row echelon form using elementary row operations;
        /// </summary>
        /// <param name="value">The MatrixD to put into row echelon form.</param>
        /// <returns>When the method completes, contains the row echelon form of the MatrixD.</returns>
        public static MatrixD RowEchelonForm(MatrixD value)
        {
            MatrixD result;
            RowEchelonForm(ref value, out result);
            return result;
        }

        /// <summary>
        /// Brings the MatrixD into reduced row echelon form using elementary row operations.
        /// </summary>
        /// <param name="value">The MatrixD to put into reduced row echelon form.</param>
        /// <param name="augment">The fifth column of the MatrixD.</param>
        /// <param name="result">When the method completes, contains the resultant MatrixD after the operation.</param>
        /// <param name="augmentResult">When the method completes, contains the resultant fifth column of the MatrixD.</param>
        /// <remarks>
        /// <para>The fifth column is often called the augmented part of the MatrixD. This is because the fifth
        /// column is really just an extension of the MatrixD so that there is a place to put all of the
        /// non-zero components after the operation is complete.</para>
        /// <para>Often times the resultant MatrixD will the identity MatrixD or a MatrixD similar to the identity
        /// MatrixD. Sometimes, however, that is not possible and numbers other than zero and one may appear.</para>
        /// <para>This method can be used to solve systems of linear equations. Upon completion of this method,
        /// the <paramref name="augmentResult"/> will contain the solution for the system. It is up to the user
        /// to analyze both the input and the result to determine if a solution really exists.</para>
        /// </remarks>
        public static void ReducedRowEchelonForm(ref MatrixD value, ref Vector4D augment, out MatrixD result, out Vector4D augmentResult)
        {
            //Source: http://rosettacode.org
            //Reference: http://rosettacode.org/wiki/Reduced_row_echelon_form

            double[,] MatrixD = new double[4, 5];

            MatrixD[0, 0] = value[0, 0];
            MatrixD[0, 1] = value[0, 1];
            MatrixD[0, 2] = value[0, 2];
            MatrixD[0, 3] = value[0, 3];
            MatrixD[0, 4] = augment[0];

            MatrixD[1, 0] = value[1, 0];
            MatrixD[1, 1] = value[1, 1];
            MatrixD[1, 2] = value[1, 2];
            MatrixD[1, 3] = value[1, 3];
            MatrixD[1, 4] = augment[1];

            MatrixD[2, 0] = value[2, 0];
            MatrixD[2, 1] = value[2, 1];
            MatrixD[2, 2] = value[2, 2];
            MatrixD[2, 3] = value[2, 3];
            MatrixD[2, 4] = augment[2];

            MatrixD[3, 0] = value[3, 0];
            MatrixD[3, 1] = value[3, 1];
            MatrixD[3, 2] = value[3, 2];
            MatrixD[3, 3] = value[3, 3];
            MatrixD[3, 4] = augment[3];

            int lead = 0;
            int rowcount = 4;
            int columncount = 5;

            for (int r = 0; r < rowcount; r++)
            {
                if (columncount <= lead)
                    break;

                int i = r;

                while (MatrixD[i, lead] == 0)
                {
                    i++;

                    if (i == rowcount)
                    {
                        i = r;
                        lead++;

                        if (columncount == lead)
                            break;
                    }
                }

                for (int j = 0; j < columncount; j++)
                {
                    double temp = MatrixD[r, j];
                    MatrixD[r, j] = MatrixD[i, j];
                    MatrixD[i, j] = temp;
                }

                double div = MatrixD[r, lead];

                for (int j = 0; j < columncount; j++)
                {
                    MatrixD[r, j] /= div;
                }

                for (int j = 0; j < rowcount; j++)
                {
                    if (j != r)
                    {
                        double sub = MatrixD[j, lead];
                        for (int k = 0; k < columncount; k++) MatrixD[j, k] -= (sub * MatrixD[r, k]);
                    }
                }

                lead++;
            }

            result.M11 = MatrixD[0, 0];
            result.M12 = MatrixD[0, 1];
            result.M13 = MatrixD[0, 2];
            result.M14 = MatrixD[0, 3];

            result.M21 = MatrixD[1, 0];
            result.M22 = MatrixD[1, 1];
            result.M23 = MatrixD[1, 2];
            result.M24 = MatrixD[1, 3];

            result.M31 = MatrixD[2, 0];
            result.M32 = MatrixD[2, 1];
            result.M33 = MatrixD[2, 2];
            result.M34 = MatrixD[2, 3];

            result.M41 = MatrixD[3, 0];
            result.M42 = MatrixD[3, 1];
            result.M43 = MatrixD[3, 2];
            result.M44 = MatrixD[3, 3];

            augmentResult.X = MatrixD[0, 4];
            augmentResult.Y = MatrixD[1, 4];
            augmentResult.Z = MatrixD[2, 4];
            augmentResult.W = MatrixD[3, 4];
        }

        /// <summary>
        /// Creates a spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <param name="result">When the method completes, contains the created billboard MatrixD.</param>
        public static void Billboard(ref Vector3D objectPosition, ref Vector3D cameraPosition, ref Vector3D cameraUpVector, ref Vector3D cameraForwardVector, out MatrixD result)
        {
            Vector3D crossed;
            Vector3D final;
            Vector3D difference = objectPosition - cameraPosition;

            double lengthSq = difference.LengthSquared();
            if (MathUtilD.IsZero(lengthSq))
                difference = -cameraForwardVector;
            else
                difference *= (double)(1.0 / System.Math.Sqrt(lengthSq));

            Vector3D.Cross(ref cameraUpVector, ref difference, out crossed);
            crossed.Normalize();
            Vector3D.Cross(ref difference, ref crossed, out final);

            result.M11 = crossed.X;
            result.M12 = crossed.Y;
            result.M13 = crossed.Z;
            result.M14 = 0.0d;
            result.M21 = final.X;
            result.M22 = final.Y;
            result.M23 = final.Z;
            result.M24 = 0.0d;
            result.M31 = difference.X;
            result.M32 = difference.Y;
            result.M33 = difference.Z;
            result.M34 = 0.0d;
            result.M41 = objectPosition.X;
            result.M42 = objectPosition.Y;
            result.M43 = objectPosition.Z;
            result.M44 = 1.0d;
        }

        /// <summary>
        /// Creates a spherical billboard that rotates around a specified object position.
        /// </summary>
        /// <param name="objectPosition">The position of the object around which the billboard will rotate.</param>
        /// <param name="cameraPosition">The position of the camera.</param>
        /// <param name="cameraUpVector">The up vector of the camera.</param>
        /// <param name="cameraForwardVector">The forward vector of the camera.</param>
        /// <returns>The created billboard MatrixD.</returns>
        public static MatrixD Billboard(Vector3D objectPosition, Vector3D cameraPosition, Vector3D cameraUpVector, Vector3D cameraForwardVector)
        {
            MatrixD result;
            Billboard(ref objectPosition, ref cameraPosition, ref cameraUpVector, ref cameraForwardVector, out result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed, look-at MatrixD.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at MatrixD.</param>
        public static void LookAtLH(ref Vector3D eye, ref Vector3D target, ref Vector3D up, out MatrixD result)
        {
            Vector3D xaxis, yaxis, zaxis;
            Vector3D.Subtract(ref target, ref eye, out zaxis); zaxis.Normalize();
            Vector3D.Cross(ref up, ref zaxis, out xaxis); xaxis.Normalize();
            Vector3D.Cross(ref zaxis, ref xaxis, out yaxis);

            result = MatrixD.Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;

            Vector3D.Dot(ref xaxis, ref eye, out result.M41);
            Vector3D.Dot(ref yaxis, ref eye, out result.M42);
            Vector3D.Dot(ref zaxis, ref eye, out result.M43);

            result.M41 = -result.M41;
            result.M42 = -result.M42;
            result.M43 = -result.M43;
        }

        /// <summary>
        /// Creates a left-handed, look-at MatrixD.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <returns>The created look-at MatrixD.</returns>
        public static MatrixD LookAtLH(Vector3D eye, Vector3D target, Vector3D up)
        {
            MatrixD result;
            LookAtLH(ref eye, ref target, ref up, out result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, look-at MatrixD.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <param name="result">When the method completes, contains the created look-at MatrixD.</param>
        public static void LookAtRH(ref Vector3D eye, ref Vector3D target, ref Vector3D up, out MatrixD result)
        {
            Vector3D xaxis, yaxis, zaxis;
            Vector3D.Subtract(ref eye, ref target, out zaxis); zaxis.Normalize();
            Vector3D.Cross(ref up, ref zaxis, out xaxis); xaxis.Normalize();
            Vector3D.Cross(ref zaxis, ref xaxis, out yaxis);

            result = MatrixD.Identity;
            result.M11 = xaxis.X; result.M21 = xaxis.Y; result.M31 = xaxis.Z;
            result.M12 = yaxis.X; result.M22 = yaxis.Y; result.M32 = yaxis.Z;
            result.M13 = zaxis.X; result.M23 = zaxis.Y; result.M33 = zaxis.Z;

            Vector3D.Dot(ref xaxis, ref eye, out result.M41);
            Vector3D.Dot(ref yaxis, ref eye, out result.M42);
            Vector3D.Dot(ref zaxis, ref eye, out result.M43);

            result.M41 = -result.M41;
            result.M42 = -result.M42;
            result.M43 = -result.M43;
        }

        /// <summary>
        /// Creates a right-handed, look-at MatrixD.
        /// </summary>
        /// <param name="eye">The position of the viewer's eye.</param>
        /// <param name="target">The camera look-at target.</param>
        /// <param name="up">The camera's up vector.</param>
        /// <returns>The created look-at MatrixD.</returns>
        public static MatrixD LookAtRH(Vector3D eye, Vector3D target, Vector3D up)
        {
            MatrixD result;
            LookAtRH(ref eye, ref target, ref up, out result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed, orthographic projection MatrixD.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void OrthoLH(double width, double height, double znear, double zfar, out MatrixD result)
        {
            double halfWidth = width * 0.5d;
            double halfHeight = height * 0.5d;

            OrthoOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a left-handed, orthographic projection MatrixD.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD OrthoLH(double width, double height, double znear, double zfar)
        {
            MatrixD result;
            OrthoLH(width, height, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, orthographic projection MatrixD.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void OrthoRH(double width, double height, double znear, double zfar, out MatrixD result)
        {
            double halfWidth = width * 0.5d;
            double halfHeight = height * 0.5d;

            OrthoOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a right-handed, orthographic projection MatrixD.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD OrthoRH(double width, double height, double znear, double zfar)
        {
            MatrixD result;
            OrthoRH(width, height, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed, customized orthographic projection MatrixD.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void OrthoOffCenterLH(double left, double right, double bottom, double top, double znear, double zfar, out MatrixD result)
        {
            double zRange = 1.0d / (zfar - znear);

            result = MatrixD.Identity;
            result.M11 = 2.0d / (right - left);
            result.M22 = 2.0d / (top - bottom);
            result.M33 = zRange;
            result.M41 = (left + right) / (left - right);
            result.M42 = (top + bottom) / (bottom - top);
            result.M43 = -znear * zRange;
        }

        /// <summary>
        /// Creates a left-handed, customized orthographic projection MatrixD.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD OrthoOffCenterLH(double left, double right, double bottom, double top, double znear, double zfar)
        {
            MatrixD result;
            OrthoOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, customized orthographic projection MatrixD.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void OrthoOffCenterRH(double left, double right, double bottom, double top, double znear, double zfar, out MatrixD result)
        {
            OrthoOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            result.M33 *= -1.0d;
        }

        /// <summary>
        /// Creates a right-handed, customized orthographic projection MatrixD.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD OrthoOffCenterRH(double left, double right, double bottom, double top, double znear, double zfar)
        {
            MatrixD result;
            OrthoOffCenterRH(left, right, bottom, top, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed, perspective projection MatrixD.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void PerspectiveLH(double width, double height, double znear, double zfar, out MatrixD result)
        {
            double halfWidth = width * 0.5d;
            double halfHeight = height * 0.5d;

            PerspectiveOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a left-handed, perspective projection MatrixD.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD PerspectiveLH(double width, double height, double znear, double zfar)
        {
            MatrixD result;
            PerspectiveLH(width, height, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, perspective projection MatrixD.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void PerspectiveRH(double width, double height, double znear, double zfar, out MatrixD result)
        {
            double halfWidth = width * 0.5d;
            double halfHeight = height * 0.5d;

            PerspectiveOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a right-handed, perspective projection MatrixD.
        /// </summary>
        /// <param name="width">Width of the viewing volume.</param>
        /// <param name="height">Height of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD PerspectiveRH(double width, double height, double znear, double zfar)
        {
            MatrixD result;
            PerspectiveRH(width, height, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed, perspective projection MatrixD based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void PerspectiveFovLH(double fov, double aspect, double znear, double zfar, out MatrixD result)
        {
            double yScale = (double)(1.0 / System.Math.Tan(fov * 0.5d));
            double xScale = yScale / aspect;

            double halfWidth = znear / xScale;
            double halfHeight = znear / yScale;

            PerspectiveOffCenterLH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a left-handed, perspective projection MatrixD based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD PerspectiveFovLH(double fov, double aspect, double znear, double zfar)
        {
            MatrixD result;
            PerspectiveFovLH(fov, aspect, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, perspective projection MatrixD based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void PerspectiveFovRH(double fov, double aspect, double znear, double zfar, out MatrixD result)
        {
            double yScale = (double)(1.0 / System.Math.Tan(fov * 0.5d));
            double xScale = yScale / aspect;

            double halfWidth = znear / xScale;
            double halfHeight = znear / yScale;

            PerspectiveOffCenterRH(-halfWidth, halfWidth, -halfHeight, halfHeight, znear, zfar, out result);
        }

        /// <summary>
        /// Creates a right-handed, perspective projection MatrixD based on a field of view.
        /// </summary>
        /// <param name="fov">Field of view in the y direction, in radians.</param>
        /// <param name="aspect">Aspect ratio, defined as view space width divided by height.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD PerspectiveFovRH(double fov, double aspect, double znear, double zfar)
        {
            MatrixD result;
            PerspectiveFovRH(fov, aspect, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a left-handed, customized perspective projection MatrixD.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void PerspectiveOffCenterLH(double left, double right, double bottom, double top, double znear, double zfar, out MatrixD result)
        {
            double zRange = zfar / (zfar - znear);

            result = new MatrixD();
            result.M11 = 2.0d * znear / (right - left);
            result.M22 = 2.0d * znear / (top - bottom);
            result.M31 = (left + right) / (left - right);
            result.M32 = (top + bottom) / (bottom - top);
            result.M33 = zRange;
            result.M34 = 1.0d;
            result.M43 = -znear * zRange;
        }

        /// <summary>
        /// Creates a left-handed, customized perspective projection MatrixD.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD PerspectiveOffCenterLH(double left, double right, double bottom, double top, double znear, double zfar)
        {
            MatrixD result;
            PerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Creates a right-handed, customized perspective projection MatrixD.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <param name="result">When the method completes, contains the created projection MatrixD.</param>
        public static void PerspectiveOffCenterRH(double left, double right, double bottom, double top, double znear, double zfar, out MatrixD result)
        {
            PerspectiveOffCenterLH(left, right, bottom, top, znear, zfar, out result);
            result.M31 *= -1.0d;
            result.M32 *= -1.0d;
            result.M33 *= -1.0d;
            result.M34 *= -1.0d;
        }

        /// <summary>
        /// Creates a right-handed, customized perspective projection MatrixD.
        /// </summary>
        /// <param name="left">Minimum x-value of the viewing volume.</param>
        /// <param name="right">Maximum x-value of the viewing volume.</param>
        /// <param name="bottom">Minimum y-value of the viewing volume.</param>
        /// <param name="top">Maximum y-value of the viewing volume.</param>
        /// <param name="znear">Minimum z-value of the viewing volume.</param>
        /// <param name="zfar">Maximum z-value of the viewing volume.</param>
        /// <returns>The created projection MatrixD.</returns>
        public static MatrixD PerspectiveOffCenterRH(double left, double right, double bottom, double top, double znear, double zfar)
        {
            MatrixD result;
            PerspectiveOffCenterRH(left, right, bottom, top, znear, zfar, out result);
            return result;
        }

        /// <summary>
        /// Builds a MatrixD that can be used to reflect vectors about a plane.
        /// </summary>
        /// <param name="plane">The plane for which the reflection occurs. This parameter is assumed to be normalized.</param>
        /// <param name="result">When the method completes, contains the reflection MatrixD.</param>
        public static void Reflection(ref Plane plane, out MatrixD result)
        {
            double x = plane.Normal.X;
            double y = plane.Normal.Y;
            double z = plane.Normal.Z;
            double x2 = -2.0d * x;
            double y2 = -2.0d * y;
            double z2 = -2.0d * z;

            result.M11 = (x2 * x) + 1.0d;
            result.M12 = y2 * x;
            result.M13 = z2 * x;
            result.M14 = 0.0d;
            result.M21 = x2 * y;
            result.M22 = (y2 * y) + 1.0d;
            result.M23 = z2 * y;
            result.M24 = 0.0d;
            result.M31 = x2 * z;
            result.M32 = y2 * z;
            result.M33 = (z2 * z) + 1.0d;
            result.M34 = 0.0d;
            result.M41 = x2 * plane.D;
            result.M42 = y2 * plane.D;
            result.M43 = z2 * plane.D;
            result.M44 = 1.0d;
        }

        /// <summary>
        /// Builds a MatrixD that can be used to reflect vectors about a plane.
        /// </summary>
        /// <param name="plane">The plane for which the reflection occurs. This parameter is assumed to be normalized.</param>
        /// <returns>The reflection MatrixD.</returns>
        public static MatrixD Reflection(Plane plane)
        {
            MatrixD result;
            Reflection(ref plane, out result);
            return result;
        }

        /// <summary>
        /// Creates a MatrixD that flattens geometry into a shadow.
        /// </summary>
        /// <param name="light">The light direction. If the W component is 0, the light is directional light; if the
        /// W component is 1, the light is a point light.</param>
        /// <param name="plane">The plane onto which to project the geometry as a shadow. This parameter is assumed to be normalized.</param>
        /// <param name="result">When the method completes, contains the shadow MatrixD.</param>
        public static void Shadow(ref Vector4D light, ref Plane plane, out MatrixD result)
        {        
            double dot = (plane.Normal.X * light.X) + (plane.Normal.Y * light.Y) + (plane.Normal.Z * light.Z) + (plane.D * light.W);
            double x = -plane.Normal.X;
            double y = -plane.Normal.Y;
            double z = -plane.Normal.Z;
            double d = -plane.D;
        
            result.M11 = (x * light.X) + dot;
            result.M21 = y * light.X;
            result.M31 = z * light.X;
            result.M41 = d * light.X;
            result.M12 = x * light.Y;
            result.M22 = (y * light.Y) + dot;
            result.M32 = z * light.Y;
            result.M42 = d * light.Y;
            result.M13 = x * light.Z;
            result.M23 = y * light.Z;
            result.M33 = (z * light.Z) + dot;
            result.M43 = d * light.Z;
            result.M14 = x * light.W;
            result.M24 = y * light.W;
            result.M34 = z * light.W;
            result.M44 = (d * light.W) + dot;
        }
        
        /// <summary>
        /// Creates a MatrixD that flattens geometry into a shadow.
        /// </summary>
        /// <param name="light">The light direction. If the W component is 0, the light is directional light; if the
        /// W component is 1, the light is a point light.</param>
        /// <param name="plane">The plane onto which to project the geometry as a shadow. This parameter is assumed to be normalized.</param>
        /// <returns>The shadow MatrixD.</returns>
        public static MatrixD Shadow(Vector4D light, Plane plane)
        {
            MatrixD result;
            Shadow(ref light, ref plane, out result);
            return result;
        }

        /// <summary>
        /// Creates a MatrixD that scales along the x-axis, y-axis, and y-axis.
        /// </summary>
        /// <param name="scale">Scaling factor for all three axes.</param>
        /// <param name="result">When the method completes, contains the created scaling MatrixD.</param>
        public static void Scaling(ref Vector3D scale, out MatrixD result)
        {
            Scaling(scale.X, scale.Y, scale.Z, out result);
        }

        /// <summary>
        /// Creates a MatrixD that scales along the x-axis, y-axis, and y-axis.
        /// </summary>
        /// <param name="scale">Scaling factor for all three axes.</param>
        /// <returns>The created scaling MatrixD.</returns>
        public static MatrixD Scaling(Vector3D scale)
        {
            MatrixD result;
            Scaling(ref scale, out result);
            return result;
        }

        /// <summary>
        /// Creates a MatrixD that scales along the x-axis, y-axis, and y-axis.
        /// </summary>
        /// <param name="x">Scaling factor that is applied along the x-axis.</param>
        /// <param name="y">Scaling factor that is applied along the y-axis.</param>
        /// <param name="z">Scaling factor that is applied along the z-axis.</param>
        /// <param name="result">When the method completes, contains the created scaling MatrixD.</param>
        public static void Scaling(double x, double y, double z, out MatrixD result)
        {
            result = MatrixD.Identity;
            result.M11 = x;
            result.M22 = y;
            result.M33 = z;
        }

        /// <summary>
        /// Creates a MatrixD that scales along the x-axis, y-axis, and y-axis.
        /// </summary>
        /// <param name="x">Scaling factor that is applied along the x-axis.</param>
        /// <param name="y">Scaling factor that is applied along the y-axis.</param>
        /// <param name="z">Scaling factor that is applied along the z-axis.</param>
        /// <returns>The created scaling MatrixD.</returns>
        public static MatrixD Scaling(double x, double y, double z)
        {
            MatrixD result;
            Scaling(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Creates a MatrixD that uniformly scales along all three axis.
        /// </summary>
        /// <param name="scale">The uniform scale that is applied along all axis.</param>
        /// <param name="result">When the method completes, contains the created scaling MatrixD.</param>
        public static void Scaling(double scale, out MatrixD result)
        {
            result = MatrixD.Identity;
            result.M11 = result.M22 = result.M33 = scale;
        }

        /// <summary>
        /// Creates a MatrixD that uniformly scales along all three axis.
        /// </summary>
        /// <param name="scale">The uniform scale that is applied along all axis.</param>
        /// <returns>The created scaling MatrixD.</returns>
        public static MatrixD Scaling(double scale)
        {
            MatrixD result;
            Scaling(scale, out result);
            return result;
        }

        /// <summary>
        /// Creates a MatrixD that rotates around the x-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <param name="result">When the method completes, contains the created rotation MatrixD.</param>
        public static void RotationX(double angle, out MatrixD result)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            result = MatrixD.Identity;
            result.M22 = cos;
            result.M23 = sin;
            result.M32 = -sin;
            result.M33 = cos;
        }

        /// <summary>
        /// Creates a MatrixD that rotates around the x-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <returns>The created rotation MatrixD.</returns>
        public static MatrixD RotationX(double angle)
        {
            MatrixD result;
            RotationX(angle, out result);
            return result;
        }

        /// <summary>
        /// Creates a MatrixD that rotates around the y-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <param name="result">When the method completes, contains the created rotation MatrixD.</param>
        public static void RotationY(double angle, out MatrixD result)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            result = MatrixD.Identity;
            result.M11 = cos;
            result.M13 = -sin;
            result.M31 = sin;
            result.M33 = cos;
        }

        /// <summary>
        /// Creates a MatrixD that rotates around the y-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <returns>The created rotation MatrixD.</returns>
        public static MatrixD RotationY(double angle)
        {
            MatrixD result;
            RotationY(angle, out result);
            return result;
        }

        /// <summary>
        /// Creates a MatrixD that rotates around the z-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <param name="result">When the method completes, contains the created rotation MatrixD.</param>
        public static void RotationZ(double angle, out MatrixD result)
        {
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);

            result = MatrixD.Identity;
            result.M11 = cos;
            result.M12 = sin;
            result.M21 = -sin;
            result.M22 = cos;
        }

        /// <summary>
        /// Creates a MatrixD that rotates around the z-axis.
        /// </summary>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <returns>The created rotation MatrixD.</returns>
        public static MatrixD RotationZ(double angle)
        {
            MatrixD result;
            RotationZ(angle, out result);
            return result;
        }

        /// <summary>
        /// Creates a MatrixD that rotates around an arbitrary axis.
        /// </summary>
        /// <param name="axis">The axis around which to rotate. This parameter is assumed to be normalized.</param>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <param name="result">When the method completes, contains the created rotation MatrixD.</param>
        public static void RotationAxis(ref Vector3D axis, double angle, out MatrixD result)
        {
            double x = axis.X;
            double y = axis.Y;
            double z = axis.Z;
            double cos = (double)System.Math.Cos(angle);
            double sin = (double)System.Math.Sin(angle);
            double xx = x * x;
            double yy = y * y;
            double zz = z * z;
            double xy = x * y;
            double xz = x * z;
            double yz = y * z;

            result = MatrixD.Identity;
            result.M11 = xx + (cos * (1.0d - xx));
            result.M12 = (xy - (cos * xy)) + (sin * z);
            result.M13 = (xz - (cos * xz)) - (sin * y);
            result.M21 = (xy - (cos * xy)) - (sin * z);
            result.M22 = yy + (cos * (1.0d - yy));
            result.M23 = (yz - (cos * yz)) + (sin * x);
            result.M31 = (xz - (cos * xz)) + (sin * y);
            result.M32 = (yz - (cos * yz)) - (sin * x);
            result.M33 = zz + (cos * (1.0d - zz));
        }

        /// <summary>
        /// Creates a MatrixD that rotates around an arbitrary axis.
        /// </summary>
        /// <param name="axis">The axis around which to rotate. This parameter is assumed to be normalized.</param>
        /// <param name="angle">Angle of rotation in radians. Angles are measured clockwise when looking along the rotation axis toward the origin.</param>
        /// <returns>The created rotation MatrixD.</returns>
        public static MatrixD RotationAxis(Vector3D axis, double angle)
        {
            MatrixD result;
            RotationAxis(ref axis, angle, out result);
            return result;
        }

        /// <summary>
        /// Creates a rotation MatrixD from a quaternion.
        /// </summary>
        /// <param name="rotation">The quaternion to use to build the MatrixD.</param>
        /// <param name="result">The created rotation MatrixD.</param>
        public static void RotationQuaternion(ref Quaternion rotation, out MatrixD result)
        {
            double xx = rotation.X * rotation.X;
            double yy = rotation.Y * rotation.Y;
            double zz = rotation.Z * rotation.Z;
            double xy = rotation.X * rotation.Y;
            double zw = rotation.Z * rotation.W;
            double zx = rotation.Z * rotation.X;
            double yw = rotation.Y * rotation.W;
            double yz = rotation.Y * rotation.Z;
            double xw = rotation.X * rotation.W;

            result = MatrixD.Identity;
            result.M11 = 1.0d - (2.0d * (yy + zz));
            result.M12 = 2.0d * (xy + zw);
            result.M13 = 2.0d * (zx - yw);
            result.M21 = 2.0d * (xy - zw);
            result.M22 = 1.0d - (2.0d * (zz + xx));
            result.M23 = 2.0d * (yz + xw);
            result.M31 = 2.0d * (zx + yw);
            result.M32 = 2.0d * (yz - xw);
            result.M33 = 1.0d - (2.0d * (yy + xx));
        }

        /// <summary>
        /// Creates a rotation MatrixD from a quaternion.
        /// </summary>
        /// <param name="rotation">The quaternion to use to build the MatrixD.</param>
        /// <returns>The created rotation MatrixD.</returns>
        public static MatrixD RotationQuaternion(Quaternion rotation)
        {
            MatrixD result;
            RotationQuaternion(ref rotation, out result);
            return result;
        }

        /// <summary>
        /// Creates a rotation MatrixD with a specified yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw around the y-axis, in radians.</param>
        /// <param name="pitch">Pitch around the x-axis, in radians.</param>
        /// <param name="roll">Roll around the z-axis, in radians.</param>
        /// <param name="result">When the method completes, contains the created rotation MatrixD.</param>
        public static void RotationYawPitchRoll(double yaw, double pitch, double roll, out MatrixD result)
        {
            Quaternion quaternion = new Quaternion();
            Quaternion.RotationYawPitchRoll((float)yaw, (float)pitch, (float)roll, out quaternion); // todo: don't cast to float!
            RotationQuaternion(ref quaternion, out result);
        }

        /// <summary>
        /// Creates a rotation MatrixD with a specified yaw, pitch, and roll.
        /// </summary>
        /// <param name="yaw">Yaw around the y-axis, in radians.</param>
        /// <param name="pitch">Pitch around the x-axis, in radians.</param>
        /// <param name="roll">Roll around the z-axis, in radians.</param>
        /// <returns>The created rotation MatrixD.</returns>
        public static MatrixD RotationYawPitchRoll(double yaw, double pitch, double roll)
        {
            MatrixD result;
            RotationYawPitchRoll(yaw, pitch, roll, out result);
            return result;
        }

        /// <summary>
        /// Creates a translation MatrixD using the specified offsets.
        /// </summary>
        /// <param name="value">The offset for all three coordinate planes.</param>
        /// <param name="result">When the method completes, contains the created translation MatrixD.</param>
        public static void Translation(ref Vector3D value, out MatrixD result)
        {
            Translation(value.X, value.Y, value.Z, out result);
        }

        /// <summary>
        /// Creates a translation MatrixD using the specified offsets.
        /// </summary>
        /// <param name="value">The offset for all three coordinate planes.</param>
        /// <returns>The created translation MatrixD.</returns>
        public static MatrixD Translation(Vector3D value)
        {
            MatrixD result;
            Translation(ref value, out result);
            return result;
        }

        /// <summary>
        /// Creates a translation MatrixD using the specified offsets.
        /// </summary>
        /// <param name="x">X-coordinate offset.</param>
        /// <param name="y">Y-coordinate offset.</param>
        /// <param name="z">Z-coordinate offset.</param>
        /// <param name="result">When the method completes, contains the created translation MatrixD.</param>
        public static void Translation(double x, double y, double z, out MatrixD result)
        {
            result = MatrixD.Identity;
            result.M41 = x;
            result.M42 = y;
            result.M43 = z;
        }

        /// <summary>
        /// Creates a translation MatrixD using the specified offsets.
        /// </summary>
        /// <param name="x">X-coordinate offset.</param>
        /// <param name="y">Y-coordinate offset.</param>
        /// <param name="z">Z-coordinate offset.</param>
        /// <returns>The created translation MatrixD.</returns>
        public static MatrixD Translation(double x, double y, double z)
        {
            MatrixD result;
            Translation(x, y, z, out result);
            return result;
        }

        /// <summary>
        /// Creates a skew/shear MatrixD by means of a translation vector, a rotation vector, and a rotation angle.
        /// shearing is performed in the direction of translation vector, where translation vector and rotation vector define the shearing plane.
        /// The effect is such that the skewed rotation vector has the specified angle with rotation itself.
        /// </summary>
        /// <param name="angle">The rotation angle.</param>
        /// <param name="rotationVec">The rotation vector</param>
        /// <param name="transVec">The translation vector</param>
        /// <param name="MatrixD">Contains the created skew/shear MatrixD. </param>
        public static void Skew(double angle, ref Vector3D rotationVec, ref Vector3D transVec, out MatrixD MatrixD)
        {
            //http://elckerlyc.ewi.utwente.nl/browser/Elckerlyc/Hmi/HmiMath/src/hmi/math/Mat3f.java
            double MINIMAL_SKEW_ANGLE = 0.000001f;

            Vector3D e0 = rotationVec;
            Vector3D e1 = Vector3D.Normalize(transVec);

            double rv1;
            Vector3D.Dot(ref rotationVec, ref  e1, out rv1);
            e0 += rv1 * e1;
            double rv0;
            Vector3D.Dot(ref rotationVec, ref e0, out rv0);
            double cosa = (double)System.Math.Cos(angle);
            double sina = (double)System.Math.Sin(angle);
            double rr0 = rv0 * cosa - rv1 * sina;
            double rr1 = rv0 * sina + rv1 * cosa;

            if (rr0 < MINIMAL_SKEW_ANGLE)
                throw new ArgumentException("illegal skew angle");

            double d = (rr1 / rr0) - (rv1 / rv0);

            MatrixD = MatrixD.Identity;
            MatrixD.M11 = d * e1[0] * e0[0] + 1.0f;
            MatrixD.M12 = d * e1[0] * e0[1];
            MatrixD.M13 = d * e1[0] * e0[2];
            MatrixD.M21 = d * e1[1] * e0[0];
            MatrixD.M22 = d * e1[1] * e0[1] + 1.0f;
            MatrixD.M23 = d * e1[1] * e0[2];
            MatrixD.M31 = d * e1[2] * e0[0];
            MatrixD.M32 = d * e1[2] * e0[1];
            MatrixD.M33 = d * e1[2] * e0[2] + 1.0f;
        }

        /// <summary>
        /// Creates a 3D affine transformation MatrixD.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation MatrixD.</param>
        public static void AffineTransformation(double scaling, ref Quaternion rotation, ref Vector3D translation, out MatrixD result)
        {
            result = Scaling(scaling) * RotationQuaternion(rotation) * Translation(translation);
        }

        /// <summary>
        /// Creates a 3D affine transformation MatrixD.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation MatrixD.</returns>
        public static MatrixD AffineTransformation(double scaling, Quaternion rotation, Vector3D translation)
        {
            MatrixD result;
            AffineTransformation(scaling, ref rotation, ref translation, out result);
            return result;
        }

        /// <summary>
        /// Creates a 3D affine transformation MatrixD.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created affine transformation MatrixD.</param>
        public static void AffineTransformation(double scaling, ref Vector3D rotationCenter, ref Quaternion rotation, ref Vector3D translation, out MatrixD result)
        {
            result = Scaling(scaling) * Translation(-rotationCenter) * RotationQuaternion(rotation) *
                Translation(rotationCenter) * Translation(translation);
        }

        /// <summary>
        /// Creates a 3D affine transformation MatrixD.
        /// </summary>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created affine transformation MatrixD.</returns>
        public static MatrixD AffineTransformation(double scaling, Vector3D rotationCenter, Quaternion rotation, Vector3D translation)
        {
            MatrixD result;
            AffineTransformation(scaling, ref rotationCenter, ref rotation, ref translation, out result);
            return result;
        }


        /// <summary>
        /// Creates a transformation MatrixD.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <param name="result">When the method completes, contains the created transformation MatrixD.</param>
        public static void Transformation(ref Vector3D scalingCenter, ref Quaternion scalingRotation, ref Vector3D scaling, ref Vector3D rotationCenter, ref Quaternion rotation, ref Vector3D translation, out MatrixD result)
        {
            MatrixD sr = RotationQuaternion(scalingRotation);

            result = Translation(-scalingCenter) * Transpose(sr) * Scaling(scaling) * sr * Translation(scalingCenter) * Translation(-rotationCenter) *
                RotationQuaternion(rotation) * Translation(rotationCenter) * Translation(translation);       
        }

        /// <summary>
        /// Creates a transformation MatrixD.
        /// </summary>
        /// <param name="scalingCenter">Center point of the scaling operation.</param>
        /// <param name="scalingRotation">Scaling rotation amount.</param>
        /// <param name="scaling">Scaling factor.</param>
        /// <param name="rotationCenter">The center of the rotation.</param>
        /// <param name="rotation">The rotation of the transformation.</param>
        /// <param name="translation">The translation factor of the transformation.</param>
        /// <returns>The created transformation MatrixD.</returns>
        public static MatrixD Transformation(Vector3D scalingCenter, Quaternion scalingRotation, Vector3D scaling, Vector3D rotationCenter, Quaternion rotation, Vector3D translation)
        {
            MatrixD result;
            Transformation(ref scalingCenter, ref scalingRotation, ref scaling, ref rotationCenter, ref rotation, ref translation, out result);
            return result;
        }


        /// <summary>
        /// Adds two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to add.</param>
        /// <param name="right">The second MatrixD to add.</param>
        /// <returns>The sum of the two matrices.</returns>
        public static MatrixD operator +(MatrixD left, MatrixD right)
        {
            MatrixD result;
            Add(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Assert a MatrixD (return it unchanged).
        /// </summary>
        /// <param name="value">The MatrixD to assert (unchanged).</param>
        /// <returns>The asserted (unchanged) MatrixD.</returns>
        public static MatrixD operator +(MatrixD value)
        {
            return value;
        }

        /// <summary>
        /// Subtracts two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to subtract.</param>
        /// <param name="right">The second MatrixD to subtract.</param>
        /// <returns>The difference between the two matrices.</returns>
        public static MatrixD operator -(MatrixD left, MatrixD right)
        {
            MatrixD result;
            Subtract(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Negates a MatrixD.
        /// </summary>
        /// <param name="value">The MatrixD to negate.</param>
        /// <returns>The negated MatrixD.</returns>
        public static MatrixD operator -(MatrixD value)
        {
            MatrixD result;
            Negate(ref value, out result);
            return result;
        }

        /// <summary>
        /// Scales a MatrixD by a given value.
        /// </summary>
        /// <param name="right">The MatrixD to scale.</param>
        /// <param name="left">The amount by which to scale.</param>
        /// <returns>The scaled MatrixD.</returns>
        public static MatrixD operator *(double left, MatrixD right)
        {
            MatrixD result;
            Multiply(ref right, left, out result);
            return result;
        }

        /// <summary>
        /// Scales a MatrixD by a given value.
        /// </summary>
        /// <param name="left">The MatrixD to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled MatrixD.</returns>
        public static MatrixD operator *(MatrixD left, double right)
        {
            MatrixD result;
            Multiply(ref left, right, out result);
            return result;
        }

        /// <summary>
        /// Multiplies two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to multiply.</param>
        /// <param name="right">The second MatrixD to multiply.</param>
        /// <returns>The product of the two matrices.</returns>
        public static MatrixD operator *(MatrixD left, MatrixD right)
        {
            MatrixD result;
            Multiply(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Scales a MatrixD by a given value.
        /// </summary>
        /// <param name="left">The MatrixD to scale.</param>
        /// <param name="right">The amount by which to scale.</param>
        /// <returns>The scaled MatrixD.</returns>
        public static MatrixD operator /(MatrixD left, double right)
        {
            MatrixD result;
            Divide(ref left, right, out result);
            return result;
        }

        /// <summary>
        /// Divides two matrices.
        /// </summary>
        /// <param name="left">The first MatrixD to divide.</param>
        /// <param name="right">The second MatrixD to divide.</param>
        /// <returns>The quotient of the two matrices.</returns>
        public static MatrixD operator /(MatrixD left, MatrixD right)
        {
            MatrixD result;
            Divide(ref left, ref right, out result);
            return result;
        }

        /// <summary>
        /// Tests for equality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has the same value as <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator ==(MatrixD left, MatrixD right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Tests for inequality between two objects.
        /// </summary>
        /// <param name="left">The first value to compare.</param>
        /// <param name="right">The second value to compare.</param>
        /// <returns><c>true</c> if <paramref name="left"/> has a different value than <paramref name="right"/>; otherwise, <c>false</c>.</returns>
        public static bool operator !=(MatrixD left, MatrixD right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
                M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44);
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

            return string.Format(format, CultureInfo.CurrentCulture, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
                M11.ToString(format, CultureInfo.CurrentCulture), M12.ToString(format, CultureInfo.CurrentCulture), M13.ToString(format, CultureInfo.CurrentCulture), M14.ToString(format, CultureInfo.CurrentCulture),
                M21.ToString(format, CultureInfo.CurrentCulture), M22.ToString(format, CultureInfo.CurrentCulture), M23.ToString(format, CultureInfo.CurrentCulture), M24.ToString(format, CultureInfo.CurrentCulture),
                M31.ToString(format, CultureInfo.CurrentCulture), M32.ToString(format, CultureInfo.CurrentCulture), M33.ToString(format, CultureInfo.CurrentCulture), M34.ToString(format, CultureInfo.CurrentCulture),
                M41.ToString(format, CultureInfo.CurrentCulture), M42.ToString(format, CultureInfo.CurrentCulture), M43.ToString(format, CultureInfo.CurrentCulture), M44.ToString(format, CultureInfo.CurrentCulture));
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
            return string.Format(formatProvider, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
                M11.ToString(formatProvider), M12.ToString(formatProvider), M13.ToString(formatProvider), M14.ToString(formatProvider),
                M21.ToString(formatProvider), M22.ToString(formatProvider), M23.ToString(formatProvider), M24.ToString(formatProvider),
                M31.ToString(formatProvider), M32.ToString(formatProvider), M33.ToString(formatProvider), M34.ToString(formatProvider),
                M41.ToString(formatProvider), M42.ToString(formatProvider), M43.ToString(formatProvider), M44.ToString(formatProvider));
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

            return string.Format(format, formatProvider, "[M11:{0} M12:{1} M13:{2} M14:{3}] [M21:{4} M22:{5} M23:{6} M24:{7}] [M31:{8} M32:{9} M33:{10} M34:{11}] [M41:{12} M42:{13} M43:{14} M44:{15}]",
                M11.ToString(format, formatProvider), M12.ToString(format, formatProvider), M13.ToString(format, formatProvider), M14.ToString(format, formatProvider),
                M21.ToString(format, formatProvider), M22.ToString(format, formatProvider), M23.ToString(format, formatProvider), M24.ToString(format, formatProvider),
                M31.ToString(format, formatProvider), M32.ToString(format, formatProvider), M33.ToString(format, formatProvider), M34.ToString(format, formatProvider),
                M41.ToString(format, formatProvider), M42.ToString(format, formatProvider), M43.ToString(format, formatProvider), M44.ToString(format, formatProvider));
        }

        public Matrix ToMatrix()
        {
            return new Matrix((float)this.M11, (float)this.M12, (float)this.M13, (float)this.M14,
                                (float)this.M21, (float)this.M22, (float)this.M23, (float)this.M24,
                                (float)this.M31, (float)this.M32, (float)this.M33, (float)this.M34,
                                (float)this.M41, (float)this.M42, (float)this.M43, (float)this.M44);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return M11.GetHashCode() + M12.GetHashCode() + M13.GetHashCode() + M14.GetHashCode() +
               M21.GetHashCode() + M22.GetHashCode() + M23.GetHashCode() + M24.GetHashCode() +
               M31.GetHashCode() + M32.GetHashCode() + M33.GetHashCode() + M34.GetHashCode() +
               M41.GetHashCode() + M42.GetHashCode() + M43.GetHashCode() + M44.GetHashCode();
        }

        
        /// <summary>
        /// Determines whether the specified <see cref="SharpDX.MatrixD"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="SharpDX.MatrixD"/> to compare with this instance.</param>
        /// <returns>
        /// <c>true</c> if the specified <see cref="SharpDX.MatrixD"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(MatrixD other)
        {
            
            return (MathUtilD.WithinEpsilon(other.M11, M11) &&
                MathUtilD.WithinEpsilon(other.M12, M12) &&
                MathUtilD.WithinEpsilon(other.M13, M13) &&
                MathUtilD.WithinEpsilon(other.M14, M14) &&
                MathUtilD.WithinEpsilon(other.M21, M21) &&
                MathUtilD.WithinEpsilon(other.M22, M22) &&
                MathUtilD.WithinEpsilon(other.M23, M23) &&
                MathUtilD.WithinEpsilon(other.M24, M24) &&
                MathUtilD.WithinEpsilon(other.M31, M31) &&
                MathUtilD.WithinEpsilon(other.M32, M32) &&
                MathUtilD.WithinEpsilon(other.M33, M33) &&
                MathUtilD.WithinEpsilon(other.M34, M34) &&
                MathUtilD.WithinEpsilon(other.M41, M41) &&
                MathUtilD.WithinEpsilon(other.M42, M42) &&
                MathUtilD.WithinEpsilon(other.M43, M43) &&
                MathUtilD.WithinEpsilon(other.M44, M44));
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
            if (value == null)
                return false;

            if (!ReferenceEquals(value.GetType(), typeof(MatrixD)))
                return false;

            return Equals((MatrixD)value);
        }
    }
}
