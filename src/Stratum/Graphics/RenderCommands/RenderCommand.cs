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
    class RenderCommand<T> : IRenderCommand
        where T : struct
    {
        public RenderCommand(
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
        {
            var device = Engine.GraphicsContext.Device;

            this.VertexBuffer = vertexBuffer;
            this.NumVertices = numVertices;
            this.Resources = resources;
            this.PrimitiveType = primitiveType;
            this.BlendState = blend ?? device.BlendStates.Default;
            this.DepthStencilState = depthStencil ?? device.DepthStencilStates.Default;
            this.RasterizerState = rasterizer ?? device.RasterizerStates.Default;
            this.World = world;
            this.Effect = effect;
            this.Technique = technique;
        }

        public Matrix World { get; set; }

        public PrimitiveType PrimitiveType { get; set; }
        public Effect Effect { get; set; }
        public string Technique { get; set; }

        public BlendState BlendState { get; set; }
        public DepthStencilState DepthStencilState { get; set; }
        public RasterizerState RasterizerState { get; set; }

        public SharpDX.Toolkit.Graphics.Buffer<T> VertexBuffer { get; set; }
        public int NumVertices { get; set; }
        public int StartVertexLocation { get; set; }

        public Dictionary<string, GraphicsResource> Resources { get; set; } 

        public virtual void SetGeometry(IGraphicsContext context)
        {
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer<T>(0, VertexBuffer));
            context.Device.SetVertexBuffer(0, VertexBuffer);
        }

        public virtual void SetResources(IGraphicsContext context)
        {
            foreach (var resource in Resources)
                Effect.Parameters[resource.Key].SetResource(resource.Value);
        }

        public virtual void SetConstants(IGraphicsContext context)
        {
            var rContext = context.RenderContext;
            var device = context.Device;

            // Dev Note: enforcing convention here... Effects set on the RenderCommand must
            // have constant buffer parameters with these names and types
            Effect.Parameters["World"].SetValue(World);
            Effect.Parameters["View"].SetValue(rContext.View);
            Effect.Parameters["ViewNoT"].SetValue(rContext.ViewNoTrans);
            Effect.Parameters["Proj"].SetValue(rContext.Proj);
            Effect.Parameters["ViewProj"].SetValue(rContext.ViewProj);

            var camPos = context.CurrentCamera.PositionD;
            Vector3 low, high;
            camPos.Split(out low, out high);

            Effect.Parameters["GlobalTime"].SetValue((float)rContext.Time.TotalGameTime.TotalSeconds); // todo: needs double precision
            Effect.Parameters["CameraPosition"].SetValue(context.CurrentCamera.Position);
            Effect.Parameters["CameraPositionLow"].SetValue(low);
            Effect.Parameters["CameraPositionHigh"].SetValue(high);
            Effect.Parameters["ViewportSize"].SetValue(new Vector2(device.Viewport.Width, device.Viewport.Height));
        }

        protected virtual void ApplyEffect()
        {
            if (!string.IsNullOrEmpty(Technique))
                Effect.CurrentTechnique = Effect.Techniques[Technique];
            
            Effect.CurrentTechnique.Passes[0].Apply();
        }

        public virtual void Render(IGraphicsContext context)
        {
            // set the technique and apply the effect
            ApplyEffect();

            // issue the draw call
            context.Device.Draw(PrimitiveType, NumVertices, StartVertexLocation);
        }
    }
}
