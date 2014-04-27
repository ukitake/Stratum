using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stratum.Graphics;

namespace Stratum
{
    public class RenderingEngine : IRender
    {
        public RenderingEngine()
        {
            IGraphicsContext context = Engine.GraphicsContext;
            int width = context.Device.BackBuffer.Width;
            int height = context.Device.BackBuffer.Height;
            sceneTarget = RenderTarget2D.New(context.Device, width, height, PixelFormat.R16G16B16A16.Float);
            texToTargetEffect = EffectLoader.Load(@"Graphics/Shaders/CopyTextureToTarget.fx");
            gbuffer = context.GBuffer;

            font = FontLoader.Load(@"Graphics/Fonts/Arial16.xml");
            fpsClock = new Stopwatch();
            fpsText = string.Empty;
        }

        private GeometryBuffer gbuffer;

        // HDR target 
        private RenderTarget2D sceneTarget;

        private Effect texToTargetEffect;

        private long frameCount;
        private SpriteFont font;
        private Stopwatch fpsClock;
        private string fpsText;
        private bool isFirst = true;

        public void Render(GameTime gameTime, IGraphicsContext context)
        {
            if (isFirst)
            {
                isFirst = false;
                fpsClock.Start();
            }

            var device = context.Device;

            device.SetRenderTargets(device.DepthStencilBuffer, device.BackBuffer);
            device.SetViewports(new ViewportF(0, 0, device.BackBuffer.Width, device.BackBuffer.Height));

            device.Clear(device.DepthStencilBuffer, SharpDX.Direct3D11.DepthStencilClearFlags.Depth, 1, 0);
            device.Clear(device.BackBuffer, Color.Black);

            device.SetBlendState(device.BlendStates.Default);
            device.SetDepthStencilState(device.DepthStencilStates.None);
            device.SetRasterizerState(device.RasterizerStates.CullBack);

            gbuffer.SetAsRenderTarget();
            gbuffer.Clear();

            // update current camera
            context.CurrentCamera.Update(gameTime);

            device.SetDepthStencilState(device.DepthStencilStates.Default);

            // Render SceneGraph to GBuffer
            //worldEngine.SceneGraph.Render(gameTime, context);

            // switch to scene target
            device.Clear(sceneTarget, Color.Black);
            device.SetRenderTargets(sceneTarget);

            // Earth test
            //worldEngine.SceneGraph.Render(gameTime, context);

            //worldEngine.Skybox.Render(gameTime, context);

            // Render Custom Visualization Systems

            device.SetDepthStencilState(device.DepthStencilStates.None);
            device.SetBlendState(device.BlendStates.Default);

            // Render Lights
            context.GBuffer.RenderDirectionalLight(new Graphics.DirectionalLight());

            // Render atmosphere as post-process
            //worldEngine.Atmosphere.Render(gameTime, context);

            // copy scene target to back buffer
            device.SetRenderTargets(device.BackBuffer);
            texToTargetEffect.Parameters["CopyTexture"].SetResource(sceneTarget);
            texToTargetEffect.Parameters["CopyTextureSamp"].SetResource(device.SamplerStates.LinearClamp);
            device.DrawQuad(texToTargetEffect);

            frameCount++;
            if (fpsClock.ElapsedMilliseconds > 1000.0f)
            {
                fpsText = string.Format("{0:F2} FPS", (float)frameCount * 1000 / fpsClock.ElapsedMilliseconds);
                frameCount = 0;
                fpsClock.Restart();
            }

            context.SpriteBatch.Begin();
            context.SpriteBatch.DrawString(font, fpsText, Vector2.Zero, Color.White);
            context.SpriteBatch.DrawString(font, string.Format("camera X: {0}", context.CurrentCamera.Position.X), new Vector2(0, 15), Color.White);
            context.SpriteBatch.DrawString(font, string.Format("camera Y: {0}", context.CurrentCamera.Position.Y), new Vector2(0, 30), Color.White);
            context.SpriteBatch.DrawString(font, string.Format("camera Z: {0}", context.CurrentCamera.Position.Z), new Vector2(0, 45), Color.White);
            context.SpriteBatch.DrawString(font, string.Format("Camera Height: {0}", context.CurrentCamera.Position.Length()), new Vector2(0, 60), Color.White);
            context.SpriteBatch.End();

            // Present
            device.Present();

            // reset all state
            device.ClearState();

            // commit the changes to the image source
            if (context.ImageSourcePresenter != null)
            {
                var renderTarget = ((GraphicsDeviceService)context.GraphicsService).BackBuffer;
                context.ImageSourcePresenter.Commit(renderTarget);
            }
        }


        public void RenderDebug(GameTime gameTime, IGraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
