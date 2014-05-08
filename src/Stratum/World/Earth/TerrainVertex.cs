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
        public TerrainVertex(float x, float y, float z, float u, float v)
        {
            Position = new Vector3(x, y, z);
            TexCoord = new Vector2(u, v);
        }

        public TerrainVertex(Vector3 pos, Vector2 tex)
        {
            Position = pos;
            TexCoord = tex;
        }

        public TerrainVertex(double x, double y, double z, double u, double v)
        {
            Position = new Vector3((float)x, (float)y, (float)z);
            TexCoord = new Vector2((float)u, (float)v);
        }

        public TerrainVertex(Vector3D pos, Vector2 tex)
        {
            Position = pos.ToVector3();
            TexCoord = tex;
        }

        [VertexElement("POSITION")]
        public Vector3 Position;

        [VertexElement("TEXCOORD0")]
        public Vector2 TexCoord;
    }
}
