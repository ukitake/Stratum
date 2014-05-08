using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stratum.Graphics;

namespace Stratum.WorldEngine
{
    public class Sun : IRender
    {
        private GraphicsDevice device;

        public Sun()
        {
            device = Engine.GraphicsContext.Device;
        }

        public void QueueRenderCommands(GameTime gameTime, Renderer renderer, IGraphicsContext context)
        {
            
        }
    }
}
