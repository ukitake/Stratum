using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;
using Stratum.Input;
using ButtonState = SharpDX.Toolkit.Input.ButtonState;

namespace Stratum.Input
{
    public class MouseContext
    {
        private StratumMouseManager mouse;

        private MouseState previousState;
        private MouseState currentState;

        public MouseContext(Form form)
        {
            mouse = new StratumMouseManager(form);
            mouse.Initialize();

            previousState = mouse.GetState();
            currentState = previousState;
        }

        public void Update(GameTime gameTime)
        {
            previousState = currentState;
            currentState = mouse.GetState();
        }

        public bool IsLeftButtonDown
        {
            get { return currentState.Left == ButtonState.Pressed; }
        }

        public bool IsRightButtonDown
        {
            get { return currentState.Right == ButtonState.Pressed; }
        }

        public bool IsMiddleButtonDown
        {
            get { return currentState.Middle == ButtonState.Pressed; }
        }

        public bool WasLeftButtonPressed
        {
            get { return previousState.Left == ButtonState.Released && currentState.Left == ButtonState.Pressed; }
        }

        public bool WasRightButtonPressed
        {
            get { return previousState.Right == ButtonState.Released && currentState.Right == ButtonState.Pressed; }
        }

        public bool WasMiddleButtonPressed
        {
            get { return previousState.Middle == ButtonState.Released && currentState.Middle == ButtonState.Pressed; }
        }

        public bool WasLeftButtonReleased
        {
            get { return previousState.Left == ButtonState.Pressed && currentState.Left == ButtonState.Released; }
        }

        public bool WasRightButtonReleased
        {
            get { return previousState.Right == ButtonState.Pressed && currentState.Right == ButtonState.Released; }
        }

        public bool WasMiddleButtonReleased
        {
            get { return previousState.Middle == ButtonState.Pressed && currentState.Middle == ButtonState.Released; }
        }

        public int WheelDelta
        {
            get { return currentState.WheelDelta; }
        }

        public int WheelAcceleration
        {
            get { return currentState.WheelDelta - previousState.WheelDelta; }
        }

        public Vector2 Location { get { return new Vector2(currentState.X, currentState.Y); } }
    }
}
