using SharpDX;
using Stratum.GIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.World.Earth
{
    public class CesiumTerrainNode : TerrainNode
    {
        public CesiumTerrainNode(CesiumTerrainNode parent, Quadrant quad, LatLon bl, LatLon tl, LatLon tr, LatLon br)
            : base(parent, quad, bl, tl, tr, br)
        {
        }

        public CesiumTerrainNode(CesiumTerrainNode parent, Quadrant quad, LatLon bl, LatLon tr)
            : base(parent, quad, bl, tr)
        {

        }

        protected override void Split()
        {
            if (this.splitDepth <= maxSplit)
            {
                if (children == null)
                {
                    LatLon leftMidpoint = LatLon.Average(BL, TL);
                    LatLon rightMidpoint = LatLon.Average(BR, TR);
                    LatLon topMidpoint = LatLon.Average(TL, TR);
                    LatLon bottomMidpoint = LatLon.Average(BL, BR);

                    CesiumTerrainNode bl = new CesiumTerrainNode(this, Quadrant.BL, BL, leftMidpoint, center, bottomMidpoint);
                    CesiumTerrainNode tl = new CesiumTerrainNode(this, Quadrant.TL, leftMidpoint, TL, topMidpoint, center);
                    CesiumTerrainNode tr = new CesiumTerrainNode(this, Quadrant.TR, center, topMidpoint, TR, rightMidpoint);
                    CesiumTerrainNode br = new CesiumTerrainNode(this, Quadrant.BR, bottomMidpoint, center, rightMidpoint, BR);
                    children = new CesiumTerrainNode[4] 
                    {
                        bl, tl, tr, br
                    };

                    bl.splitDepth = this.splitDepth + 1;
                    tl.splitDepth = this.splitDepth + 1;
                    tr.splitDepth = this.splitDepth + 1;
                    br.splitDepth = this.splitDepth + 1;
                }

                IsSplit = true;
            }
        }

        protected override void generateSplitBox()
        {
            if (splitDepth == 0)
            {
                // special case for splitdepth 0 

                Vector3 boxCenter = aabb.Minimum + aabb.Maximum;
                boxCenter *= 0.5f;

                float halfx = boxCenter.X - aabb.Minimum.X;
                float halfy = boxCenter.Y - aabb.Minimum.Y;
                float halfz = boxCenter.Z - aabb.Minimum.Z;

                float maxHalf = Math.Max(halfx, Math.Max(halfy, halfz));

                // adjust bounding box to not be flat
                aabb = new BoundingBox(boxCenter - new Vector3(maxHalf * 2f), boxCenter + new Vector3(maxHalf * 2f));

                Vector3 newMin = boxCenter - new Vector3(maxHalf * 2f * (float)splitFactor);
                Vector3 newMax = boxCenter + new Vector3(maxHalf * 2f * (float)splitFactor);

                splitBox = new BoundingBox(newMin, newMax);
            }
            else
            {
                base.generateSplitBox();
            }
        }
    }
}
