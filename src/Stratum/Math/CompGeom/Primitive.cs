using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.CompGeom
{
    public abstract class Primitive
    {
        public Primitive()
        { }

        public virtual bool Test(BoundingVolume visitor)
        {
            return visitor.Test(this);
        }

        public virtual bool Test(Primitive visitor)
        {
            return visitor.Test(this);
        }

        public abstract bool Test(AxisAlignedBoundingBox other);
        public abstract bool Test(BoundingSphere other);
        public abstract bool Test(BoundingFrustum other);
        public abstract bool Test(Line other);
        public abstract bool Test(Ray other);
        public abstract bool Test(Plane other);
        public abstract bool Test(LineSegment other);
		public abstract bool Test(Triangle other);
		public abstract bool Test(Cylinder other);
    }
}
