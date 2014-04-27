using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit.Graphics;

namespace WPFDXInterop
{
    public interface IImageSourcePresenter
    {
        bool IsResizing { get; set; }
        void Commit(RenderTarget2D renderTarget);
    }
}
