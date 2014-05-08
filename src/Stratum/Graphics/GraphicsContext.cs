using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Graphics;
using Stratum.Graphics;
using WPFDXInterop;

namespace Stratum.Graphics
{
    public class GraphicsContext : IGraphicsContext
    {
        public GraphicsContext(IGraphicsDeviceService service, Device d3dDevice, DeviceContext d3dContext)
        {
            this.RenderContext = new RenderContext();

            this.GraphicsService = service;
            this.Device = service.GraphicsDevice;
            this.D3D11Device = d3dDevice;
            this.D3D11Context = d3dContext;

            this.SpriteBatch = new SpriteBatch(service.GraphicsDevice);

            this.GBuffer = new GeometryBuffer(this);
            this.CurrentCamera = new PlanetCamera(6359.99);
        }

        public IGraphicsDeviceService GraphicsService { get; private set; }

        public IImageSourcePresenter ImageSourcePresenter { get; set; }

        public IRenderContext RenderContext { get; private set; }

        public SharpDX.Toolkit.Graphics.GraphicsDevice Device { get; private set; }

        public SharpDX.Toolkit.Graphics.SpriteBatch SpriteBatch { get; private set; }

        public Device D3D11Device { get; private set; }

        public DeviceContext D3D11Context { get; private set; }

        public GeometryBuffer GBuffer { get; private set; }

        public float AspectRatio { get { return (float)Device.BackBuffer.Width / (float)Device.BackBuffer.Height; } }

        public Camera CurrentCamera { get; set; }

        public void ExclusiveDeviceExec(Action actionThatNeedsExclusiveDevice)
        {
            lock (Device)
            {
                actionThatNeedsExclusiveDevice();
            }
        }
    }
}
