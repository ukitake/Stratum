using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.World.Earth
{
    public struct TerrainVertex
    {
        public TerrainVertex(Vector3D pos, Vector2 tex, float splitDepth)
        {
            Vector3 high, low;
            pos.Split(out low, out high);

            PositionHigh = high;
            PositionLow = low;
            TexCoord = tex;
            Depth = splitDepth;
        }

        [VertexElement("POSITION0")]
        public Vector3 PositionHigh;

        [VertexElement("POSITION1")]
        public Vector3 PositionLow;

        [VertexElement("TEXCOORD0")]
        public Vector2 TexCoord;

        [VertexElement("TEXCOORD1")]
        public float Depth;
    }
}
