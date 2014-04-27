using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using Stratum.Input;
using Keys = SharpDX.Toolkit.Input.Keys;

namespace Stratum.Input
{
    public class KeyboardContext
    {
        private StratumKeyboardManager keyboard;

        private KeyboardState previousState;
        private KeyboardState currentState;

        public KeyboardContext(Form form)
        {
            keyboard = new StratumKeyboardManager(form);
            keyboard.Initialize();

            previousState = keyboard.GetState();
            currentState = previousState;
        }

        //public KeyboardContext(DirectInput directInput)
        //{
        //    //keyboard = new SharpDX.DirectInput.Keyboard(directInput);
        //    //keyboard.Acquire();
        //    //previousState = keyboard.GetCurrentState();
        //    //keyboard.Unacquire();
        //}

        public virtual void Update(GameTime gameTime)
        {
            previousState = currentState;
            currentState = keyboard.GetState();
        }

        public bool IsKeyDown(Keys key)
        {
            return currentState.IsKeyDown(key);
        }

        public bool WasKeyPressed(Keys key)
        {
            return previousState.IsKeyUp(key) && currentState.IsKeyDown(key);
        }

        public bool WasKeyReleased(Keys key)
        {
            return previousState.IsKeyDown(key) && currentState.IsKeyUp(key);
        }
    }
}
