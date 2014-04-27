using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public abstract class BoundingVolume
    {
        public BoundingVolume()
        { }

        /// <summary>
        /// Uses double dispatch to polymorphically test two arbitrary bounding volumes
        /// </summary>
        /// <param name="visitor">'visitor' in the visitor pattern</param>
        /// <returns>bool indicated whether the bounding volumes intersect or not</returns>
        public virtual bool Test(BoundingVolume visitor)
        {
            return visitor.Test(this);
        }

        public virtual bool TestMoving(BoundingVolume visitor, Vector3 v0, Vector3 v1, out float t)
        {
            return visitor.TestMoving(this, v0, v1, out t);
        }

        public virtual bool Test(Primitive visitor)
        {
            return visitor.Test(this);
        }

        public virtual BoundingVolume Merge(BoundingVolume other)
        {
            return other.Merge(this);
        }

        public abstract bool Test(BoundingSphere other);
        public abstract bool TestMoving(BoundingSphere other, Vector3 v0, Vector3 v1, out float t);
        public abstract bool Test(AxisAlignedBoundingBox other);
        public abstract bool Test(BoundingFrustum other);
        public abstract bool Test(Line other);
        public abstract bool Test(Ray other);
        public abstract bool Test(Plane other);
        public abstract bool Test(LineSegment other);
		public abstract bool Test(Triangle other);
		public abstract bool Test(Cylinder other);

        public abstract Vector3 Support(Vector3 dir, float dt);

        public abstract BoundingVolume Merge(BoundingSphere other);
        public abstract BoundingVolume Merge(AxisAlignedBoundingBox other);
        public abstract BoundingVolume Merge(BoundingFrustum other);
        public abstract BoundingVolume Merge(Triangle other);
        
    }
}
