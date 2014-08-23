using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit.Graphics;
using Stratum.Graphics;
using Stratum.Components;
using Stratum.Graphics.RenderCommands;
using Stratum.CompGeom;

namespace Stratum.WorldEngine
{
    public class Atmosphere : RenderableComponent
    {
        private class AtmosphereRenderCommand : RenderCommand<LightQuadVertex>
        {
            public AtmosphereRenderCommand(
                Effect effect,
                PrimitiveType primitiveType,
                SharpDX.Toolkit.Graphics.Buffer<LightQuadVertex> vertexBuffer,
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
                // set camera parameters
                var camera = context.CurrentCamera;

                Effect.Parameters["wPosCamera"].SetValue(camera.Position);
                Effect.Parameters["near_plane"].SetValue((float)camera.NearPlane);
                Effect.Parameters["far_plane"].SetValue((float)camera.FarPlane);
                Effect.Parameters["fov"].SetValue((float)camera.FieldOfView);

                Vector3D[] frustumCornersWS = new Vector3D[8];
                Vector3D[] farFrustumCornersWS = new Vector3D[4];
                Vector3D[] farFrustumCornersVS = new Vector3D[4];
                MatrixD view = camera.ViewD;

                BoundingFrustumD frustum = new BoundingFrustumD(context.RenderContext.ViewProjD);
                frustum.GetCorners(frustumCornersWS);

                //  2 ____________  1
                //   |            |
                //   |            |
                //   |            |
                //  3|____________| 0
                //
                for (int i = 4; i < 8; i++)
                {
                    farFrustumCornersWS[i - 4] = frustumCornersWS[i];
                }

                Vector3D.TransformCoordinate(farFrustumCornersWS, ref view, farFrustumCornersVS);

                Vector3D[] o = new Vector3D[4];
                Vector4D[] o4 = new Vector4D[4];
                for (int i = 0; i < 4; i++)
                {
                    Vector3D.Normalize(ref farFrustumCornersVS[i], out o[i]);
                    o4[i] = new Vector4D(o[i], 1.0);
                }

                Effect.Parameters["frustumCornersVS"].SetValue(o4.Select(v => v.ToVector4()).ToArray());
                view.Invert();
                Effect.Parameters["IView"].SetValue(view.ToMatrix());

                // set sun parameters
                Effect.Parameters["sunVector"].SetValue(new Vector3(0, 0, 1));
            }
        }

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

        public const float Rg = 6360f;
        public const float Rt = 6420f;
        public const float RL = 6421f;
        public const int TRANSMITTANCE_W = 256;
        public const int TRANSMITTANCE_H = 64;
        public const int SKY_W = 64;
        public const int SKY_H = 16;
        public const int RES_R = 32;
        public const int RES_MU = 128;
        public const int RES_MU_S = 32;
        public const int RES_NU = 8;
        public const float AVERAGE_GROUND_REFLECTANCE = 0.1f;
        public const float HR = 8f;
        public Vector3 betaR = new Vector3(5.8e-3f, 1.35e-2f, 3.31e-2f);
        public const float HM = 1.2f;
        public Vector3 betaMSca = new Vector3(4e-3f, 4e-3f, 4e-3f);
        public Vector3 betaMEx = new Vector3(4.44e-3f, 4.44e-3f, 4.44e-3f);
        public const float mieG = 0.8f;

        SharpDX.Toolkit.Graphics.Buffer<LightQuadVertex> atmosVb;

        RenderTarget2D transmittanceT;
        RenderTarget2D irradianceT;
        RenderTarget3D inscatterT;
        RenderTarget2D deltaET;
        RenderTarget3D deltaSRT;
        RenderTarget3D deltaSMT;
        RenderTarget3D deltaJT;

        Effect atmosphere;
        Effect copyInscatter1;
        Effect copyInscatterN;
        Effect copyIrradiance;
        Effect inscatter1;
        Effect inscatterN;
        Effect inscatterS;
        Effect irradiance1;
        Effect irradianceN;
        Effect transmittance;

        SharpDX.Toolkit.Graphics.Buffer<QuadVert> vb;

        RenderTarget2D fbo;
        int step;
        int order;

        public Atmosphere()
        {
            GraphicsDevice device = Engine.GraphicsContext.Device;
            transmittanceT = RenderTarget2D.New(device, TRANSMITTANCE_W, TRANSMITTANCE_H, PixelFormat.R16G16B16A16.Float);
            irradianceT = RenderTarget2D.New(device, SKY_W, SKY_H, PixelFormat.R16G16B16A16.Float);
            inscatterT = RenderTarget3D.New(device, RES_MU_S * RES_NU, RES_MU, RES_R, PixelFormat.R16G16B16A16.Float);
            deltaET = RenderTarget2D.New(device, SKY_W, SKY_H, PixelFormat.R16G16B16A16.Float);
            deltaSRT = RenderTarget3D.New(device, RES_MU_S * RES_NU, RES_MU, RES_R, PixelFormat.R16G16B16A16.Float);
            deltaSMT = RenderTarget3D.New(device, RES_MU_S * RES_NU, RES_MU, RES_R, PixelFormat.R16G16B16A16.Float);
            deltaJT = RenderTarget3D.New(device, RES_MU_S * RES_NU, RES_MU, RES_R, PixelFormat.R16G16B16A16.Float);

            atmosphere = EffectLoader.Load(@"World/Atmosphere/Shaders/atmosphere.fx");
            copyInscatter1 = EffectLoader.Load(@"World/Atmosphere/Shaders/copyInscatter1.fx");
            copyInscatterN = EffectLoader.Load(@"World/Atmosphere/Shaders/copyInscatterN.fx");
            copyIrradiance = EffectLoader.Load(@"World/Atmosphere/Shaders/copyIrradiance.fx");
            inscatter1 = EffectLoader.Load(@"World/Atmosphere/Shaders/inscatter1.fx");
            inscatterN = EffectLoader.Load(@"World/Atmosphere/Shaders/inscatterN.fx");
            inscatterS = EffectLoader.Load(@"World/Atmosphere/Shaders/inscatterS.fx");
            irradiance1 = EffectLoader.Load(@"World/Atmosphere/Shaders/irradiance1.fx");
            irradianceN = EffectLoader.Load(@"World/Atmosphere/Shaders/irradianceN.fx");
            transmittance = EffectLoader.Load(@"World/Atmosphere/Shaders/transmittance.fx");

            QuadVert[] verts = new QuadVert[6]
            {
                new QuadVert(-1, -1, 0),
                new QuadVert(-1, 1, 0),
                new QuadVert(1, 1, 0),
                new QuadVert(1, 1, 0),
                new QuadVert(1, -1, 0),
                new QuadVert(-1, -1, 0)
            };

            vb = SharpDX.Toolkit.Graphics.Buffer.New<QuadVert>(device, verts, BufferFlags.VertexBuffer);

            PreProcess();
        }

        void setLayer(Effect p, int layer)
        {
            double r = layer / (double)(RES_R - 1.0);
            r = r * r;
            r = Math.Sqrt(Rg * Rg + r * (Rt * Rt - Rg * Rg)) + (layer == 0 ? 0.01 : (layer == RES_R - 1 ? -0.001 : 0.0));
            double dmin = Rt - r;
            double dmax = Math.Sqrt(r * r - Rg * Rg) + Math.Sqrt(Rt * Rt - Rg * Rg);
            double dminp = r - Rg;
            double dmaxp = Math.Sqrt(r * r - Rg * Rg);

            p.Parameters["r"].SetValue((float)r);
            p.Parameters["dhdH"].SetValue(new Vector4((float)dmin, (float)dmax, (float)dminp, (float)dmaxp));
            p.Parameters["layer"].SetValue(layer);
        }

        public void PreProcess()
        {
            order = 2;

            GraphicsDevice device = Engine.GraphicsContext.Device;
            device.SetRasterizerState(device.RasterizerStates.CullNone);
            device.SetBlendState(device.BlendStates.Default);
            device.SetDepthStencilState(device.DepthStencilStates.None);

            // compute transmittance texture T (line 1 in algorithm 4.1)
            device.SetRenderTargets(transmittanceT);
            device.SetViewport(new ViewportF(0, 0, TRANSMITTANCE_W, TRANSMITTANCE_H));
            device.DrawQuad(transmittance);

            // compute irradiance texture deltaE (line 2 in algorithm 4.1)
            device.SetRenderTargets(deltaET);
            device.SetViewport(new ViewportF(0, 0, SKY_W, SKY_H));
            irradiance1.Parameters["transTex"].SetResource(transmittanceT);
            device.DrawQuad(irradiance1);

            //// compute single scattering texture deltaS (line 3 in algorithm 4.1)
            //// Rayleigh and Mie separated in deltaSR + deltaSM
            device.SetRenderTargets(deltaSRT, deltaSMT);
            device.SetViewport(new ViewportF(0, 0, RES_MU_S * RES_NU, RES_MU));
            device.SetVertexBuffer(vb);
            device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vb));
            inscatter1.Parameters["_transTex"].SetResource(transmittanceT);
            inscatter1.Parameters["_transSamp"].SetResource(device.SamplerStates.LinearClamp);
            for (int layer = 0; layer < RES_R; layer++)
            {
                setLayer(inscatter1, layer);
                inscatter1.CurrentTechnique.Passes[0].Apply();
                device.Draw(PrimitiveType.TriangleList, 6);
            }

            // copy deltaE into irradiance texture E (line 4 in algorithm 4.1)
            // really this just clears irradianceT
            device.SetRenderTargets(irradianceT);
            device.SetViewport(new ViewportF(0, 0, SKY_W, SKY_H));
            copyIrradiance.Parameters["k"].SetValue(0f);
            copyIrradiance.Parameters["deltaE"].SetResource(deltaET);
            device.DrawQuad(copyIrradiance);

            for (; order < 3; order++)
            {
                // copy deltaS into inscatter texture S (line 5 in algorithm 4.1)
                device.SetRenderTargets(inscatterT);
                device.SetViewport(new ViewportF(0, 0, RES_MU_S * RES_NU, RES_MU));
                device.SetVertexBuffer(vb);
                device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vb));
                copyInscatter1.Parameters["deltaSR"].SetResource(deltaSRT);
                copyInscatter1.Parameters["deltaSM"].SetResource(deltaSMT);
                for (int layer = 0; layer < RES_R; layer++)
                {
                    setLayer(copyInscatter1, layer);
                    copyInscatter1.CurrentTechnique.Passes[0].Apply();
                    device.Draw(PrimitiveType.TriangleList, 6);
                }

                // compute deltaJ (line 7 in algorithm 4.1)
                device.SetRenderTargets(deltaJT);
                device.SetViewport(new ViewportF(0, 0, RES_MU_S * RES_NU, RES_MU));
                device.SetVertexBuffer(vb);
                device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vb));
                inscatterS.Parameters["first"].SetValue(order == 2 ? 1.0f : 0.0f);
                inscatterS.Parameters["trans"].SetResource(transmittanceT);
                inscatterS.Parameters["deltaE"].SetResource(deltaET);
                inscatterS.Parameters["deltaSR"].SetResource(deltaSRT);
                inscatterS.Parameters["deltaSM"].SetResource(deltaSMT);
                for (int layer = 0; layer < RES_R; layer++)
                {
                    setLayer(inscatterS, layer);
                    inscatterS.CurrentTechnique.Passes[0].Apply();
                    device.Draw(PrimitiveType.TriangleList, 6);
                }

                // compute deltaE (line 8 in algorithm 4.1)
                device.SetRenderTargets(deltaET);
                device.SetViewport(new ViewportF(0, 0, SKY_W, SKY_H));
                irradianceN.Parameters["first"].SetValue(order == 2 ? 1.0f : 0.0f);
                irradianceN.Parameters["deltaSR"].SetResource(deltaSRT);
                irradianceN.Parameters["deltaSM"].SetResource(deltaSMT);
                device.DrawQuad(irradianceN);

                // compute deltaS (line 9 in algorithm 4.1)
                device.SetRenderTargets(deltaSRT);
                device.SetViewport(new ViewportF(0, 0, RES_MU_S * RES_NU, RES_MU));
                device.SetVertexBuffer(vb);
                device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vb));
                inscatterN.Parameters["trans"].SetResource(transmittanceT);
                inscatterN.Parameters["deltaJ"].SetResource(deltaJT);
                for (int layer = 0; layer < RES_R; layer++)
                {
                    setLayer(inscatterN, layer);
                    inscatterN.CurrentTechnique.Passes[0].Apply();
                    device.Draw(PrimitiveType.TriangleList, 6);
                }

                device.SetBlendState(BlendState.New(device,
                    SharpDX.Direct3D11.BlendOption.One,
                    SharpDX.Direct3D11.BlendOption.One,
                    SharpDX.Direct3D11.BlendOperation.Add,
                    SharpDX.Direct3D11.BlendOption.One,
                    SharpDX.Direct3D11.BlendOption.One,
                    SharpDX.Direct3D11.BlendOperation.Add));

                // add deltaE into irradiance texture E (line 10 in algorithm 4.1)
                device.SetRenderTargets(irradianceT);
                device.SetViewport(new ViewportF(0, 0, SKY_W, SKY_H));
                copyIrradiance.Parameters["k"].SetValue(1.0f);
                copyIrradiance.Parameters["deltaE"].SetResource(deltaET);
                device.DrawQuad(copyIrradiance);

                // add deltaS into inscatter texture S (line 11 in algorithm 4.1)
                device.SetRenderTargets(inscatterT);
                device.SetViewport(new ViewportF(0, 0, RES_MU_S * RES_NU, RES_MU));
                device.SetVertexBuffer(vb);
                device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vb));
                copyInscatterN.Parameters["deltaS"].SetResource(deltaSRT);
                for (int layer = 0; layer < RES_R; layer++)
                {
                    setLayer(copyInscatterN, layer);
                    copyInscatterN.CurrentTechnique.Passes[0].Apply();
                    device.Draw(PrimitiveType.TriangleList, 6);
                }

                device.SetBlendState(device.BlendStates.Opaque);
            } // end multiple scattering loop

            LightQuadVertex[] lightVerts = new LightQuadVertex[6]
            {
                new LightQuadVertex(-1, -1, 0, 0, 1, 3),
                new LightQuadVertex(-1, 1, 0, 0, 0, 2),
                new LightQuadVertex(1, 1, 0, 1, 0, 1),
                
                new LightQuadVertex(1, 1, 0, 1, 0, 1),
                new LightQuadVertex(1, -1, 0, 1, 1, 0),
                new LightQuadVertex(-1, -1, 0, 0, 1, 3)
            };

            atmosVb = SharpDX.Toolkit.Graphics.Buffer.New<LightQuadVertex>(device, lightVerts, BufferFlags.VertexBuffer);
        }

        private void SetSunParameters()
        {
            atmosphere.Parameters["sunVector"].SetValue(new Vector3(0, 0, -1));
        }

        private void SetTextures(IGraphicsContext context)
        {
            atmosphere.Parameters["gbDepthTexture"].SetResource(context.GBuffer.DepthTarget);
            atmosphere.Parameters["gbNormalTexture"].SetResource(context.GBuffer.NormalTarget);
            atmosphere.Parameters["gbAlbedoTexture"].SetResource(context.GBuffer.AlbedoTarget);
            atmosphere.Parameters["texTransmittance"].SetResource(transmittanceT);
            atmosphere.Parameters["texIrradiance"].SetResource(irradianceT);
            atmosphere.Parameters["texInscatter"].SetResource(inscatterT);
        }

        private Dictionary<string, GraphicsResource> resourcesDic;

        public override void QueueRenderCommands(SharpDX.Toolkit.GameTime gameTime, Renderer renderer, IGraphicsContext context)
        {
            if (resourcesDic == null)
            {
                resourcesDic = new Dictionary<string, GraphicsResource>();
                resourcesDic["gbDepthTexture"] = context.GBuffer.DepthTarget;
                resourcesDic["gbNormalTexture"] = context.GBuffer.NormalTarget;
                resourcesDic["gbAlbedoTexture"] = context.GBuffer.AlbedoTarget;
                resourcesDic["texTransmittance"] = transmittanceT;
                resourcesDic["texIrradiance"] = irradianceT;
                resourcesDic["texInscatter"] = inscatterT;
            }

            var command = GenerateRenderCommand();
            renderer.EnqueuePostProcess(command);

            // post process 
            //SetCameraParameters(context.CurrentCamera);
            //SetSunParameters();
            //SetTextures(context);

            //context.Device.SetBlendState(context.Device.BlendStates.AlphaBlend);

            //context.Device.SetVertexBuffer(atmosVb);
            //context.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, atmosVb));
            //atmosphere.Techniques["Atmosphere"].Passes[0].Apply();
            //context.Device.Draw(PrimitiveType.TriangleList, 6);
        }

        protected override IRenderCommand GenerateRenderCommand()
        {
            return new AtmosphereRenderCommand(atmosphere, PrimitiveType.TriangleList, atmosVb, 6,
                resourcesDic, this.Object.World.ToMatrix(), Engine.GraphicsContext.Device.BlendStates.AlphaBlend, null, null, "Atmosphere");
        }
    }
}
