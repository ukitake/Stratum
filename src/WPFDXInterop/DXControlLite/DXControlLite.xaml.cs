using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace WPFDXInterop
{
    /// <summary>
    /// The difference between DXControl and DXControlLite is that DXControl spins up and manages its own SharpDX Game
    /// whereas DXControlLite only presents the BackBuffer of the IGraphicsDeviceService it has been given
    /// </summary>
    public partial class DXControlLite : UserControl
    {
        private IGraphicsDeviceService deviceService;
        private DXImageSourceLite imageSource;

        /// <summary>
        /// Should not be used... will not work
        /// </summary>
        public DXControlLite()
        {
            InitializeComponent();
        }

        public DXControlLite(IGraphicsDeviceService deviceService)
        {
            InitializeComponent();

            this.deviceService = deviceService;
            this.Loaded += DXControlLite_Loaded;

            this.deviceService.DeviceCreated += deviceService_DeviceCreated;
            this.deviceService.DeviceLost += deviceService_DeviceLost;
            this.deviceService.DeviceDisposing += deviceService_DeviceDisposing;
        }

        /// <summary>
        /// Presenter.Commit() should be called after each render pass of whatever external engine 
        /// this control is being used to present.
        /// </summary>
        public IImageSourcePresenter Presenter { get { return imageSource; } }

        private void DXControlLite_Loaded(object sender, RoutedEventArgs e)
        {
            imageSource = new DXImageSourceLite(deviceService.GraphicsDevice, (int)this.ActualWidth, (int)this.ActualHeight);
            rootImage.Source = imageSource.WriteableBitmap;
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (!DesignerProperties.GetIsInDesignMode(this) && deviceService != null)
            {
                DoResize();
                Invalidate();
            }
            base.OnRenderSizeChanged(sizeInfo);
        }

        private void DoResize()
        {
            if (imageSource != null)
            {
                rootImage.Source = null;

                imageSource.Resize((int)ActualWidth, (int)ActualHeight);
                rootImage.Source = imageSource.WriteableBitmap;
            }
        }

        private void deviceService_DeviceCreated(object sender, EventArgs e)
        {

        }

        private void deviceService_DeviceLost(object sender, EventArgs e)
        {

        }

        private void deviceService_DeviceDisposing(object sender, EventArgs e)
        {

        }

        public void Invalidate()
        {
            this.InvalidateVisual();
        }
    }
}
