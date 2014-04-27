using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public class AxisAlignedBoundingBox : BoundingVolume
    {
        public AxisAlignedBoundingBox()
        {
        }

        public AxisAlignedBoundingBox(SharpDX.BoundingBox bb)
        {
            this.sbb = bb;
        }

        public AxisAlignedBoundingBox(Vector3 min, Vector3 max)
        {
            this.sbb = new SharpDX.BoundingBox(min, max);
        }
        
        internal SharpDX.BoundingBox sbb;

        public Vector3 Min
        {
            get { return sbb.Minimum; }
            set { sbb.Minimum = value; }
        }

        public Vector3 Max
        {
            get { return sbb.Maximum; }
            set { sbb.Maximum = value; }
        }

		private Vector3? mid;
		public Vector3 Mid
		{
			get
			{
				if (mid == null)
					mid = Min + ((Max - Min) / 2);
				return mid.Value;
			}
		}

		public float XSpan { get { return Max.X - Min.X; } }
		public float YSpan { get { return Max.Y - Min.Y; } }
		public float ZSpan { get { return Max.Z - Min.Z; } }

		private float? minSide;
		public float MinSide
		{
			get
			{
				if (minSide == null)
					minSide = Math.Min(Math.Min(XSpan, YSpan), ZSpan);
				return minSide.Value;
			}
		}

		public float Width { get { return Max.X - Min.X; } }
		public float Height { get { return Max.Y - Min.Y; } }
		public float Depth { get { return Max.Z - Min.Z; } }

        public ContainmentType Contains(AxisAlignedBoundingBox other)
        {
            return sbb.Contains(ref other.sbb);
        }

		public ContainmentType Contains(BoundingSphere other)
		{
			return sbb.Contains(ref other.sSphere);
		}

		public ContainmentType Contains(BoundingBox bb)
		{
			return sbb.Contains(ref bb);
		}

		public ContainmentType Contains(SharpDX.BoundingSphere bs)
		{
			return sbb.Contains(ref bs);
		}

		public BoundingBox[] Subdivide()
		{
			Vector3 middle = new Vector3(Min.X + Width / 2, Min.Y + Height / 2, Min.Z + Depth / 2);

			// these indices coincide with the indices returned by GetIndex
			BoundingBox[] ret = new BoundingBox[8];
			ret[7] = new BoundingBox(Min, middle);
			ret[6] = new BoundingBox(new Vector3(Min.X, Min.Y, middle.Z), new Vector3(middle.X, middle.Y, Max.Z));
			ret[2] = new BoundingBox(new Vector3(middle.X, Min.Y, middle.Z), new Vector3(Max.X, middle.Y, Max.Z));
			ret[3] = new BoundingBox(new Vector3(middle.X, Min.Y, Min.Z), new Vector3(Max.X, middle.Y, middle.Z));

			ret[5] = new BoundingBox(new Vector3(Min.X, middle.Y, Min.Z), new Vector3(middle.X, Max.Y, middle.Z));
			ret[4] = new BoundingBox(new Vector3(Min.X, middle.Y, middle.Z), new Vector3(middle.X, Max.Y, Max.Z));
			ret[0] = new BoundingBox(middle, Max);
			ret[1] = new BoundingBox(new Vector3(middle.X, middle.Y, Min.Z), new Vector3(Max.X, Max.Y, middle.Z));

			return ret;
		}

		/// <summary>
		/// Returns the index into the array of child bounding boxes that result from subdividing 
		/// this AABB for the given position
		/// </summary>
		/// <param name="position"></param>
		/// <returns></returns>
		public int GetIndex(Vector3 position)
		{
			// 000 = rtb
			// 001 = rtf
			// 010 = rbb
			// 011 = rbf
			// 100 = ltb
			// 101 = ltf
			// 110 = lbb
			// 111 = lbf

			Vector3 mid = Mid;
			int xyz = 0;
			xyz |= position.X < mid.X ? 1 : 0;
			xyz |= position.Y < mid.Y ? 2 : 0;
			xyz |= position.Z < mid.Z ? 4 : 0;
			return xyz;
		}

        public override Vector3 Support(Vector3 dir, float dt)
        {
            Vector3[] verts = new Vector3[8]
            {
                Min,
                new Vector3(Min.X, Min.Y, Max.Z),
                new Vector3(Max.X, Min.Y, Max.Z),
                new Vector3(Max.X, Min.Y, Min.Z),
                new Vector3(Min.X, Max.Y, Min.Z),
                new Vector3(Min.X, Max.Y, Max.Z),
                Max,
                new Vector3(Max.X, Max.Y, Min.Z)
            };

            Vector3 center = new Vector3((Max.X - Min.X) / 2f, (Max.Y - Min.Y) / 2f, (Max.Z - Min.Z) / 2f);

            double maxDot = 0;
            int vertIndex = -1;
            for (int i = 0; i < 8; i++)
            {
                float dot = Vector3.Dot(verts[i] - center, dir);
                if (dot > maxDot)
                {
                    maxDot = dot;
                    vertIndex = i;
                }
            }

            return verts[vertIndex];
        }

        #region Test

        public override bool Test(BoundingSphere other)
        {
            return sbb.Intersects(ref other.sSphere);
        }

        public override bool TestMoving(BoundingSphere other, Vector3 v0, Vector3 v1, out float t)
        {
            throw new NotImplementedException();
        }

        public override bool Test(AxisAlignedBoundingBox other)
        {
            return sbb.Intersects(ref other.sbb);
        }

        public override bool Test(BoundingFrustum other)
        {
            return other.sbf.Intersects(ref sbb);
        }

        public override bool Test(Line other)
        {
            return sbb.Intersects(ref other.rn) || sbb.Intersects(ref other.rp);
        }

        public override bool Test(Ray other)
        {
            return sbb.Intersects(ref other.sray);
        }

        public override bool Test(Plane other)
        {
            return sbb.Intersects(ref other.splane) == PlaneIntersectionType.Intersecting;
        }

		public bool Test(SharpDX.Plane other)
		{
			return sbb.Intersects(ref other) == PlaneIntersectionType.Intersecting;
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

        public static AxisAlignedBoundingBox FromPoints(Vector3[] points)
        {
            return new AxisAlignedBoundingBox(BoundingBox.FromPoints(points)); 
        }

        public static AxisAlignedBoundingBox FromSphere(BoundingSphere sphere)
        {
            return new AxisAlignedBoundingBox(BoundingBox.FromSphere(sphere.sSphere));
        }

        #region Merge

        public static AxisAlignedBoundingBox Merge(AxisAlignedBoundingBox first, AxisAlignedBoundingBox second)
        {
            return new AxisAlignedBoundingBox(BoundingBox.Merge(first.sbb, second.sbb));
        }

        public override BoundingVolume Merge(BoundingSphere other)
        {
            var otherB = SharpDX.BoundingBox.FromSphere(other.sSphere);
            return new AxisAlignedBoundingBox(SharpDX.BoundingBox.Merge(this.sbb, otherB));
        }

        public override BoundingVolume Merge(AxisAlignedBoundingBox other)
        {
            return AxisAlignedBoundingBox.Merge(this, other);
        }

        public override BoundingVolume Merge(BoundingFrustum other)
        {
            var otherB = SharpDX.BoundingBox.FromPoints(other.sbf.GetCorners());
            return new AxisAlignedBoundingBox(SharpDX.BoundingBox.Merge(this.sbb, otherB));
        }

        public override BoundingVolume Merge(Triangle other)
        {
            var otherB = SharpDX.BoundingBox.FromPoints(new[] { other.P1, other.P2, other.P3 });
            return new AxisAlignedBoundingBox(SharpDX.BoundingBox.Merge(this.sbb, otherB));
        }

        #endregion
    }
}
