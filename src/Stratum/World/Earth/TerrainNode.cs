using BruTile;
using SharpDX;
using Stratum.GIS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.World.Earth
{
    /// <summary>
    /// A node in a Terrain Quadtree
    /// </summary>
    public class TerrainNode
    {
        public static readonly int maxSplit = 22;
        public static readonly double splitFactor = 1.2; // must be greater than 1 for a restricted quadtree

        protected int splitDepth;
        protected TerrainNode[] children;

        //protected TerrainVertex[] vertices;

        protected LatLon[] corners;
        protected LatLon center;
        protected Extent latLonExtents;
        protected Extent meterExtents;

        protected BoundingBox aabb;
        protected BoundingBox splitBox;

        public TerrainNode(TerrainNode parent, Quadrant quad, LatLon bl, LatLon tr)
        {
            Parent = parent;
            Quadrant = quad;

            corners = new LatLon[4]
            {
                bl, new LatLon(tr.Latitude, bl.Longitude), tr, new LatLon(bl.Latitude, tr.Longitude)
            };

            latLonExtents = new Extent(BL.Longitude, BL.Latitude, TR.Longitude, TR.Latitude);

            var mercBL = RenderWGS84.ToGoogleBing(BL.Longitude, BL.Latitude);
            var mercTR = RenderWGS84.ToGoogleBing(TR.Longitude, TR.Latitude);
            meterExtents = new Extent(mercBL.X, mercBL.Y, mercTR.X, mercTR.Y);

            center = LatLon.Average(corners);
            splitDepth = 0;

            Vector3D[] transformed = transformCorners();
            this.aabb = BoundingBox.FromPoints(transformed.Select(vec => vec.ToVector3()).ToArray());
            generateSplitBox();
        }

        public TerrainNode(TerrainNode parent, Quadrant quad, LatLon bl, LatLon tl, LatLon tr, LatLon br)
        {
            Parent = parent;
            Quadrant = quad;

            corners = new LatLon[4] 
            {
                bl, tl, tr, br
            };

            latLonExtents = new Extent(BL.Longitude, BL.Latitude, TR.Longitude, TR.Latitude);

            var mercBL = RenderWGS84.ToGoogleBing(BL.Longitude, BL.Latitude);
            var mercTR = RenderWGS84.ToGoogleBing(TR.Longitude, TR.Latitude);
            meterExtents = new Extent(mercBL.X, mercBL.Y, mercTR.X, mercTR.Y);

            center = LatLon.Average(corners);
            splitDepth = 0;

            Vector3D[] transformed = transformCorners();
            this.aabb = BoundingBox.FromPoints(transformed.Select(vec => vec.ToVector3()).ToArray());
            generateSplitBox();
        }

        protected TerrainNode(TerrainNode parent, Quadrant quad, LatLon bl, LatLon tl, LatLon tr, LatLon br, int splitD)
            : this(parent, quad, bl, tl, tr, br)
        {
            this.splitDepth = splitD;
        }

        public TerrainNode Parent { get; private set; }

        public BoundingBox AABB { get { return aabb; } }

        public Quadrant Quadrant { get; private set; }

        public LatLon BL { get { return corners[0]; } }
        public LatLon TL { get { return corners[1]; } }
        public LatLon TR { get { return corners[2]; } }
        public LatLon BR { get { return corners[3]; } }
        public LatLon Center { get { return center; } }

        public Extent LatLonExtents { get { return latLonExtents; } }
        public Extent MeterExtents { get { return meterExtents; } }

        public bool IsSplit { get; protected set; }

        public int Depth { get { return splitDepth; } }

        public TerrainNode[] Children { get { return children; } }

        public TerrainVertex[] Geometry
        {
            get
            {
                var rc = Engine.GraphicsContext.RenderContext;
                var vertices = new TerrainVertex[4];

                var corners = transformCorners();
                var viewCorners = corners.Select(c => Vector3D.Transform(c, rc.ViewProjD)).ToArray();

                vertices[0] = new TerrainVertex(viewCorners[0], new Vector2(0, 1));
                vertices[1] = new TerrainVertex(viewCorners[1], new Vector2(0, 0));
                vertices[2] = new TerrainVertex(viewCorners[2], new Vector2(1, 0));
                vertices[3] = new TerrainVertex(viewCorners[3], new Vector2(1, 1));

                return vertices;
            }
        }

        public bool IsVisible(Camera camera)
        {
            return camera.Frustum.Intersects(ref aabb);
        }

        // defaults to bing tile scheme where tiles are 256 X 256
        public virtual double GroundResolution()
        {
            return meterExtents.Width / 256.0;
        }

        public virtual void Update(Camera camera)
        {
            var pos = camera.Position;
            if (IsSplit)
            {
                if (splitBox.Contains(ref pos) == ContainmentType.Disjoint)
                {
                    Collapse();
                }
                else
                {
                    // recursive call to children
                    foreach (var child in children)
                        child.Update(camera);
                }
            }
            else
            {
                if (splitBox.Contains(ref pos) != ContainmentType.Disjoint)
                {
                    Split();

                    if (IsSplit)
                    {
                        // recursive call to children
                        foreach (var child in children)
                            child.Update(camera);
                    }
                }
            }
        }

        protected virtual void Split()
        {
            if (this.splitDepth <= maxSplit)
            {
                if (children == null)
                {
                    LatLon leftMidpoint = LatLon.Average(BL, TL);
                    LatLon rightMidpoint = LatLon.Average(BR, TR);
                    LatLon topMidpoint = LatLon.Average(TL, TR);
                    LatLon bottomMidpoint = LatLon.Average(BL, BR);

                    TerrainNode bl = new TerrainNode(this, Quadrant.BL, BL, leftMidpoint, center, bottomMidpoint);
                    TerrainNode tl = new TerrainNode(this, Quadrant.TL, leftMidpoint, TL, topMidpoint, center);
                    TerrainNode tr = new TerrainNode(this, Quadrant.TR, center, topMidpoint, TR, rightMidpoint);
                    TerrainNode br = new TerrainNode(this, Quadrant.BR, bottomMidpoint, center, rightMidpoint, BR);
                    children = new TerrainNode[4] 
                    {
                        bl, tl, tr, br
                    };

                    foreach (var child in children)
                        child.splitDepth = this.splitDepth + 1;
                }

                IsSplit = true;
            }
        }

        protected virtual void Collapse()
        {
            IsSplit = false;
        }

        /// <summary>
        /// Transforms the corners of this node to world space
        /// </summary>
        /// <returns></returns>
        protected virtual Vector3D[] transformCorners()
        {
            Vector3D[] transformed = new Vector3D[4];

            Vector3D[] cartesianCorners = corners.Select(latlon => RenderWGS84.ToWorld(latlon.Latitude, latlon.Longitude)).ToArray();
            for (int i = 0; i < cartesianCorners.Length; i++)
            {
                Vector3D normalized;
                Vector3D.Normalize(ref cartesianCorners[i], out normalized);
                transformed[i] = normalized * RenderWGS84.EarthRadius;
            }

            return transformed;
        }

        /// <summary>
        /// Generates the axis aligned bounding box used as the split criteria 
        /// </summary>
        protected virtual void generateSplitBox()
        {
            //splitBox = new BoundingSphere(aabb.Center, aabb.Radius * (float)splitFactor);

            Vector3 boxCenter = aabb.Minimum + aabb.Maximum;
            boxCenter *= 0.5f;

            float halfx = boxCenter.X - aabb.Minimum.X;
            float halfy = boxCenter.Y - aabb.Minimum.Y;
            float halfz = boxCenter.Z - aabb.Minimum.Z;

            float maxHalf = Math.Max(halfx, Math.Max(halfy, halfz));

            Vector3 newMin = boxCenter - new Vector3(maxHalf * 2f * (float)splitFactor);
            Vector3 newMax = boxCenter + new Vector3(maxHalf * 2f * (float)splitFactor);

            splitBox = new BoundingBox(newMin, newMax);
        }
    }
}
