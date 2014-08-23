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

        public override void SetConstants(IGraphicsContext context)
        {
            base.SetConstants(context);
            var camera = context.CurrentCamera;

            Effect.Parameters["wPosCamera"].SetValue(camera.Position);
            Effect.Parameters["near_plane"].SetValue((float)camera.NearPlane);
            Effect.Parameters["far_plane"].SetValue((float)camera.FarPlane);
            Effect.Parameters["fov"].SetValue((float)camera.FieldOfView);

            Vector3[] frustumCornersWS = new Vector3[8];
            Vector4[] frustumCornersVS = new Vector4[8];
            Vector4[] farFrustumCornersVS = new Vector4[4];
            BoundingFrustum frustum = new BoundingFrustum(context.RenderContext.ViewProjD.ToMatrix());
            frustum.GetCorners(frustumCornersWS);

            var view = camera.ViewD.ToMatrix();
            // TODO: take out the translation part of the view matrix
            Vector3.Transform(frustumCornersWS, ref view, frustumCornersVS);

            //  2 ____________  1
            //   |            |
            //   |            |
            //   |            |
            //  3|____________| 0
            //
            for (int i = 4; i < 8; i++)
            {
                farFrustumCornersVS[i - 4] = frustumCornersVS[i];
            }

            Effect.Parameters["frustumCornersVS"].SetValue(farFrustumCornersVS);
        }
    }
}
