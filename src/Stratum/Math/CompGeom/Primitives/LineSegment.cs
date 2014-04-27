using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public class LineSegment : Primitive
    {
        public LineSegment()
        {

        }

        public LineSegment(Vector3 point1, Vector3 point2)
        {
            this.Point1 = point1;
            this.Point2 = point2;
        }

        public Vector3 Point1 { get; set; }
        public Vector3 Point2 { get; set; }
        public Vector3 Direction { get { return Point2 - Point1; } }

        public override bool Test(AxisAlignedBoundingBox other)
        {
			return IntersectionTests.Test(this, other);
        }

        public override bool Test(BoundingSphere other)
        {
			return IntersectionTests.Test(this, other);
        }

        public override bool Test(BoundingFrustum other)
        {
			return IntersectionTests.Test(this, other);
        }

        public override bool Test(Line other)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override bool Test(Ray other)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override bool Test(Plane other)
        {
			return IntersectionTests.Test(this, other);
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
			throw new NotImplementedException();
		}
	}
}
