using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public class Plane : Primitive
    {
        public Plane()
        {
        }

        public Plane(SharpDX.Plane plane)
        {
            this.splane = plane;
        }

        public Plane(Vector3 pointOnPlane, Vector3 normal)
        {
            this.Point = pointOnPlane;
            this.splane = new SharpDX.Plane(pointOnPlane, normal);
        }

        public Plane(Vector3 p1, Vector3 p2, Vector3 p3)
        {
            this.Point = p1;
            this.splane = new SharpDX.Plane(p1, p2, p3);
        }

        public SharpDX.Plane splane;

        public Vector3 Point { get; set; }

        public float D
        {
            get { return splane.D; }
            set { splane.D = value; }
        }

        public Vector3 Normal
        {
            get { return splane.Normal; }
            set { splane.Normal = value; }
        }

        public override bool Test(AxisAlignedBoundingBox other)
        {
            return splane.Intersects(ref other.sbb) == PlaneIntersectionType.Intersecting;
        }

        public override bool Test(BoundingSphere other)
        {
            return splane.Intersects(ref other.sSphere) == PlaneIntersectionType.Intersecting;
        }

        public override bool Test(BoundingFrustum other)
        {
            return other.sbf.Intersects(ref splane) == PlaneIntersectionType.Intersecting;
        }

        public override bool Test(Line other)
        {
            return splane.Intersects(ref other.rn) || splane.Intersects(ref other.rp);
        }

        public override bool Test(Ray other)
        {
            return splane.Intersects(ref other.sray);
        }

        public override bool Test(Plane other)
        {
            return splane.Intersects(ref other.splane);
        }

        public override bool Test(LineSegment other)
        {
			return IntersectionTests.Test(other, this);
        }

		public override bool Test(Triangle other)
		{
            return IntersectionTests.Test(this.splane, other.P1, other.P2, other.P3);
		}

		public override bool Test(Cylinder other)
		{
            // TODO
			throw new NotImplementedException();
		}
	}
}
