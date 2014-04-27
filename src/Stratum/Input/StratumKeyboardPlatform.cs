using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit.Input;

namespace Stratum.Input
{
    public abstract class StratumKeyboardPlatform
    {
        protected StratumKeyboardPlatform(object nativeControl)
        {
            if (nativeControl == null) throw new ArgumentNullException("nativeControl");
            BindWindow(nativeControl);
        }

        internal static StratumKeyboardPlatform Create(object nativeControl)
        {
            if (nativeControl is System.Windows.Forms.Control || nativeControl is IntPtr)
            {
                return new StratumKeyboardPlatformDesktop(nativeControl);
            }

            // TODO: WPF input handling platform

            return null;
        }

        internal event Action<Keys> KeyPressed;
        internal event Action<Keys> KeyReleased;

        protected abstract void BindWindow(object nativeControl);

        protected void RaiseKeyPressed(Keys key)
        {
            if (key == Keys.None) return;
            Raise(KeyPressed, key);
        }

        protected void RaiseKeyReleased(Keys key)
        {
            if (key == Keys.None) return;
            Raise(KeyReleased, key);
        }

        private static void Raise<TArg>(Action<TArg> handler, TArg argument)
        {
            if (handler != null)
                handler(argument);
        }
    }
}
