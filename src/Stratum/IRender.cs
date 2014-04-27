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
        void Render(GameTime gameTime, IGraphicsContext context);
        void RenderDebug(GameTime gameTime, IGraphicsContext context);
    }
}
