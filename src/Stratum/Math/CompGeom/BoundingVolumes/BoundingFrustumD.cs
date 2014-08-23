using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    /// <summary>
    /// Defines a frustum which can be used in frustum culling, zoom to Extents (zoom to fit) operations, 
    /// (MatrixD, frustum, camera) interchange, and many kind of intersection testing.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct BoundingFrustumD : IEquatable<BoundingFrustumD>
    {
        private MatrixD pMatrixD;
        private PlaneD pNear;
        private PlaneD pFar;
        private PlaneD pLeft;
        private PlaneD pRight;
        private PlaneD pTop;
        private PlaneD pBottom;

        /// <summary>
        /// Gets the near PlaneD of the BoundingFrustum.
        /// </summary>
        public PlaneD Near
        {
            get
            {
                return pNear;
            }
        }
        /// <summary>
        /// Gets the far PlaneD of the BoundingFrustum.
        /// </summary>
        public PlaneD Far
        {
            get
            {
                return pFar;
            }
        }
        /// <summary>
        /// Gets the left PlaneD of the BoundingFrustum.
        /// </summary>
        public PlaneD Left
        {
            get
            {
                return pLeft;
            }
        }
        /// <summary>
        /// Gets the right PlaneD of the BoundingFrustum.
        /// </summary>
        public PlaneD Right
        {
            get
            {
                return pRight;
            }
        }
        /// <summary>
        /// Gets the top PlaneD of the BoundingFrustum.
        /// </summary>
        public PlaneD Top
        {
            get
            {
                return pTop;
            }
        }
        /// <summary>
        /// Gets the bottom PlaneD of the BoundingFrustum.
        /// </summary>
        public PlaneD Bottom
        {
            get
            {
                return pBottom;
            }
        }

        /// <summary>
        /// Creates a new instance of BoundingFrustum.
        /// </summary>
        /// <param name="MatrixD">Combined MatrixD that usually takes view × projection MatrixD.</param>
        public BoundingFrustumD(MatrixD MatrixD)
        {
            pMatrixD = MatrixD;
            GetPlanesFromMatrix(ref pMatrixD, out pNear, out pFar, out pLeft, out pRight, out pTop, out pBottom);
        }

        public override int GetHashCode()
        {
            return pMatrixD.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="BoundingFrustum"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="BoundingFrustum"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="BoundingFrustum"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(BoundingFrustumD other)
        {
            return this.pMatrixD == other.pMatrixD;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object"/> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object"/> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj != null && obj is BoundingFrustumD)
                return Equals((BoundingFrustumD)obj);
            return false;
        }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(BoundingFrustumD left, BoundingFrustumD right)
        {
            return left.Equals(right);
        }

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(BoundingFrustumD left, BoundingFrustumD right)
        {
            return !left.Equals(right);
        }

        /// <summary>
        /// Returns one of the 6 PlaneDs related to this frustum.
        /// </summary>
        /// <param name="index">PlaneD index where 0 fro Left, 1 for Right, 2 for Top, 3 for Bottom, 4 for Near, 5 for Far</param>
        /// <returns></returns>
        public PlaneD GetPlaneD(int index)
        {
            switch (index)
            {
                case 0: return pLeft;
                case 1: return pRight;
                case 2: return pTop;
                case 3: return pBottom;
                case 4: return pNear;
                case 5: return pFar;
                default:
                    return new PlaneD();
            }
        }

        private static void GetPlanesFromMatrix(ref MatrixD matrix, out PlaneD near, out PlaneD far, out PlaneD left, out PlaneD right, out PlaneD top, out PlaneD bottom)
        {
            //http://www.chadvernon.com/blog/resources/directx9/frustum-culling/

            // Left plane
            left.Normal.X = matrix.M14 + matrix.M11;
            left.Normal.Y = matrix.M24 + matrix.M21;
            left.Normal.Z = matrix.M34 + matrix.M31;
            left.D = matrix.M44 + matrix.M41;
            left.Normalize();

            // Right plane
            right.Normal.X = matrix.M14 - matrix.M11;
            right.Normal.Y = matrix.M24 - matrix.M21;
            right.Normal.Z = matrix.M34 - matrix.M31;
            right.D = matrix.M44 - matrix.M41;
            right.Normalize();

            // Top plane
            top.Normal.X = matrix.M14 - matrix.M12;
            top.Normal.Y = matrix.M24 - matrix.M22;
            top.Normal.Z = matrix.M34 - matrix.M32;
            top.D = matrix.M44 - matrix.M42;
            top.Normalize();

            // Bottom plane
            bottom.Normal.X = matrix.M14 + matrix.M12;
            bottom.Normal.Y = matrix.M24 + matrix.M22;
            bottom.Normal.Z = matrix.M34 + matrix.M32;
            bottom.D = matrix.M44 + matrix.M42;
            bottom.Normalize();

            // Near plane
            near.Normal.X = matrix.M13;
            near.Normal.Y = matrix.M23;
            near.Normal.Z = matrix.M33;
            near.D = matrix.M43;
            near.Normalize();

            // Far plane
            far.Normal.X = matrix.M14 - matrix.M13;
            far.Normal.Y = matrix.M24 - matrix.M23;
            far.Normal.Z = matrix.M34 - matrix.M33;
            far.D = matrix.M44 - matrix.M43;
            far.Normalize();
        }

        private static Vector3D Get3PlaneDsInterPoint(ref PlaneD p1, ref PlaneD p2, ref PlaneD p3)
        {
            //P = -d1 * N2xN3 / N1.N2xN3 - d2 * N3xN1 / N2.N3xN1 - d3 * N1xN2 / N3.N1xN2 
            Vector3D v =
                -p1.D * Vector3D.Cross(p2.Normal, p3.Normal) / Vector3D.Dot(p1.Normal, Vector3D.Cross(p2.Normal, p3.Normal))
                - p2.D * Vector3D.Cross(p3.Normal, p1.Normal) / Vector3D.Dot(p2.Normal, Vector3D.Cross(p3.Normal, p1.Normal))
                - p3.D * Vector3D.Cross(p1.Normal, p2.Normal) / Vector3D.Dot(p3.Normal, Vector3D.Cross(p1.Normal, p2.Normal));

            return v;
        }

        /// <summary>
        /// Creates a new frustum relaying on perspective camera parameters
        /// </summary>
        /// <param name="cameraPos">The camera pos.</param>
        /// <param name="lookDir">The look dir.</param>
        /// <param name="upDir">Up dir.</param>
        /// <param name="fov">The fov.</param>
        /// <param name="znear">The znear.</param>
        /// <param name="zfar">The zfar.</param>
        /// <param name="aspect">The aspect.</param>
        /// <returns>The bounding frustum calculated from perspective camera</returns>
        public static BoundingFrustumD FromCamera(Vector3D cameraPos, Vector3D lookDir, Vector3D upDir, float fov, float znear, float zfar, float aspect)
        {
            //http://knol.google.com/k/view-frustum

            lookDir = Vector3D.Normalize(lookDir);
            upDir = Vector3D.Normalize(upDir);

            Vector3D nearCenter = cameraPos + lookDir * znear;
            Vector3D farCenter = cameraPos + lookDir * zfar;
            float nearHalfHeight = (float)(znear * Math.Tan(fov / 2f));
            float farHalfHeight = (float)(zfar * Math.Tan(fov / 2f));
            float nearHalfWidth = nearHalfHeight * aspect;
            float farHalfWidth = farHalfHeight * aspect;

            Vector3D rightDir = Vector3D.Normalize(Vector3D.Cross(upDir, lookDir));
            Vector3D Near1 = nearCenter - nearHalfHeight * upDir + nearHalfWidth * rightDir;
            Vector3D Near2 = nearCenter + nearHalfHeight * upDir + nearHalfWidth * rightDir;
            Vector3D Near3 = nearCenter + nearHalfHeight * upDir - nearHalfWidth * rightDir;
            Vector3D Near4 = nearCenter - nearHalfHeight * upDir - nearHalfWidth * rightDir;
            Vector3D Far1 = farCenter - farHalfHeight * upDir + farHalfWidth * rightDir;
            Vector3D Far2 = farCenter + farHalfHeight * upDir + farHalfWidth * rightDir;
            Vector3D Far3 = farCenter + farHalfHeight * upDir - farHalfWidth * rightDir;
            Vector3D Far4 = farCenter - farHalfHeight * upDir - farHalfWidth * rightDir;

            var result = new BoundingFrustumD();
            result.pNear = new PlaneD(Near1, Near2, Near3);
            result.pFar = new PlaneD(Far3, Far2, Far1);
            result.pLeft = new PlaneD(Near4, Near3, Far3);
            result.pRight = new PlaneD(Far1, Far2, Near2);
            result.pTop = new PlaneD(Near2, Far2, Far3);
            result.pBottom = new PlaneD(Far4, Far1, Near1);

            result.pNear.Normalize();
            result.pFar.Normalize();
            result.pLeft.Normalize();
            result.pRight.Normalize();
            result.pTop.Normalize();
            result.pBottom.Normalize();

            result.pMatrixD = MatrixD.LookAtLH(cameraPos, cameraPos + lookDir * 10, upDir) * MatrixD.PerspectiveFovLH(fov, aspect, znear, zfar);

            return result;
        }

        /// <summary>
        /// Returns the 8 corners of the frustum, element0 is Near1 (near right down corner)
        /// , element1 is Near2 (near right top corner)
        /// , element2 is Near3 (near Left top corner)
        /// , element3 is Near4 (near Left down corner)
        /// , element4 is Far1 (far right down corner)
        /// , element5 is Far2 (far right top corner)
        /// , element6 is Far3 (far left top corner)
        /// , element7 is Far4 (far left down corner)
        /// </summary>
        /// <returns>The 8 corners of the frustum</returns>
        public Vector3D[] GetCorners()
        {
            var corners = new Vector3D[8];
            GetCorners(corners);
            return corners;
        }

        /// <summary>
        /// Returns the 8 corners of the frustum, element0 is Near1 (near right down corner)
        /// , element1 is Near2 (near right top corner)
        /// , element2 is Near3 (near Left top corner)
        /// , element3 is Near4 (near Left down corner)
        /// , element4 is Far1 (far right down corner)
        /// , element5 is Far2 (far right top corner)
        /// , element6 is Far3 (far left top corner)
        /// , element7 is Far4 (far left down corner)
        /// </summary>
        /// <returns>The 8 corners of the frustum</returns>
        public void GetCorners(Vector3D[] corners)
        {
            corners[0] = Get3PlaneDsInterPoint(ref pNear, ref  pBottom, ref  pRight);    //Near1
            corners[1] = Get3PlaneDsInterPoint(ref pNear, ref  pTop, ref  pRight);       //Near2
            corners[2] = Get3PlaneDsInterPoint(ref pNear, ref  pTop, ref  pLeft);        //Near3
            corners[3] = Get3PlaneDsInterPoint(ref pNear, ref  pBottom, ref  pLeft);     //Near3
            corners[4] = Get3PlaneDsInterPoint(ref pFar, ref  pBottom, ref  pRight);    //Far1
            corners[5] = Get3PlaneDsInterPoint(ref pFar, ref  pTop, ref  pRight);       //Far2
            corners[6] = Get3PlaneDsInterPoint(ref pFar, ref  pTop, ref  pLeft);        //Far3
            corners[7] = Get3PlaneDsInterPoint(ref pFar, ref  pBottom, ref  pLeft);     //Far3
        }

        /// <summary>
        /// Checks whether a point lay inside, intersects or lay outside the frustum.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Type of the containment</returns>
        public ContainmentType Contains(ref Vector3D point)
        {
            var result = PlaneIntersectionType.Front;
            var PlaneDResult = PlaneIntersectionType.Front;
            for (int i = 0; i < 6; i++)
            {
                switch (i)
                {
                    case 0: PlaneDResult = pNear.Intersects(ref point); break;
                    case 1: PlaneDResult = pFar.Intersects(ref point); break;
                    case 2: PlaneDResult = pLeft.Intersects(ref point); break;
                    case 3: PlaneDResult = pRight.Intersects(ref point); break;
                    case 4: PlaneDResult = pTop.Intersects(ref point); break;
                    case 5: PlaneDResult = pBottom.Intersects(ref point); break;
                }
                switch (PlaneDResult)
                {
                    case PlaneIntersectionType.Back:
                        return ContainmentType.Disjoint;
                    case PlaneIntersectionType.Intersecting:
                        result = PlaneIntersectionType.Intersecting;
                        break;
                }
            }
            switch (result)
            {
                case PlaneIntersectionType.Intersecting: return ContainmentType.Intersects;
                default: return ContainmentType.Contains;
            }
        }

        /// <summary>
        /// Checks whether a point lay inside, intersects or lay outside the frustum.
        /// </summary>
        /// <param name="point">The point.</param>
        /// <returns>Type of the containment</returns>
        public ContainmentType Contains(Vector3D point)
        {
            return Contains(ref point);
        }

        /// <summary>
        /// Get the width of the frustum at specified depth.
        /// </summary>
        /// <param name="depth">the depth at which to calculate frustum width.</param>
        /// <returns>With of the frustum at the specified depth</returns>
        public float GetWidthAtDepth(double depth)
        {
            double hAngle = ((Math.PI / 2.0 - Math.Acos(Vector3D.Dot(pNear.Normal, pLeft.Normal))));
            return (float)(Math.Tan(hAngle) * depth * 2);
        }

        /// <summary>
        /// Get the height of the frustum at specified depth.
        /// </summary>
        /// <param name="depth">the depth at which to calculate frustum height.</param>
        /// <returns>Height of the frustum at the specified depth</returns>
        public float GetHeightAtDepth(float depth)
        {
            float vAngle = (float)((Math.PI / 2.0 - Math.Acos(Vector3D.Dot(pNear.Normal, pTop.Normal))));
            return (float)(Math.Tan(vAngle) * depth * 2);
        }

        /// <summary>
        /// Indicate whether the current BoundingFrustrum is Orthographic.
        /// </summary>
        /// <value>
        /// 	<c>true</c> if the current BoundingFrustrum is Orthographic; otherwise, <c>false</c>.
        /// </value>
        public bool IsOrthographic
        {
            get
            {
                return (pLeft.Normal == -pRight.Normal) && (pTop.Normal == -pBottom.Normal);
            }
        }
    }
}
