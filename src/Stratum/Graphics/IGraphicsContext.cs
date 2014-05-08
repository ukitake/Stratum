using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.Toolkit.Graphics;
using Stratum.Graphics;
using WPFDXInterop;

namespace Stratum.Graphics
{
    public interface IGraphicsContext
    {
        IGraphicsDeviceService GraphicsService { get; }
        IImageSourcePresenter ImageSourcePresenter { get; set; }

        IRenderContext RenderContext { get; }

        GraphicsDevice Device { get; }
        SpriteBatch SpriteBatch { get; }

        Device D3D11Device { get; }
        DeviceContext D3D11Context { get; }

        GeometryBuffer GBuffer { get; }

        float AspectRatio { get; }
        Camera CurrentCamera { get; set; }

        void ExclusiveDeviceExec(Action actionThatNeedsExclusiveDevice);
    }
}
