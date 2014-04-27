using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Stratum.CompGeom
{
	public struct STriangle
	{
		public STriangle(Vector3 _p1, Vector3 _p2, Vector3 _p3)
		{
			p1 = _p1;
			p2 = _p2;
			p3 = _p3;
		}

		public Vector3 p1;
		public Vector3 p2;
		public Vector3 p3;
	}

	public class Triangle : Primitive
	{
		public Triangle()
		{
		}

		public Triangle(Vector3 p1, Vector3 p2, Vector3 p3)
		{
			this.P1 = p1;
			this.P2 = p2;
			this.P3 = p3;

			this.normal = Vector3.Cross(P1 - P2, P3 - P2);
		}

		public Vector3 P1;
		public Vector3 P2;
		public Vector3 P3;

		public Edge e1;
		public Edge e2;
		public Edge e3;

		private Vector3 normal;
		public Vector3 Normal { get { return normal; } }

		public override bool Test(AxisAlignedBoundingBox other)
		{
            var c1 = Collision.BoxContainsPoint(ref other.sbb, ref P1);
            if (c1 == ContainmentType.Disjoint)
            {
                var c2 = Collision.BoxContainsPoint(ref other.sbb, ref P2);
                if (c2 == ContainmentType.Disjoint)
                {
                    var c3 = Collision.BoxContainsPoint(ref other.sbb, ref P3);
                    if (c3 == ContainmentType.Disjoint)
                    {
                        return false;
                    }
                }
            }

            return true;
		}

		public override bool Test(BoundingSphere other)
		{
            return Collision.SphereIntersectsTriangle(ref other.sSphere, ref P1, ref P2, ref P3);
		}

		public override bool Test(BoundingFrustum other)
		{
            var c1 = other.sbf.Contains(ref P1);
            if (c1 == ContainmentType.Disjoint)
            {
                var c2 = other.sbf.Contains(ref P2);
                if (c2 == ContainmentType.Disjoint)
                {
                    var c3 = other.sbf.Contains(ref P3);
                    if (c3 == ContainmentType.Disjoint)
                    {
                        return false;
                    }
                }
            }

            return true;
		}

		public override bool Test(Line other)
		{
            float op, on;
            return Collision.RayIntersectsTriangle(ref other.rp, ref P1, ref P2, ref P3, out op) || 
                Collision.RayIntersectsTriangle(ref other.rn, ref P1, ref P2, ref P3, out on);
		}

		public override bool Test(Ray other)
		{
            float o;
            return Collision.RayIntersectsTriangle(ref other.sray, ref P1, ref P2, ref P3, out o);
		}

		public override bool Test(Plane other)
		{
            return Collision.PlaneIntersectsTriangle(ref other.splane, ref P1, ref P2, ref P3) == PlaneIntersectionType.Intersecting;
		}

		public override bool Test(LineSegment other)
		{
            
            return IntersectionTests.Test(other, P1, P2, P3);
		}

		public override bool Test(Triangle other)
		{
            return IntersectionTests.Test(this.P1, this.P2, this.P3, other.P1, other.P2, other.P3);
		}

		public override bool Test(Cylinder other)
		{
            // TODO
			throw new NotImplementedException();
		}
	}
}
