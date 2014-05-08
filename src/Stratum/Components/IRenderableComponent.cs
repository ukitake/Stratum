using Stratum.Graphics.RenderCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Components
{
    public interface IRenderableComponent
    {
        IRenderCommand GenerateRenderCommand();
    }
}
