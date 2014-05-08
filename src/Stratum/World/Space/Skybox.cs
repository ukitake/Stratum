using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stratum.Graphics;

namespace Stratum
{
    public struct SkyboxVertex
    {
        public SkyboxVertex(float x, float y, float z)
        {
            Position = new Vector3(x, y, z);
        }

        [VertexElement("POSITION")]
        public Vector3 Position;
    }

    public class Skybox : IRender
    {
        public Skybox(GraphicsDevice device)
        {
            float x = 0.525731f;
            float z = 0.850651f;

            var vertices = new SkyboxVertex[12]
            {
                new SkyboxVertex(-x, 0f, z), new SkyboxVertex(x, 0f, z),
                new SkyboxVertex(-x, 0f, -z), new SkyboxVertex(x, 0f, -z),
                new SkyboxVertex(0f, z, x), new SkyboxVertex(0f, z, -x),
                new SkyboxVertex(0f, -z, x), new SkyboxVertex(0f, -z, -x),
                new SkyboxVertex(z, x, 0f), new SkyboxVertex(-z, x, 0f),
                new SkyboxVertex(z, -x, 0f), new SkyboxVertex(-z, -x, 0f),
            };

            var indices = new int[60]
            {
                1,4,0,  4,9,0,  4,5,9,  8,5,4,  1,8,4,
                1,10,8, 10,3,8, 8,3,5,  3,2,5,  3,7,2,
                3,10,7, 10,6,7, 6,11,7, 6,0,11, 6,1,0,
                10,1,6, 11,0,9, 2,11,9, 5,2,9,  11,2,7
            };

            vertexBuffer = Buffer<SkyboxVertex>.New(device, vertices, BufferFlags.VertexBuffer);
            indexBuffer = Buffer<int>.New(device, indices, BufferFlags.IndexBuffer);
            skyboxEffect = EffectLoader.Load(@"Graphics/Shaders/Skybox.fx");
            skyboxTex = Texture2D.Load(device, @"G:\Users\Athrun\Documents\Stratum\trunk\src\Stratum\WorldEngine\Earth\milkyWay.tif");
        }

        private Buffer<SkyboxVertex> vertexBuffer;
        private Buffer<int> indexBuffer;
        private Effect skyboxEffect;

        private Texture2D skyboxTex;

        public void QueueRenderCommands(GameTime gameTime, Renderer renderer, IGraphicsContext context)
        {
            context.Device.SetVertexBuffer(vertexBuffer);
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vertexBuffer));
            context.Device.SetIndexBuffer(indexBuffer, true);

            context.Device.SetDepthStencilState(context.Device.DepthStencilStates.None);
            context.Device.SetBlendState(context.Device.BlendStates.Opaque);
            context.Device.SetRasterizerState(context.Device.RasterizerStates.CullNone);

            Matrix specialProj = Matrix.PerspectiveFovLH(MathUtil.PiOverFour, context.AspectRatio, 0.1f, 2f);

            skyboxEffect.Parameters["World"].SetValue(Matrix.Identity);
            skyboxEffect.Parameters["View"].SetValue(context.CurrentCamera.View);
            Matrix viewNoT = context.CurrentCamera.View;
            viewNoT.TranslationVector = Vector3.Zero;
            skyboxEffect.Parameters["ViewNoT"].SetValue(viewNoT);
            skyboxEffect.Parameters["Proj"].SetValue(specialProj);
            skyboxEffect.Parameters["skyboxTex"].SetResource(skyboxTex);
            skyboxEffect.Parameters["skyboxSampler"].SetResource(context.Device.SamplerStates.LinearClamp);

            skyboxEffect.CurrentTechnique.Passes[0].Apply();
            context.Device.DrawIndexed(PrimitiveType.PatchList(3), indexBuffer.ElementCount);
        }


        public void RenderDebug(GameTime gameTime, IGraphicsContext context)
        {
            throw new NotImplementedException();
        }


        public void RenderDeferred(GameTime gameTime, IGraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
