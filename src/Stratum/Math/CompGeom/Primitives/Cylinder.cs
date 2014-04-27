using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
	public class Cylinder : Primitive
	{
		public override bool Test(AxisAlignedBoundingBox other)
		{
			throw new NotImplementedException();
		}

		public override bool Test(BoundingSphere other)
		{
			throw new NotImplementedException();
		}

		public override bool Test(BoundingFrustum other)
		{
			throw new NotImplementedException();
		}

		public override bool Test(Line other)
		{
			throw new NotImplementedException();
		}

		public override bool Test(Ray other)
		{
			throw new NotImplementedException();
		}

		public override bool Test(Plane other)
		{
			throw new NotImplementedException();
		}

		public override bool Test(LineSegment other)
		{
			throw new NotImplementedException();
		}

		public override bool Test(Triangle other)
		{
			throw new NotImplementedException();
		}

		public override bool Test(Cylinder other)
		{
			throw new NotImplementedException();
		}
	}
}
