using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX.DirectInput;
using SharpDX.Toolkit;
using SharpDX.XInput;

namespace Stratum.Input
{
    public class InputContext : IInputContext
    {
        public InputContext()
        {
            Gamepad = new GamepadContext();
            //Mouse = new MouseContext();
            //Keyboard = new KeyboardContext();
        }

        public InputContext(Form form)
        {
            Gamepad = new GamepadContext();
            Mouse = new MouseContext(form);
            Keyboard = new KeyboardContext(form);
        }

        public void Update(GameTime gameTime)
        {
            Gamepad.Update(gameTime);
            Mouse.Update(gameTime);
            Keyboard.Update(gameTime);
        }

        public GamepadContext Gamepad { get; private set; }

        public MouseContext Mouse { get; private set; }

        public KeyboardContext Keyboard { get; private set; }
    }
}
