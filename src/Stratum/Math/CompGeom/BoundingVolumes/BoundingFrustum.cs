using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public class BoundingFrustum : BoundingVolume
    {
        public BoundingFrustum()
        {
        }

        public BoundingFrustum(SharpDX.BoundingFrustum bf)
        {
            this.sbf = bf;
        }

        public BoundingFrustum(Matrix viewProj)
        {
            this.sbf = new SharpDX.BoundingFrustum(viewProj);
        }

        public SharpDX.BoundingFrustum sbf;

        public ContainmentType Contains(BoundingSphere other)
        {
            return sbf.Contains(ref other.sSphere);
        }

        public void Contains(BoundingSphere other, out ContainmentType ct)
        {
			if (other == null)
				ct = ContainmentType.Disjoint;
			else
				ct = sbf.Contains(ref other.sSphere);
        }

        public ContainmentType Contains(AxisAlignedBoundingBox other)
        {
            return sbf.Contains(ref other.sbb);
        }

        public bool Contains(BoundingFrustum other)
        {
            return sbf.Contains(ref other.sbf);
        }

        public ContainmentType Contains(Vector3 point)
        {
            return sbf.Contains(ref point);
        }

        public override Vector3 Support(Vector3 dir, float dt)
        {
            throw new NotImplementedException();
        }

        #region Test

        public override bool Test(BoundingSphere other)
        {
            return sbf.Intersects(ref other.sSphere);
        }

        public override bool TestMoving(BoundingSphere other, Vector3 v0, Vector3 v1, out float t)
        {
            throw new NotImplementedException();
        }

        public override bool Test(AxisAlignedBoundingBox other)
        {
            return sbf.Intersects(ref other.sbb);
        }

        public override bool Test(BoundingFrustum other)
        {
            // TODO
            throw new NotImplementedException();
        }

        public override bool Test(Line other)
        {
            return sbf.Intersects(ref other.rn) || sbf.Intersects(ref other.rp);
        }

        public override bool Test(Ray other)
        {
            return sbf.Intersects(ref other.sray);
        }

        public override bool Test(Plane other)
        {
            return sbf.Intersects(ref other.splane) == PlaneIntersectionType.Intersecting;
        }

        public override bool Test(LineSegment other)
        {
			return IntersectionTests.Test(other, this);
        }

		public override bool Test(Triangle other)
		{
            // TODO
			throw new NotImplementedException();
		}

		public override bool Test(Cylinder other)
		{
            // TODO
			throw new NotImplementedException();
        }

        #endregion

        #region Merge

        public override BoundingVolume Merge(BoundingSphere other)
        {
            throw new NotImplementedException();
        }

        public override BoundingVolume Merge(AxisAlignedBoundingBox other)
        {
            throw new NotImplementedException();
        }

        public override BoundingVolume Merge(BoundingFrustum other)
        {
            throw new NotImplementedException();
        }

        public override BoundingVolume Merge(Triangle other)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
