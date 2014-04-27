using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public class BoundingSphere : BoundingVolume
    {
        public BoundingSphere()
        {
        }

        public BoundingSphere(Vector3 center, float radius)
        {
            this.sSphere = new SharpDX.BoundingSphere(center, radius);
        }

        public BoundingSphere(SharpDX.BoundingSphere sphere)
        {
            this.sSphere = sphere;
        }

        public SharpDX.BoundingSphere sSphere;

        public Vector3 Center
        {
            get { return sSphere.Center; }
            set { sSphere.Center = value; }
        }

        public float Radius
        {
            get { return sSphere.Radius; }
            set { sSphere.Radius = value; }
        }

        public override Vector3 Support(Vector3 dir, float dt)
        {
            dir.Normalize();
            return Center + dir * Radius;
        }

        public static BoundingSphere Merge(BoundingSphere first, BoundingSphere second)
        {
			if (first == null && second != null)
				return second;
			if (second == null && first != null)
				return first;

            return new BoundingSphere(SharpDX.BoundingSphere.Merge(first.sSphere, second.sSphere));
        }

        public static BoundingSphere FromPoints(Vector3[] points)
        {
            return new BoundingSphere(SharpDX.BoundingSphere.FromPoints(points));
        }

        public ContainmentType Contains(BoundingSphere other)
        {
            return sSphere.Contains(ref other.sSphere);
        }

        public ContainmentType Contains(Vector3 point)
        {
            return sSphere.Contains(ref point);
        }

        #region Test

        public override bool Test(BoundingSphere other)
        {
            return sSphere.Intersects(ref other.sSphere);
        }

        public override bool TestMoving(BoundingSphere other, Vector3 v0, Vector3 v1, out float t)
        {
            return DynamicIntersectionTests.Test(this, other, v0, v1, out t);
        }

        public override bool Test(AxisAlignedBoundingBox other)
        {
            return sSphere.Intersects(ref other.sbb);
        }

        public override bool Test(BoundingFrustum other)
        {
            return other.sbf.Intersects(ref this.sSphere);
        }

        public override bool Test(Line other)
        {
            return sSphere.Intersects(ref other.rn) || sSphere.Intersects(ref other.rp);
        }

        public override bool Test(Ray other)
        {
            return sSphere.Intersects(ref other.sray);
        }

        public override bool Test(Plane other)
        {
            return sSphere.Intersects(ref other.splane) == PlaneIntersectionType.Intersecting;
        }

        public override bool Test(LineSegment other)
        {
			return IntersectionTests.Test(other, this);
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

        #endregion

        #region Merge

        public override BoundingVolume Merge(BoundingSphere other)
        {
            return BoundingSphere.Merge(this, other);
        }

        public override BoundingVolume Merge(AxisAlignedBoundingBox other)
        {
            var otherS = SharpDX.BoundingSphere.FromBox(other.sbb);
            return new BoundingSphere(SharpDX.BoundingSphere.Merge(this.sSphere, otherS));
        }

        public override BoundingVolume Merge(BoundingFrustum other)
        {
            var otherS = SharpDX.BoundingSphere.FromPoints(other.sbf.GetCorners());
            return new BoundingSphere(SharpDX.BoundingSphere.Merge(this.sSphere, otherS));
        }

        public override BoundingVolume Merge(Triangle other)
        {
            var otherS = SharpDX.BoundingSphere.FromPoints(new[] { other.P1, other.P2, other.P3 });
            return new BoundingSphere(SharpDX.BoundingSphere.Merge(this.sSphere, otherS));
        }

        #endregion
    }
}
