using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public struct PlaneD
    {
        public double D;
        public Vector3D Normal;

        public PlaneD(Vector3D point, Vector3D normal)
        {
            this.D = -Vector3D.Dot(normal, point);
            this.Normal = normal;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharpDX.Plane"/> struct.
        /// </summary>
        /// <param name="point1">First point of a triangle defining the plane.</param>
        /// <param name="point2">Second point of a triangle defining the plane.</param>
        /// <param name="point3">Third point of a triangle defining the plane.</param>
        public PlaneD(Vector3D point1, Vector3D point2, Vector3D point3)
        {
            double x1 = point2.X - point1.X;
            double y1 = point2.Y - point1.Y;
            double z1 = point2.Z - point1.Z;
            double x2 = point3.X - point1.X;
            double y2 = point3.Y - point1.Y;
            double z2 = point3.Z - point1.Z;
            double yz = (y1 * z2) - (z1 * y2);
            double xz = (z1 * x2) - (x1 * z2);
            double xy = (x1 * y2) - (y1 * x2);
            double invPyth = 1.0d / (Math.Sqrt((yz * yz) + (xz * xz) + (xy * xy)));

            Normal = new Vector3D();
            Normal.X = yz * invPyth;
            Normal.Y = xz * invPyth;
            Normal.Z = xy * invPyth;
            D = -((Normal.X * point1.X) + (Normal.Y * point1.Y) + (Normal.Z * point1.Z));
        }

        /// <summary>
        /// Changes the coefficients of the normal vector of the plane to make it of unit length.
        /// </summary>
        public void Normalize()
        {
            double magnitude = 1.0d / (Math.Sqrt((Normal.X * Normal.X) + (Normal.Y * Normal.Y) + (Normal.Z * Normal.Z)));

            Normal.X *= magnitude;
            Normal.Y *= magnitude;
            Normal.Z *= magnitude;
            D *= magnitude;
        }

        /// <summary>
        /// Determines whether there is an intersection between a <see cref="SharpDX.Plane"/> and a point.
        /// </summary>
        /// <param name="plane">The plane to test.</param>
        /// <param name="point">The point to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        private static PlaneIntersectionType PlaneIntersectsPoint(ref PlaneD plane, ref Vector3D point)
        {
            double distance;
            Vector3D.Dot(ref plane.Normal, ref point, out distance);
            distance += plane.D;

            if (distance > 0f)
                return PlaneIntersectionType.Front;

            if (distance < 0f)
                return PlaneIntersectionType.Back;

            return PlaneIntersectionType.Intersecting;
        }

        /// <summary>
        /// Determines if there is an intersection between the current object and a point.
        /// </summary>
        /// <param name="point">The point to test.</param>
        /// <returns>Whether the two objects intersected.</returns>
        public PlaneIntersectionType Intersects(ref Vector3D point)
        {
            return PlaneIntersectsPoint(ref this, ref point);
        }
    }
}
