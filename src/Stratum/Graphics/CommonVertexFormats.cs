using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics
{
    public struct QuadVert
    {
        public QuadVert(float x, float y, float z)
        {
            Pos = new Vector3(x, y, z);
        }

        [VertexElement("POSITION")]
        public Vector3 Pos;
    }


}
