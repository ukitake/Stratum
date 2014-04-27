using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using SharpDX.Direct3D11;
using SharpDX.Direct3D;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System.Reflection;

namespace WPFDXInterop
{
    public class DXGraphicsDeviceService : IGraphicsDeviceService
    {
        System.Windows.Forms.Panel _dummyFormsControl;

        GraphicsDeviceInformation _deviceInformation;

        

        public GraphicsDevice GraphicsDevice { get; private set; }
        public Device _dxDevice;

        public DXGraphicsDeviceService()
        {
            _dummyFormsControl = new System.Windows.Forms.Panel();
            CreateDevice(_dummyFormsControl);
            //CreateDevice();
        }

        public Device DXDevice
        {
            get { return _dxDevice; }
        }

        //private void CreateDevice()
        //{
        //    _dxDevice = new Device(DriverType.Hardware, DeviceCreationFlags.BgraSupport, FeatureLevel.Level_10_0);
        //    GraphicsDevice = GraphicsDevice.New(_dxDevice);

        //    if (DeviceCreated != null) DeviceCreated(this, EventArgs.Empty);
        //}

        private void CreateDevice(System.Windows.Forms.Control control)
        {
            _deviceInformation = new GraphicsDeviceInformation()
            {
                GraphicsProfile = FeatureLevel.Level_11_0,
                DeviceCreationFlags = SharpDX.Direct3D11.DeviceCreationFlags.Debug,
                PresentationParameters = new PresentationParameters()
                {
                    BackBufferFormat = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
                    DepthStencilFormat = DepthFormat.Depth24Stencil8,
                    BackBufferWidth = 0,
                    BackBufferHeight = 0,
                    IsFullScreen = false,
                    RenderTargetUsage = SharpDX.DXGI.Usage.RenderTargetOutput,
                    MultiSampleCount = MSAALevel.None,
                    DeviceWindowHandle = control
                }
            };
            FindAdapter();
            GraphicsDevice = GraphicsDevice.New(_deviceInformation.Adapter, _deviceInformation.DeviceCreationFlags);

            //GraphicsDevice.Presenter = new SwapChainGraphicsPresenter(GraphicsDevice, _deviceInformation.PresentationParameters);

            _dxDevice = (Device) typeof(GraphicsDevice).GetField("Device",BindingFlags.Instance | BindingFlags.NonPublic).GetValue(GraphicsDevice);

            if (DeviceCreated != null) DeviceCreated(this, EventArgs.Empty);
        }

        public void ResetDevice()
        {
            if (DeviceReset != null) DeviceReset(this, EventArgs.Empty);
        }

        private void FindAdapter()
        {
            foreach (var graphicsAdapter in GraphicsAdapter.Adapters)
            {
                if (graphicsAdapter.IsProfileSupported(_deviceInformation.GraphicsProfile))
                {
                    _deviceInformation.Adapter = graphicsAdapter;
                    return;
                }
            }
            throw new Exception("Unable to find DirectX 11 compatible graphics adapter");
        }

        public void Release()
        {
            Disposer.SafeDispose(ref _dxDevice);
            GraphicsDevice.Dispose();
            GraphicsDevice = null;
        }

        public event EventHandler<EventArgs> DeviceCreated;

        public event EventHandler<EventArgs> DeviceDisposing;

        public event EventHandler<EventArgs> DeviceReset;

        public event EventHandler<EventArgs> DeviceResetting;

        public event EventHandler<EventArgs> DeviceChangeBegin;

        public event EventHandler<EventArgs> DeviceChangeEnd;

        public event EventHandler<EventArgs> DeviceLost;
    }
}
