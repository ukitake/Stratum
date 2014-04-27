using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Stratum.Input
{
    internal enum MouseButton
    {
        /// <summary>
        /// No button ('null-object' pattern)
        /// </summary>
        None,

        /// <summary>
        /// Left mouse button
        /// </summary>
        Left,

        /// <summary>
        /// Middle mouse button
        /// </summary>
        Middle,

        /// <summary>
        /// Right mouse button
        /// </summary>
        Right,

        /// <summary>
        /// Mouse X-Button 1
        /// </summary>
        XButton1,

        /// <summary>
        /// Mouse X-Button 2
        /// </summary>
        XButton2
    }

    public abstract class StratumMousePlatform
    {
        private readonly object nativeControl;

        protected StratumMousePlatform(object nativeControl)
        {
            if (nativeControl == null) throw new ArgumentNullException("nativeControl");

            this.nativeControl = nativeControl;
            BindWindow(nativeControl);
        }

        internal static StratumMousePlatform Create(object nativeControl)
        {
            if (nativeControl is System.Windows.Forms.Control || nativeControl is IntPtr)
                return new StratumMousePlatformDesktop(nativeControl); // WinForms platform

            return null;
        }

        internal event Action<MouseButton> MouseDown;
        internal event Action<MouseButton> MouseUp;
        internal event Action<int> MouseWheelDelta;

        internal Vector2 GetLocation()
        {
            return GetLocationInternal();
        }

        internal virtual void SetLocation(Vector2 point) { }

        protected abstract void BindWindow(object nativeControl);

        protected abstract Vector2 GetLocationInternal();
        
        internal void OnMouseDown(MouseButton button)
        {
            Raise(MouseDown, button);
        }

        internal void OnMouseUp(MouseButton button)
        {
            Raise(MouseUp, button);
        }

        protected void OnMouseWheel(int wheelDelta)
        {
            Raise(MouseWheelDelta, wheelDelta);
        }

        private static void Raise<TArg>(Action<TArg> handler, TArg argument)
        {
            if (handler != null)
                handler(argument);
        }
    }
}
