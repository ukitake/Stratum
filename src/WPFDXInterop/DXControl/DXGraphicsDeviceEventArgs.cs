using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit.Graphics;

namespace WPFDXInterop
{
    public class DXGraphicsDeviceEventArgs : EventArgs
    {
        /// <summary>
        /// Gets the GraphicsDevice.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }

        /// <summary>
        /// Initializes a new GraphicsDeviceEventArgs.
        /// </summary>
        /// <param name="device">The GraphicsDevice associated with the event.</param>
        public DXGraphicsDeviceEventArgs(GraphicsDevice device)
        {
            GraphicsDevice = device;
        }
    }
}
