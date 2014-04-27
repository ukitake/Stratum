using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Stratum.Input
{
    public class StratumKeyboardManager : SharpDX.Component, IGameSystem, IComponent, IKeyboardService
    {
        private Form form;

        private readonly HashSet<SharpDX.Toolkit.Input.Keys> pressedKeys = new HashSet<SharpDX.Toolkit.Input.Keys>();

        private StratumKeyboardPlatform platform;

        public StratumKeyboardManager(Form form)
        {
            this.form = form;
        }

        public void Initialize()
        {
            platform = StratumKeyboardPlatform.Create(form);

            platform.KeyPressed += platform_KeyPressed;
            platform.KeyReleased += platform_KeyReleased;
        }

        void platform_KeyPressed(SharpDX.Toolkit.Input.Keys key)
        {
            pressedKeys.Add(key);
        }

        void platform_KeyReleased(SharpDX.Toolkit.Input.Keys key)
        {
            pressedKeys.Remove(key);
        }

        public KeyboardState GetState()
        {
            // constructor is internal
            return (KeyboardState)Activator.CreateInstance(typeof(KeyboardState), BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { pressedKeys }, null);
        }
    }
}
