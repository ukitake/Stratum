using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit.Graphics;

namespace Stratum.Graphics
{
    /// <summary>
    /// Interface for interop with WPF... used for resizing the DirectX swap chain according to wpf
    /// control size changes
    /// </summary>
    public interface IWpfGraphicsDevice
    {
        RenderTarget2D BackBuffer { get; }
        void Resize(int width, int height);
    }
}
