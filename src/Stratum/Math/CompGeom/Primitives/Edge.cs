using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Stratum.CompGeom
{
	public class Edge
	{
		public Edge(Triangle right, Vector3 start, Vector3 end)
		{
			this.rightFace = right;
			this.start = start;
			this.end = end;
            this.dir = end - start;
		}

		public Vector3 start;
		public Vector3 end;
        public Vector3 dir;
		public Triangle leftFace;
		public Triangle rightFace;

        public Vector3 OtherPoint(Vector3 startOrEnd)
        {
            if (startOrEnd == start)
                return end;
            else if (startOrEnd == end)
                return start;
            else
                return Vector3.Zero;
        }

		public override int GetHashCode()
		{
			// overall hash function is:
			// H = (x1 + y1 + z1) ^ (x2 + y2 + z2)
 			// there is still a posibility for collision if the xyz components of the start and end points add up to the same two values
			return start.GetHashCode() ^ end.GetHashCode();
		}
	}
}
