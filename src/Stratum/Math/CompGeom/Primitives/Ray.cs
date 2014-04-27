using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public class Ray : Primitive
    {
        public Ray()
        {
        }

        public Ray(SharpDX.Ray ray)
        {
            this.sray = ray;
        }

        public Ray(Vector3 point, Vector3 direction)
        {
            this.sray = new SharpDX.Ray(point, direction);
        }

        public SharpDX.Ray sray;

        public override bool Test(AxisAlignedBoundingBox other)
        {
            return sray.Intersects(ref other.sbb);
        }

        public override bool Test(BoundingSphere other)
        {
            return sray.Intersects(ref other.sSphere);
        }

        public override bool Test(BoundingFrustum other)
        {
            return other.sbf.Intersects(ref sray);
        }

        public override bool Test(Line other)
        {
            return sray.Intersects(ref other.rp) || sray.Intersects(ref other.rn);
        }

        public override bool Test(Ray other)
        {
            return sray.Intersects(ref other.sray);
        }

        public override bool Test(Plane other)
        {
            return sray.Intersects(ref other.splane);
        }

        public override bool Test(LineSegment other)
        {
            // TODO
            throw new NotImplementedException();
        }

		public override bool Test(Triangle other)
		{
            return IntersectionTests.Test(this.sray, other.P1, other.P2, other.P3);
		}

		public override bool Test(Cylinder other)
		{
            // TODO
			throw new NotImplementedException();
		}
	}
}
