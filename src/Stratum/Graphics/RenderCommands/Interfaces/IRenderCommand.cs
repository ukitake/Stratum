using SharpDX;
using SharpDX.Toolkit.Graphics;
using Stratum.CompGeom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics.RenderCommands
{
    public interface IRenderCommand
    {
        Matrix World { get; }

        PrimitiveType PrimitiveType { get; }
        Effect Effect { get; }
        string Technique { get; }

        BlendState BlendState { get; }
        DepthStencilState DepthStencilState { get; }
        RasterizerState RasterizerState { get; }

        void SetGeometry(IGraphicsContext context);
        void SetResources(IGraphicsContext context);
        void SetConstants(IGraphicsContext context);

        void Render(IGraphicsContext context);
    }
}
