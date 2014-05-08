using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics.RenderCommands
{
    class RenderDeferredIndexedCommand<T> : RenderIndexedCommand<T>
        where T : struct
    {
        public RenderDeferredIndexedCommand(
            Effect effect,
            PrimitiveType primitiveType,
            SharpDX.Toolkit.Graphics.Buffer<T> vertexBuffer,
            int numVertices,
            SharpDX.Toolkit.Graphics.Buffer indexBuffer,
            int numIndices,
            bool is32bit,
            Dictionary<string, GraphicsResource> resources,
            Matrix world,
            BlendState blend = null,
            DepthStencilState depthStencil = null,
            RasterizerState rasterizer = null,
            string technique = null,
            int startIndex = 0)
            : base(effect, primitiveType, vertexBuffer, numVertices, indexBuffer, numIndices, is32bit, resources, world, blend, depthStencil, rasterizer, technique)
        {

        }
    }
}
