using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics.RenderCommands
{
    class RenderDeferredCommand<T> : RenderCommand<T>, IRenderDeferredCommand
        where T : struct
    {
        public RenderDeferredCommand(
            Effect effect,
            PrimitiveType primitiveType,
            SharpDX.Toolkit.Graphics.Buffer<T> vertexBuffer,
            int numVertices,
            Dictionary<string, GraphicsResource> resources,
            Matrix world,
            BlendState blend = null,
            DepthStencilState depthStencil = null,
            RasterizerState rasterizer = null,
            string technique = null)
            : base(effect, primitiveType, vertexBuffer, numVertices, resources, world, blend, depthStencil, rasterizer, technique)
        {

        }
    }
}
