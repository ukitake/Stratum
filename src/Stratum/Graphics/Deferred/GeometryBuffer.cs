using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Graphics;

namespace Stratum.Graphics
{
    public class GeometryBuffer
    {
        private struct LightQuadVertex
        {
            public LightQuadVertex(float x, float y, float z, float tx, float ty, float index)
            {
                Position = new Vector3(x, y, z);
                TexAndCornerIndex = new Vector3(tx, ty, index);
            }

            [VertexElement("POSITION")]
            public Vector3 Position;

            [VertexElement("TEXCOORD")]
            public Vector3 TexAndCornerIndex;
        }

        public GeometryBuffer(IGraphicsContext context)
        {
            this.context = context;

            int width = context.Device.BackBuffer.Width;
            int height = context.Device.BackBuffer.Height;
            DepthTarget = RenderTarget2D.New(context.Device, width, height, PixelFormat.R32.Float);
            NormalTarget = RenderTarget2D.New(context.Device, width, height, PixelFormat.R32G32B32A32.Float);
            AlbedoTarget = RenderTarget2D.New(context.Device, width, height, PixelFormat.R8G8B8A8.UNormSRgb);
        }

        IGraphicsContext context;
        private Effect gbufferEffect;
        Buffer<QuadVert> vb;
        Buffer<LightQuadVertex> lightVb;

        public RenderTarget2D DepthTarget { get; set; }
        public RenderTarget2D NormalTarget { get; set; }
        public RenderTarget2D AlbedoTarget { get; set; }

        SharpDX.Toolkit.Graphics.BlendState clearBlendState;

        public void Initialize()
        {
            gbufferEffect = EffectLoader.Load(@"Graphics/Shaders/GBufferUber.fx");

            QuadVert[] verts = new QuadVert[6]
            {
                new QuadVert(-1, -1, 0),
                new QuadVert(-1, 1, 0),
                new QuadVert(1, 1, 0),
                new QuadVert(1, 1, 0),
                new QuadVert(1, -1, 0),
                new QuadVert(-1, -1, 0)
            };

            vb = SharpDX.Toolkit.Graphics.Buffer.New<QuadVert>(context.Device, verts, BufferFlags.VertexBuffer);

            LightQuadVertex[] lightVerts = new LightQuadVertex[6]
            {
                new LightQuadVertex(-1, -1, 0, 0, 1, 3),
                new LightQuadVertex(-1, 1, 0, 0, 0, 2),
                new LightQuadVertex(1, 1, 0, 1, 0, 1),
                
                new LightQuadVertex(1, 1, 0, 1, 0, 1),
                new LightQuadVertex(1, -1, 0, 1, 1, 0),
                new LightQuadVertex(-1, -1, 0, 0, 1, 3)
            };

            lightVb = SharpDX.Toolkit.Graphics.Buffer.New<LightQuadVertex>(context.Device, lightVerts, BufferFlags.VertexBuffer);
        }

        public void SetAsRenderTarget()
        {
            context.Device.SetRenderTargets(context.Device.DepthStencilBuffer, DepthTarget, NormalTarget, AlbedoTarget);
        }

        public void SetAsShaderInput(Effect shader)
        {
            SharpDX.Toolkit.Graphics.SamplerState sampler = context.Device.SamplerStates.LinearClamp;

            shader.Parameters["gbDepthTexture"].SetResource(DepthTarget);
            shader.Parameters["gbDepthSampler"].SetResource(sampler);
            shader.Parameters["gbNormalTexture"].SetResource(NormalTarget);
            shader.Parameters["gbNormalSampler"].SetResource(sampler);
            shader.Parameters["gbAlbedoTexture"].SetResource(AlbedoTarget);
            shader.Parameters["gbAlbedoSampler"].SetResource(sampler);
        }

        public void Clear()
        {
            if (clearBlendState == null)
            {
                BlendStateDescription desc = new BlendStateDescription();
                desc.RenderTarget[0].IsBlendEnabled = false;
                desc.RenderTarget[1].IsBlendEnabled = false;
                desc.RenderTarget[2].IsBlendEnabled = false;

                clearBlendState = context.Device.BlendStates.Opaque; // SharpDX.Toolkit.Graphics.BlendState.New(context.Device, desc);
            }

            context.Device.SetVertexBuffer(vb);
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vb));
            context.Device.SetDepthStencilState(context.Device.DepthStencilStates.None);
            context.Device.SetBlendState(clearBlendState);
            context.Device.SetRasterizerState(context.Device.RasterizerStates.CullNone);
            gbufferEffect.Techniques["Clear"].Passes[0].Apply();
            context.Device.Draw(PrimitiveType.TriangleList, 6);
        }

        public void SetCameraParameters(Camera camera)
        {
            gbufferEffect.Parameters["wPosCamera"].SetValue(camera.Position);
            gbufferEffect.Parameters["near_plane"].SetValue(camera.NearPlane);
            gbufferEffect.Parameters["far_plane"].SetValue(camera.FarPlane);
            gbufferEffect.Parameters["fov"].SetValue(camera.FieldOfView);

            Vector3[] frustumCornersWS = new Vector3[8];
            Vector4[] frustumCornersVS = new Vector4[8];
            Vector4[] farFrustumCornersVS = new Vector4[4];
            Matrix view = camera.View;
            Matrix viewProj = Matrix.Multiply(camera.View, camera.Proj);
            BoundingFrustum frustum = new BoundingFrustum(viewProj);
            frustum.GetCorners(frustumCornersWS);

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

            gbufferEffect.Parameters["frustumCornersVS"].SetValue(farFrustumCornersVS);
        }

        public void SetShaderParameters(SceneNode node)
        {
            gbufferEffect.Parameters["World"].SetValue(node.World.ToMatrix());
            gbufferEffect.Parameters["View"].SetValue(context.CurrentCamera.View);
            gbufferEffect.Parameters["Proj"].SetValue(context.CurrentCamera.Proj);

            Matrix viewNoTrans = context.CurrentCamera.View;
            viewNoTrans.TranslationVector = Vector3.Zero;
            gbufferEffect.Parameters["ViewNoT"].SetValue(viewNoTrans);

            // other stuff later
        }

        public void SetTexture(SharpDX.Toolkit.Graphics.Texture2D texture, string param)
        {
            gbufferEffect.Parameters[param].SetResource(texture);
        }

        public void SetSampler(SharpDX.Toolkit.Graphics.SamplerState samplerState, string param)
        {
            gbufferEffect.Parameters[param].SetResource(samplerState);
        }

        public void RenderGeometry<T>(PrimitiveType primitiveType, Buffer<T> vertexBuffer, int numVertices, string overrideTechnique = null) where T : struct
        {
            EffectTechnique technique = null;
            if (string.IsNullOrEmpty(overrideTechnique))
                technique = GetTechniqueFromVertexBuffer(vertexBuffer);
            else
                technique = gbufferEffect.Techniques[overrideTechnique];

            context.Device.SetVertexBuffer(vertexBuffer);
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vertexBuffer));
            technique.Passes[0].Apply();
            context.Device.Draw(primitiveType, numVertices);
        }

        public void RenderGeometry<T>(PrimitiveType primitiveType, Buffer<T> vertexBuffer, Buffer<int> indexBuffer, int numIndices, int startIndex = 0, int baseVertex = 0, string overrideTechnique = null) where T : struct
        {
            EffectTechnique technique = null;
            if (string.IsNullOrEmpty(overrideTechnique))
                technique = GetTechniqueFromVertexBuffer(vertexBuffer);
            else
                technique = gbufferEffect.Techniques[overrideTechnique];

            context.Device.SetVertexBuffer(vertexBuffer);
            context.Device.SetIndexBuffer(indexBuffer, true);
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vertexBuffer));
            technique.Passes[0].Apply();
            context.Device.DrawIndexed(primitiveType, numIndices, startIndex, baseVertex);
        }

        public void RenderDirectionalLight(DirectionalLight light)
        {
            // set gbuffer targets as texture inputs
            this.SetAsShaderInput(gbufferEffect);

            // set light cbuffer parameters
            gbufferEffect.Parameters["dLightDirection"].SetValue(light.Direction);
            gbufferEffect.Parameters["dLightColor"].SetValue(light.Color);

            context.Device.SetVertexBuffer(lightVb);
            context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, lightVb));
            gbufferEffect.Techniques["DirectionalLight"].Passes[0].Apply();
            context.Device.Draw(PrimitiveType.TriangleList, 6);
        }

        private EffectTechnique GetTechniqueFromVertexBuffer<T>(Buffer<T> vertexBuffer) where T : struct
        {
            EffectTechnique technique = null;
            VertexInputLayout inputLayout = VertexInputLayout.FromBuffer(0, vertexBuffer);
            VertexBufferLayout bufferLayout = inputLayout.BufferLayouts.First();

            bool pos = false, tex = false, col = false;
            var semantics = bufferLayout.VertexElements.Select(ve => ve.SemanticName.ToString()).ToArray();

            if (semantics.Contains("POSITION"))
                pos = true;

            if (semantics.Contains("TEXCOORD"))
                tex = true;

            if (semantics.Contains("COLOR"))
                col = true;

            if (pos && tex)
                technique = gbufferEffect.Techniques["PositionTexture"];

            if (pos && col)
                technique = gbufferEffect.Techniques["PositionColor"];

            if (pos)
                technique = gbufferEffect.Techniques["Position"];

            if (technique == null)
                throw new NotSupportedException();

            return technique;
        }
    }
}
