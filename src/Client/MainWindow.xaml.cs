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
using Stratum;
using Stratum.Graphics;
using WPFDXInterop;

namespace Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DXControlLite dxControl;

        public MainWindow()
        {
            InitializeComponent();

            Engine.Instance.Initialize();
            IGraphicsContext context = Engine.GraphicsContext;
            dxControl = new DXControlLite(context.GraphicsService)
            {
                HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch,
                VerticalAlignment = System.Windows.VerticalAlignment.Stretch
            };
            dxControl.Loaded += delegate
            {
                context.ImageSourcePresenter = dxControl.Presenter;

                Engine.Instance.Start();
            };
            
            dxControl.SizeChanged += dxControl_SizeChanged;

            mainPanel.Children.Add(dxControl);
        }

        void dxControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //Engine.Pause();
            IGraphicsContext context = Engine.GraphicsContext;
            IWpfGraphicsDevice device = Engine.GraphicsDeviceService;
            device.Resize((int)e.NewSize.Width, (int)e.NewSize.Height);
            //Engine.UnPause();
        }
    }
}
