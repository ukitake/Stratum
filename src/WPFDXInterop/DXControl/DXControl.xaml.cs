using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Reflection;
using System.Windows.Threading;

using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Content;
using System.Timers;

namespace WPFDXInterop
{
    /// <summary>
    /// Interaction logic for DXControl.xaml
    /// </summary>
    public partial class DXControl : UserControl
    {
        private bool _loaded;

        public DXGraphicsDeviceService _graphicsDeviceService;
        private ContentManager _contentManager;
        private GameServiceRegistry _services;

        private DXImageSource _imageSource;
        private RenderTargetGraphicsPresenter _imageSourcePresenter;

        public event EventHandler<EventArgs> Resized;
        public event EventHandler<DXGraphicsDeviceEventArgs> LoadContent;
        public event EventHandler<DXGraphicsDeviceEventArgs> Update;
        public event EventHandler<DXGraphicsDeviceEventArgs> Draw;

        private bool _ready;

        public DXControl()
        {
            InitializeComponent();
            Loaded += DXControl_Loaded;
        }

        public GraphicsDevice GraphicsDevice
        {
            get { return _graphicsDeviceService.GraphicsDevice; }
        }

        public ContentManager ContentManager
        {
            get { return _contentManager; }
        }

        public GameServiceRegistry Services
        {
            get { return _services; }
        }

        ~DXControl()
        {
            if (_imageSource != null) _imageSource.Dispose();
            if (_graphicsDeviceService != null) _graphicsDeviceService.Release();
            if (_imageSourcePresenter != null) _imageSourcePresenter.Dispose();
        }

        private void DXControl_Loaded(object sender, EventArgs args)
        {
            _services = new GameServiceRegistry();
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                InitializeGraphicsDevice();
                InitializeContentManager();

                if (LoadContent != null)
                    LoadContent(this, new DXGraphicsDeviceEventArgs(GraphicsDevice));
            }
            _imageSourcePresenter = new RenderTargetGraphicsPresenter(GraphicsDevice, _imageSource.RenderTarget);
            CompositionTarget.Rendering += CompositionTarget_Rendering;
            _ready = true;
        }


        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && _graphicsDeviceService != null)
            {
                DoResize();
                Invalidate();
            }
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void DoResize()
        {
            if (_imageSource != null)
            {
                _imageSource.Dispose();
                _imageSource = new DXImageSource(
                    GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
                _imageSourcePresenter = new RenderTargetGraphicsPresenter(GraphicsDevice, _imageSource.RenderTarget);

                rootImage.Source = _imageSource.WriteableBitmap;
                if (Resized != null)
                {
                    Resized(this, EventArgs.Empty);
                }
            }
        }

        private void InitializeContentManager()
        {
            _contentManager = new ContentManager(Services);
            _contentManager.Resolvers.Add(new FileSystemContentResolver(GetDefaultAppDirectory()));
            Services.AddService(typeof(IServiceRegistry), Services);
            Services.AddService(typeof(IContentManager), ContentManager);
            ContentManager.RootDirectory = "Content";
        }

        private string GetDefaultAppDirectory()
        {
            var assemblyUri = new Uri(Assembly.GetEntryAssembly().CodeBase);
            return System.IO.Path.GetDirectoryName(assemblyUri.LocalPath);
        }

        private void InitializeGraphicsDevice()
        {
            if (_graphicsDeviceService == null)
            {
                _graphicsDeviceService = new DXGraphicsDeviceService();

                // create the image source
                _imageSource = new DXImageSource(
                    GraphicsDevice, (int)ActualWidth, (int)ActualHeight);
                rootImage.Source = _imageSource.WriteableBitmap;

                Services.AddService(typeof(IGraphicsDeviceService), _graphicsDeviceService);
            }
        }

        void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            RenderControl();
        }

        private void RenderControl()
        {
            if (Update != null)
                Update(this, new DXGraphicsDeviceEventArgs(GraphicsDevice));

            // set the image source render target
            GraphicsDevice.SetBlendState(GraphicsDevice.BlendStates.AlphaBlend);
            GraphicsDevice.SetRenderTargets(_imageSource.DepthStencilView, _imageSource.RenderTargetView);

            GraphicsDevice.SetViewports(new ViewportF(0, 0, _imageSource.RenderTarget.Width, _imageSource.RenderTarget.Height));
            GraphicsDevice.Presenter = _imageSourcePresenter;

            GraphicsDevice.Clear(_imageSource.DepthStencilView, SharpDX.Direct3D11.DepthStencilClearFlags.Depth | SharpDX.Direct3D11.DepthStencilClearFlags.Stencil, 1, 0);
            GraphicsDevice.Clear(_imageSource.RenderTargetView, Color4.Black);

            GraphicsDevice.SetDepthStencilState(GraphicsDevice.DepthStencilStates.Default);

            // allow the control to draw
            if (Draw != null)
                Draw(this, new DXGraphicsDeviceEventArgs(GraphicsDevice));

            GraphicsDevice.Present();

            // unset the render target
            GraphicsDevice.ResetTargets();

            // commit the changes to the image source
            _imageSource.Commit();
        }

        // The control is currently configured to update base on the CompositionTarget.Rendering event
        // which will render the control as fast as possible, however if you just want to render as required 
        // uncomment the following and remove this line CompositionTarget.Rendering += CompositionTarget_Rendering;
        // in DXControl_Loaded. You can then force a render by calling (DXControl).InvalidateVisual

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    //base.OnRender(drawingContext);
        //    if (_ready)
        //    {
        //        RenderControl();
        //    }
        //}

        public void Invalidate()
        {
            this.InvalidateVisual();
        }
    }
}
