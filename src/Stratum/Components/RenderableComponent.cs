using SharpDX.Toolkit;
using Stratum.Graphics;
using Stratum.Graphics.RenderCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Components
{
    public abstract class RenderableComponent : Component
    {
        protected abstract IRenderCommand GenerateRenderCommand();

        public override void QueueRenderCommands(GameTime gameTime, Renderer renderer, IGraphicsContext context)
        {
            var command = GenerateRenderCommand();

            if (command is IRenderDeferredCommand)
            {
                Engine.Renderer.EnqueueDeferred(command as IRenderDeferredCommand);
            }
            else
            {
                Engine.Renderer.EnqueueNormal(command);
            }
        }
    }
}
