using SharpDX;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics.RenderCommands
{
    class RenderIndexedInstancedCommand<T, I> : RenderIndexedCommand<T>
        where T : struct
        where I : struct
    {
        public int InstanceCount { get; set; }
        public SharpDX.Toolkit.Graphics.Buffer<I> InstanceData { get; set; } 

        public RenderIndexedInstancedCommand(
            Effect effect,
            PrimitiveType primitiveType,
            SharpDX.Toolkit.Graphics.Buffer<T> vertexBuffer,
            int numVertices,
            SharpDX.Toolkit.Graphics.Buffer<I> instanceData,
            int numInstances,
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
            InstanceCount = numInstances;
            InstanceData = instanceData;
        }

        public override void SetGeometry(IGraphicsContext context)
        {
            context.Device.SetIndexBuffer(IndexBuffer, Is32Bit);

            context.Device.SetVertexBuffer(0, VertexBuffer);
            context.Device.SetVertexBuffer(1, InstanceData);
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, VertexBuffer));
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(1, InstanceData));
        }

        public override void Render(IGraphicsContext context)
        {
            ApplyEffect();

            context.Device.DrawIndexedInstanced(PrimitiveType, NumIndices, InstanceCount, StartIndexLocation);
        }
    }
}
