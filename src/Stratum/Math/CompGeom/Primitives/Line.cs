using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
	public struct SLine
	{
		public SLine(SharpDX.Ray ray)
		{
			point = ray.Position;
			rp = ray;
			rn = new SharpDX.Ray(ray.Position, -ray.Direction);
		}

		public SLine(Vector3 p, Vector3 dir)
		{
			point = p;
			rp = new SharpDX.Ray(p, dir);
			rn = new SharpDX.Ray(p, -dir);
		}

		public Vector3 point;
		public SharpDX.Ray rp;
		public SharpDX.Ray rn;
	}

    public class Line : Primitive
    {
        public Line()
        {
        }

        public Line(SharpDX.Ray ray)
        {
            this.Point = ray.Position;
            this.rp = ray;
            this.rn = new SharpDX.Ray(ray.Position, -ray.Direction);
        }

        public Line(Vector3 point, Vector3 direction)
        {
            this.Point = point;
            this.rp = new SharpDX.Ray(point, direction);
            this.rn = new SharpDX.Ray(point, -direction);
        }

        public Vector3 Point { get; set; }
        public SharpDX.Ray rp;
        public SharpDX.Ray rn;

        public override bool Test(AxisAlignedBoundingBox other)
        {
            return rp.Intersects(ref other.sbb) || rn.Intersects(ref other.sbb);
        }

        public override bool Test(BoundingSphere other)
        {
            return rp.Intersects(ref other.sSphere) || rn.Intersects(ref other.sSphere);
        }

        public override bool Test(BoundingFrustum other)
        {
            return other.sbf.Intersects(ref rp) || other.sbf.Intersects(ref rn);
        }

        public override bool Test(Line other)
        {
            return rp.Intersects(ref other.rp) || rp.Intersects(ref other.rn)
                || rn.Intersects(ref other.rp) || rn.Intersects(ref other.rn);
        }

        public override bool Test(Ray other)
        {
            return rp.Intersects(ref other.sray) || rn.Intersects(ref other.sray);
        }

        public override bool Test(Plane other)
        {
            return rp.Intersects(ref other.splane) || rn.Intersects(ref other.splane);
        }

        public override bool Test(LineSegment other)
        {
            // TODO
            throw new NotImplementedException();
        }

		public override bool Test(Triangle other)
		{
            return IntersectionTests.Test(this, other.P1, other.P2, other.P3);
		}

		public override bool Test(Cylinder other)
		{
            // TODO
			throw new NotImplementedException();
		}
	}
}
