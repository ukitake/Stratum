using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Graphics;

namespace WPFDXInterop
{
    public class DXImageSourceLite : IImageSourcePresenter, IDisposable
    {
        private GraphicsDevice graphicsDevice;
        private SharpDX.Toolkit.Graphics.Texture2D _stagingTexture;
        private WriteableBitmap _writeableBitmap;
        private byte[] _buffer;

        public WriteableBitmap WriteableBitmap
        {
            get { return _writeableBitmap; }
        }

        public DXImageSourceLite(GraphicsDevice graphics, int width, int height)
        {
            graphicsDevice = graphics;

            initResources(width, height);
        }

        ~DXImageSourceLite()
        {
            Dispose(false);
        }

        private void initResources(int width, int height)
        {
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

            _stagingTexture = SharpDX.Toolkit.Graphics.Texture2D.New(graphicsDevice, description);

            _buffer = new byte[width * height * 4];
            _writeableBitmap = new WriteableBitmap(
                width, height, 96, 96, PixelFormats.Bgra32, null);
        }

        public bool IsResizing
        {
            get { return resizing; }
            set { resizing = value; }
        }

        volatile bool resizing = false;
        public void Resize(int width, int height)
        {
            resizing = true;
            _stagingTexture.Dispose();
            initResources(width, height);
            resizing = false;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_stagingTexture != null) _stagingTexture.Dispose();
            if (_writeableBitmap != null) _writeableBitmap = null;
            if (_buffer != null) _buffer = null;

            //if (disposing) GC.SuppressFinalize(this);
        }

        public unsafe void Commit(RenderTarget2D renderTarget)
        {
            if (resizing)
                return;

            recreateStagingResourcesIfNeeded(renderTarget);

            renderTarget.GetData(_stagingTexture, _buffer);
            Application.Current.Dispatcher.Invoke(new Action(delegate
            {
                _writeableBitmap.Lock();

                fixed (byte* buffer = _buffer)
                {
                    SharpDX.Utilities.CopyMemory(_writeableBitmap.BackBuffer, new IntPtr(buffer), _buffer.Length);
                }

                _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, _stagingTexture.Width, _stagingTexture.Height));
                _writeableBitmap.Unlock();
            }));
        }

        private void recreateStagingResourcesIfNeeded(RenderTarget2D renderTarget)
        {
            if (_stagingTexture.Width != renderTarget.Width || _stagingTexture.Height != renderTarget.Height)
            {
                _stagingTexture.Dispose();
                initResources(renderTarget.Width, renderTarget.Height);
            }
        }
    }
}
