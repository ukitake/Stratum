using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stratum.Graphics;

namespace Stratum.Graphics
{
    public class GraphicsDeviceService : IGraphicsDeviceService, IWpfGraphicsDevice, IFormsGraphicsService
    {
        private GraphicsDevice graphicsDevice;
        private Device dxDevice;
        private DeviceContext dxContext;

        private GraphicsDeviceInformation deviceInformation;
        private RenderTarget2D renderTarget;

        public GraphicsDeviceService()
        {
            System.Windows.Forms.Control control = new System.Windows.Forms.Panel();
            
            deviceInformation = new GraphicsDeviceInformation()
            {
                GraphicsProfile = FeatureLevel.Level_11_0,
                DeviceCreationFlags = SharpDX.Direct3D11.DeviceCreationFlags.Debug,
                PresentationParameters = new PresentationParameters()
                {
                    BackBufferFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    DepthStencilFormat = DepthFormat.Depth32,
                    DepthBufferShaderResource = true,
                    BackBufferWidth = 1920,
                    BackBufferHeight = 1080,
                    IsFullScreen = false,
                    RenderTargetUsage = SharpDX.DXGI.Usage.RenderTargetOutput,
                    MultiSampleCount = MSAALevel.None,
                    DeviceWindowHandle = control
                }
            };   

            FindAdapter();

            graphicsDevice = GraphicsDevice.New(deviceInformation.Adapter, deviceInformation.DeviceCreationFlags);
            
            renderTarget = RenderTarget2D.New(graphicsDevice, 800, 600, PixelFormat.B8G8R8A8.UNorm);
            graphicsDevice.Presenter = new RenderTargetGraphicsPresenter(graphicsDevice, renderTarget, DepthFormat.Depth24Stencil8, false);
            
            dxDevice = (Device)typeof(GraphicsDevice).GetField("Device", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GraphicsDevice);
            dxContext = (DeviceContext)typeof(GraphicsDevice).GetField("Context", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GraphicsDevice);
            
            if (DeviceCreated != null)
                DeviceCreated(this, EventArgs.Empty);
        }

        public GraphicsDeviceService(System.Windows.Forms.Form form)
        {
            deviceInformation = new GraphicsDeviceInformation()
            {
                GraphicsProfile = FeatureLevel.Level_11_0,
                DeviceCreationFlags = SharpDX.Direct3D11.DeviceCreationFlags.Debug,
                PresentationParameters = new PresentationParameters()
                {
                    BackBufferFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    DepthStencilFormat = DepthFormat.Depth32,
                    DepthBufferShaderResource = true,
                    BackBufferWidth = form.Width,
                    BackBufferHeight = form.Height,
                    IsFullScreen = false,
                    RenderTargetUsage = SharpDX.DXGI.Usage.RenderTargetOutput,
                    MultiSampleCount = MSAALevel.None,
                    DeviceWindowHandle = form
                }
            };

            FindAdapter();

            graphicsDevice = GraphicsDevice.New(deviceInformation.Adapter, deviceInformation.DeviceCreationFlags);
            graphicsDevice.Presenter = new SwapChainGraphicsPresenter(graphicsDevice, deviceInformation.PresentationParameters);

            dxDevice = (Device)typeof(GraphicsDevice).GetField("Device", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GraphicsDevice);
            dxContext = (DeviceContext)typeof(GraphicsDevice).GetField("Context", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GraphicsDevice);

            this.Form = form;

            if (DeviceCreated != null)
                DeviceCreated(this, EventArgs.Empty);
        }

        public void Resize(int width, int height)
        {
            renderTarget.Dispose();
            graphicsDevice.Presenter.Dispose();

            renderTarget = RenderTarget2D.New(graphicsDevice, width, height, PixelFormat.B8G8R8A8.UNorm);
            graphicsDevice.Presenter = new RenderTargetGraphicsPresenter(graphicsDevice, renderTarget, DepthFormat.Depth24Stencil8, false);
        }

        public void ResizeSwapChain(int width, int height)
        {
            graphicsDevice.Presenter.Resize(width, height, SharpDX.DXGI.Format.B8G8R8A8_UNorm);
        }

        public event EventHandler<EventArgs> DeviceChangeBegin;

        public event EventHandler<EventArgs> DeviceChangeEnd;

        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceDisposing;

        public event EventHandler<EventArgs> DeviceLost;

        public GraphicsDevice GraphicsDevice
        {
            get { return graphicsDevice; }
        }

        public RenderTarget2D BackBuffer
        {
            get { return renderTarget ?? graphicsDevice.Presenter.BackBuffer; }
        }

        public Device Device
        {
            get { return dxDevice; }
        }

        public DeviceContext DeviceContext
        {
            get { return dxContext; }
        }

        public Form Form { get; private set; }

        private void FindAdapter()
        {
            foreach (var graphicsAdapter in GraphicsAdapter.Adapters)
            {
                if (graphicsAdapter.IsProfileSupported(deviceInformation.GraphicsProfile))
                {
                    deviceInformation.Adapter = graphicsAdapter;
                    return;
                }
            }
            throw new Exception("Unable to find DirectX 11 compatible graphics adapter");
        }
    }
}
