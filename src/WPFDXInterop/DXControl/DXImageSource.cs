using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using SharpDX.Direct3D11;

namespace WPFDXInterop
{
    public class DXImageSource : IDisposable
    {
        private RenderTargetView _renderTargetView;
        private RenderTarget2D _renderTarget;

        //private DepthStencilBuffer _depthStencilBuffer;
        private DepthStencilView _depthStencilView;

        private SharpDX.Toolkit.Graphics.Texture2D _stagingTexture;
        private WriteableBitmap _writeableBitmap;
        private byte[] _buffer;

        public RenderTarget2D RenderTarget
        {
            get { return _renderTarget; }
        }

        public RenderTargetView RenderTargetView
        {
            get { return _renderTargetView; }
        }

        //public DepthStencilBuffer DepthStencilBuffer
        //{
        //    get { return _depthStencilBuffer; }
        //}

        public DepthStencilView DepthStencilView
        {
            get { return _depthStencilView; }
        }

        public WriteableBitmap WriteableBitmap
        {
            get { return _writeableBitmap; }
        }

        public DXImageSource(GraphicsDevice graphics, int width, int height)
        {
            if (width < 10) width = 10;
            if (height < 10) height = 10;

            _renderTarget = RenderTarget2D.New(graphics, width, height, SharpDX.Toolkit.Graphics.PixelFormat.B8G8R8A8.UNorm);
            _renderTargetView = new RenderTargetView(graphics, (SharpDX.Toolkit.Graphics.GraphicsResource)_renderTarget);

            SharpDX.Direct3D11.Texture2D depthBuffer = new SharpDX.Direct3D11.Texture2D(graphics, new Texture2DDescription()
            {
                Format = SharpDX.DXGI.Format.D24_UNorm_S8_UInt,
                ArraySize=1,
                MipLevels=0,
                Width = width,
                Height = height,
                SampleDescription = new SharpDX.DXGI.SampleDescription(1,0),
                BindFlags = SharpDX.Direct3D11.BindFlags.DepthStencil
            });

            //_depthStencilBuffer = DepthStencilBuffer.New(graphics,width,height,DepthFormat.Depth24Stencil8);
            _depthStencilView = new DepthStencilView(graphics, depthBuffer);

            Texture2DDescription description = new Texture2DDescription()
            {
                Width = width,
                Height = height,
                MipLevels = 1,
                ArraySize = 1,
                Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm,
                BindFlags = BindFlags.None,
                CpuAccessFlags = CpuAccessFlags.Read,
                SampleDescription = new SharpDX.DXGI.SampleDescription()
                {
                    Count = 1,
                    Quality = 0
                },
                Usage = ResourceUsage.Staging,
                OptionFlags = ResourceOptionFlags.None
            };
            _stagingTexture = SharpDX.Toolkit.Graphics.Texture2D.New(graphics, description);

            _buffer = new byte[width * height * 4];
            _writeableBitmap = new WriteableBitmap(
                width, height, 96, 96, PixelFormats.Bgr32, null);
        }

        ~DXImageSource()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_renderTarget != null) _renderTarget.Dispose();
            if (_renderTargetView != null) _renderTargetView.Dispose();
            //if (_depthStencilBuffer != null) _depthStencilBuffer.Dispose();
            if (_depthStencilView != null) _depthStencilView.Dispose();
            if (_stagingTexture!=null) _stagingTexture.Dispose();
            if (_writeableBitmap != null) _writeableBitmap = null;
            if (_buffer != null) _buffer = null;

            //if (disposing) GC.SuppressFinalize(this);
        }

        public unsafe void Commit()
        {
            _renderTarget.GetData(_stagingTexture, _buffer);
            _writeableBitmap.Lock();

            fixed (byte* buffer = _buffer)
            {
                SharpDX.Utilities.CopyMemory(_writeableBitmap.BackBuffer, new IntPtr(buffer), _buffer.Length);
            }

            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, _renderTarget.Width, _renderTarget.Height));
            _writeableBitmap.Unlock();
        }

        public void Build(RenderTarget2D renderTarget)
        {
            renderTarget.GetData(_stagingTexture, _buffer);
            _writeableBitmap.Lock();
            Marshal.Copy(_buffer, 0, _writeableBitmap.BackBuffer, _buffer.Length);
            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, _renderTarget.Width, _renderTarget.Height));
            _writeableBitmap.Unlock();
        }
    }
}
