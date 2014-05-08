using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics.RenderCommands
{
    class RenderInstancedCommand<T, I> : RenderCommand<T>
        where T : struct
        where I : struct
    {
        public int InstanceCount { get; set; }
        public SharpDX.Toolkit.Graphics.Buffer<I> InstanceData { get; set; } 

        public RenderInstancedCommand(
            Effect effect,
            PrimitiveType primitiveType,
            SharpDX.Toolkit.Graphics.Buffer<T> vertexBuffer,
            int numVertices,
            SharpDX.Toolkit.Graphics.Buffer<I> instanceData,
            int instanceCount,
            Dictionary<string, GraphicsResource> resources,
            Matrix world,
            BlendState blend = null,
            DepthStencilState depthStencil = null,
            RasterizerState rasterizer = null,
            string technique = null)
            : base(effect, primitiveType, vertexBuffer, numVertices, resources, world, blend, depthStencil, rasterizer, technique)
        {
            this.InstanceData = instanceData;
            this.InstanceCount = instanceCount;
        }

        public override void SetGeometry(IGraphicsContext context)
        {
            context.Device.SetVertexBuffer(0, VertexBuffer);
            context.Device.SetVertexBuffer(1, InstanceData);
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, VertexBuffer));
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(1, InstanceData));
        }

        public override void Render(IGraphicsContext context)
        {
            ApplyEffect();

            context.Device.DrawInstanced(PrimitiveType, NumVertices, InstanceCount, StartVertexLocation);
        }
    }
}
