using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Stratum.Graphics
{
    public interface IFormsGraphicsService
    {
        Form Form { get; }
        void ResizeSwapChain(int width, int height);
    }
}
