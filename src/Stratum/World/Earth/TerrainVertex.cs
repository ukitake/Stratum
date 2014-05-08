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
        public TerrainVertex(Vector4D pos, Vector2 tex)
        {
            Position = pos.ToVector4();
            TexCoord = tex;
        }

        [VertexElement("POSITION")]
        public Vector4 Position;

        [VertexElement("TEXCOORD0")]
        public Vector2 TexCoord;
    }
}
