using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Stratum.Graphics
{
    public class DirectionalLight
    {
        public DirectionalLight()
        {
            Direction = new Vector3(0, -1, 0);
            Color = SharpDX.Color.White.ToVector4();
        }

        public DirectionalLight(Vector3 dir, Color color)
        {
            Direction = dir;
            Color = color.ToVector4();
        }

        public Vector3 Direction { get; set; }
        public Vector4 Color { get; set; }
    }
}
