using SharpDX;
using SharpDX.Toolkit.Graphics;
using Stratum.CompGeom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics.RenderCommands
{
    class RenderIndexedCommand<T> : RenderCommand<T>
        where T : struct
    {
        public SharpDX.Toolkit.Graphics.Buffer IndexBuffer { get; set; }
        public bool Is32Bit { get; set; }
        public int NumIndices { get; set; }
        public int StartIndexLocation { get; set; }

        public RenderIndexedCommand(
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
            : base(effect, primitiveType, vertexBuffer, numVertices, resources, world, blend, depthStencil, rasterizer, technique)
        {
            this.IndexBuffer = indexBuffer;
            this.NumIndices = numIndices;
            this.Is32Bit = is32bit;
            this.StartIndexLocation = startIndex;
        }

        public override void SetGeometry(IGraphicsContext context)
        {
            // set the vertex input layout and vertex buffer
            base.SetGeometry(context);

            context.Device.SetIndexBuffer(IndexBuffer, Is32Bit);
        }

        public override void Render(IGraphicsContext context)
        {
            ApplyEffect();

            // issue the draw call
            context.Device.DrawIndexed(PrimitiveType, NumIndices, StartIndexLocation, StartVertexLocation);
        }
    }
}
