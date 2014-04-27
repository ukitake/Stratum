using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit.Graphics;

namespace Stratum.Graphics
{
    public static class TextureHelpers
    {
        public static Texture2D TextureFromBitmap(System.Drawing.Bitmap bmp)
        {
            int[] imgData = new int[bmp.Width * bmp.Height];
            Texture2D texture;
            texture = Texture2D.New(Engine.GraphicsContext.Device, bmp.Width, bmp.Height, PixelFormat.R8G8B8A8.UNorm);

            unsafe
            {
                // lock bitmap
                System.Drawing.Imaging.BitmapData origdata =
                    bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                uint* byteData = (uint*)origdata.Scan0;

                // Switch bgra -> rgba
                for (int i = 0; i < imgData.Length; i++)
                {
                    byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);
                }

                // copy data
                System.Runtime.InteropServices.Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

                byteData = null;

                // unlock bitmap
                bmp.UnlockBits(origdata);
            }

            texture.SetData(imgData);

            return texture;
        }

        public static Texture2D TextureFromBitmapThreadSafe(System.Drawing.Bitmap bmp)
        {
            int[] imgData = new int[bmp.Width * bmp.Height];
            Texture2D texture;
            texture = Texture2D.New(Engine.GraphicsContext.Device, bmp.Width, bmp.Height, PixelFormat.R8G8B8A8.UNorm);

            unsafe
            {
                // lock bitmap
                System.Drawing.Imaging.BitmapData origdata =
                    bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, bmp.PixelFormat);

                uint* byteData = (uint*)origdata.Scan0;

                // Switch bgra -> rgba
                for (int i = 0; i < imgData.Length; i++)
                {
                    byteData[i] = (byteData[i] & 0x000000ff) << 16 | (byteData[i] & 0x0000FF00) | (byteData[i] & 0x00FF0000) >> 16 | (byteData[i] & 0xFF000000);
                }

                // copy data
                System.Runtime.InteropServices.Marshal.Copy(origdata.Scan0, imgData, 0, bmp.Width * bmp.Height);

                byteData = null;

                // unlock bitmap
                bmp.UnlockBits(origdata);
            }

            // SetData uses the D3D11 Device Context which is not thread safe... execute 
            // the SetData inside a critical section using the GraphicsDevice as the mutex
            Engine.GraphicsContext.ExclusiveDeviceExec(() => texture.SetData(imgData));

            return texture;
        }
    }
}
