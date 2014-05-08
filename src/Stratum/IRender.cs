using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit;
using Stratum.Graphics;

namespace Stratum
{
    public interface IRender
    {
        void QueueRenderCommands(GameTime gameTime, Renderer renderer, IGraphicsContext context);
    }
}
