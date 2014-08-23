using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stratum.Cesium;
using Stratum.Graphics;
using Stratum.Graphics.RenderCommands;
using Stratum.World.Earth;

namespace Stratum
{
    public class Renderer
    {
        // ignore concurrency for now
        private Queue<IRenderCommand> normalQueue = new Queue<IRenderCommand>();
        private Queue<IRenderDeferredCommand> deferredQueue = new Queue<IRenderDeferredCommand>();
        private Queue<IRenderCommand> postProcessQueue = new Queue<IRenderCommand>();
        private Queue<Stratum.Graphics.DirectionalLight> lightQueue = new Queue<Stratum.Graphics.DirectionalLight>();

        public Renderer()
        {
            IGraphicsContext context = Engine.GraphicsContext;
            int width = context.Device.BackBuffer.Width;
            int height = context.Device.BackBuffer.Height;
            sceneTarget = RenderTarget2D.New(context.Device, width, height, PixelFormat.R8G8B8A8.UNorm);
            texToTargetEffect = EffectLoader.Load(@"Graphics/Shaders/CopyTextureToTarget.fx");
            gbuffer = context.GBuffer;

            font = FontLoader.Load(@"Graphics/Fonts/Arial16.xml");
            fpsClock = new Stopwatch();
            fpsText = string.Empty;
        }

        public void StartFramerateCounter()
        {
            fpsClock.Start();
        }

        public void EnqueueNormal(IRenderCommand command)
        {
            if (command is IRenderDeferredCommand)
                throw new NotSupportedException("Cannot enqueue a deferred render command to the normal queue");

            normalQueue.Enqueue(command);
        }

        public void EnqueueDeferred(IRenderDeferredCommand command)
        {
            deferredQueue.Enqueue(command);
        }

        public void EnqueuePostProcess(IRenderCommand command)
        {
            // todo: post process might need its own kind of command?
            postProcessQueue.Enqueue(command);
        }

        public void EnqueueLight(Stratum.Graphics.DirectionalLight light)
        {
            lightQueue.Enqueue(light);
        }

        public void Render(GameTime gameTime, IGraphicsContext context)
        {
            var device = context.Device;
            var renderContext = context.RenderContext;

            // clear render queues
            normalQueue.Clear();
            deferredQueue.Clear();
            postProcessQueue.Clear();
            lightQueue.Clear();

            // HACK: update current camera... camera probably needs to be a GameObject
            context.CurrentCamera.Update(gameTime);

            // Depth First Traversal of Scene Graph to queue render commands
            Engine.SceneGraph.QueueRenderCommands(gameTime, this, context);

            PrepareForNewFrame(device);

            // render deferred queue to gbuffer 1st
            gbuffer.SetAsRenderTarget();
            gbuffer.Clear();

            while (deferredQueue.Count > 0)
            {
                var command = deferredQueue.Dequeue();
                ProcessRenderCommand(command, context);
            }

            // switch to scene target
            device.Clear(sceneTarget, Color.Black);
            device.SetRenderTargets(sceneTarget);
            
            // render all the lights to shade deferred objects
            while (lightQueue.Count > 0)
            {
                var light = lightQueue.Dequeue();
                gbuffer.RenderDirectionalLight(light);
            }
        
            // render normal queue to scene target
            // note that we're carrying over depth info from the deferred queue
            // since we haven't changed depth buffers
            while (normalQueue.Count > 0)
            {
                var command = normalQueue.Dequeue();
                ProcessRenderCommand(command, context);
            }

            // apply post processes that need to run on the MSAA render target
            // before it is resolved into the backbuffer
            while (postProcessQueue.Count > 0)
            {
                var command = postProcessQueue.Dequeue();
                ProcessRenderCommand(command, context);
            }

            // resolve MSAA scene target to back buffer
            device.Copy((GraphicsResource)sceneTarget, 0, (GraphicsResource)device.BackBuffer, 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm);

            device.SetRenderTargets(device.BackBuffer);
            RenderDiagnostics(context);

            // Present
            device.Present();

            // commit the changes to the image source
            if (context.ImageSourcePresenter != null)
            {
                var renderTarget = ((GraphicsDeviceService)context.GraphicsService).BackBuffer;
                context.ImageSourcePresenter.Commit(renderTarget);
            }
        }

        private void PrepareForNewFrame(GraphicsDevice device)
        {
            device.ClearState();

            device.Clear(device.DepthStencilBuffer, SharpDX.Direct3D11.DepthStencilClearFlags.Depth, 1, 0);
            device.Clear(device.BackBuffer, Color.Black);
        }

        private void ProcessRenderCommand(IRenderCommand command, IGraphicsContext context)
        {
            context.Device.SetBlendState(command.BlendState);
            context.Device.SetDepthStencilState(command.DepthStencilState);
            context.Device.SetRasterizerState(command.RasterizerState);

            command.SetGeometry(context);
            command.SetResources(context);
            command.SetConstants(context);

            command.Render(context);
        }

        private void CopyTextureToRenderTarget(GraphicsDevice device, Texture2DBase texture, RenderTarget2D renderTarget)
        {
            device.SetRenderTargets(renderTarget);
            texToTargetEffect.Parameters["CopyTexture"].SetResource(texture);
            texToTargetEffect.Parameters["CopyTextureSamp"].SetResource(device.SamplerStates.LinearClamp);
            device.DrawQuad(texToTargetEffect);
        }

        private void RenderDiagnostics(IGraphicsContext context)
        {
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
            context.SpriteBatch.DrawString(font, string.Format("Num Terrain Triangles: {0}", Engine.GetComponent<PlanetComponent>(2).NodeCount * 64 * 64 * 2), new Vector2(0, 75), Color.White);
            context.SpriteBatch.End();
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

        Skybox skybox;
        public void _Render(GameTime gameTime, IGraphicsContext context)
        {
            if (isFirst)
            {
                skybox = new Skybox(context.Device);
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

            //gbuffer.SetAsRenderTarget();
            //gbuffer.Clear();

            // update current camera
            context.CurrentCamera.Update(gameTime);

            device.SetDepthStencilState(device.DepthStencilStates.Default);

            // Render SceneGraph to GBuffer
            //Engine.SceneGraph.Render(gameTime, context);

            // switch to scene target
            //device.Clear(sceneTarget, Color.Black);
            //device.SetRenderTargets(sceneTarget);

            skybox.QueueRenderCommands(gameTime, this, context);

            Engine.SceneGraph.QueueRenderCommands(gameTime, this, context);
            //worldEngine.Skybox.Render(gameTime, context);

            // Render Custom Visualization Systems

            device.SetDepthStencilState(device.DepthStencilStates.None);
            device.SetBlendState(device.BlendStates.Default);

            // Render Lights
            //context.GBuffer.RenderDirectionalLight(new Graphics.DirectionalLight());

            // Render atmosphere as post-process
            //worldEngine.Atmosphere.Render(gameTime, context);

            // copy scene target to back buffer
            //device.SetRenderTargets(device.BackBuffer);
            //texToTargetEffect.Parameters["CopyTexture"].SetResource(sceneTarget);
            //texToTargetEffect.Parameters["CopyTextureSamp"].SetResource(device.SamplerStates.LinearClamp);
            //device.DrawQuad(texToTargetEffect);

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

        public void RenderGeometry<T>(PrimitiveType primitiveType, Buffer<T> vertexBuffer, int numVertices, Effect effect, string overrideTechnique = null) where T : struct
        {
            EffectTechnique technique = null;
            if (string.IsNullOrEmpty(overrideTechnique))
                technique = effect.Techniques.First();
            else
                technique = effect.Techniques[overrideTechnique];

            Engine.GraphicsContext.Device.SetVertexBuffer(vertexBuffer);
            Engine.GraphicsContext.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vertexBuffer));
            technique.Passes[0].Apply();
            Engine.GraphicsContext.Device.Draw(primitiveType, numVertices);
        }

        public void RenderGeometry<T>(PrimitiveType primitiveType, Buffer<T> vertexBuffer, Buffer<int> indexBuffer, Effect effect, int numIndices, int startIndex = 0, int baseVertex = 0, string overrideTechnique = null) where T : struct
        {
            EffectTechnique technique = null;
            if (string.IsNullOrEmpty(overrideTechnique))
                technique = effect.Techniques.First();
            else
                technique = effect.Techniques[overrideTechnique];

            Engine.GraphicsContext.Device.SetVertexBuffer(vertexBuffer);
            Engine.GraphicsContext.Device.SetIndexBuffer(indexBuffer, true);
            Engine.GraphicsContext.Device.SetVertexInputLayout(VertexInputLayout.FromBuffer(0, vertexBuffer));
            technique.Passes[0].Apply();
            Engine.GraphicsContext.Device.DrawIndexed(primitiveType, numIndices, startIndex, baseVertex);
        }


        public void RenderDeferred(GameTime gameTime, IGraphicsContext context)
        {
            throw new NotImplementedException();
        }
    }
}
