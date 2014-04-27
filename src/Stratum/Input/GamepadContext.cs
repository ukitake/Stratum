using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.XInput;

namespace Stratum.Input
{
    public class GamepadContext
    {
        public GamepadContext()
        {
            controller = new Controller(UserIndex.One);
            SetCurrentState();
            SetPreviousState();
        }

        private Controller controller;
        private State previousState;
        private State currentState;

        public bool WasButtonPressed(GamepadButtonFlags button)
        {
            return !WasButtonDown(button) && IsButtonDown(button);
        }

        public bool IsButtonDown(GamepadButtonFlags button)
        {
            return currentState.Gamepad.Buttons.HasFlag(button);
        }

        public bool WasButtonDown(GamepadButtonFlags button)
        {
            return previousState.Gamepad.Buttons.HasFlag(button);
        }

        public Vector2 LStick()
        {
            short x = currentState.Gamepad.LeftThumbX;
            short y = currentState.Gamepad.LeftThumbY;
            return leftStickToVector2(x, y);
        }

        public Vector2 RStick()
        {
            short x = currentState.Gamepad.RightThumbX;
            short y = currentState.Gamepad.RightThumbY;
            return rightStickToVector2(x, y);
        }

        private Vector2 leftStickToVector2(short x, short y)
        {
            // stick shorts go from -32768 to 32767
            // map that range to -1 to 1 considering the dead zones
            float fx = 0f, fy = 0f;
            if (x < -Gamepad.LeftThumbDeadZone)
                fx = (float)x / 32768f;
            else if (x > Gamepad.LeftThumbDeadZone)
                fx = (float)x / 32767f;

            if (y < -Gamepad.LeftThumbDeadZone)
                fy = (float)y / 32768f;
            else if (y > Gamepad.LeftThumbDeadZone)
                fy = (float)y / 32767f;

            return new Vector2(fx, fy);
        }

        private Vector2 rightStickToVector2(short x, short y)
        {
            // stick shorts go from -32768 to 32767
            // map that range to -1 to 1 considering the dead zones
            float fx = 0f, fy = 0f;
            if (x < -Gamepad.RightThumbDeadZone)
                fx = (float)x / 32768f;
            else if (x > Gamepad.RightThumbDeadZone)
                fx = (float)x / 32767f;

            if (y < -Gamepad.RightThumbDeadZone)
                fy = (float)y / 32768f;
            else if (y > Gamepad.RightThumbDeadZone)
                fy = (float)y / 32767f;

            return new Vector2(fx, fy);
        }

        public byte LTrigger()
        {
            return currentState.Gamepad.LeftTrigger;
        }

        public byte RTrigger()
        {
            return currentState.Gamepad.RightTrigger;
        }

        public void Update(GameTime gameTime)
        {
            SetPreviousState();
            SetCurrentState();
        }

        public void SetCurrentState()
        {
            if (controller.IsConnected)
                currentState = controller.GetState();
        }

        public void SetPreviousState()
        {
            previousState = currentState;
        }
    }
}
