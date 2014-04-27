using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Stratum.Input
{
    public class StratumMouseManager : SharpDX.Component, IGameSystem, IMouseService
    {
        private Form form;

        private SharpDX.Toolkit.Input.ButtonState left;
        private SharpDX.Toolkit.Input.ButtonState middle;
        private SharpDX.Toolkit.Input.ButtonState right;
        private SharpDX.Toolkit.Input.ButtonState xButton1;
        private SharpDX.Toolkit.Input.ButtonState xButton2;
        private int wheelDelta;

        // provides platform-specific binding to mouse functionality
        private StratumMousePlatform platform;

        public StratumMouseManager(Form form)
        {
            this.form = form;
        }

        public void Initialize()
        {
            // create platform-specific instance
            platform = StratumMousePlatform.Create(form);

            // platform will report state changes trough these events:
            platform.MouseDown += HandleMouseDown;
            platform.MouseUp += HandleMouseUp;
            platform.MouseWheelDelta += HandleWheelDelta;
        }

        public MouseState GetState()
        {
            // read the mouse position information
            var position = platform.GetLocation();
            int wheel = this.wheelDelta;
            this.wheelDelta = 0;
            return new MouseState(left, middle, right, xButton1, xButton2, position.X, position.Y, wheel);
        }

        public void SetPosition(Vector2 point)
        {
            if (platform == null)
                throw new InvalidOperationException("MouseManager is not initialized.");

            platform.SetLocation(point);
        }

        private void HandleMouseDown(MouseButton button)
        {
            SetButtonStateTo(button, SharpDX.Toolkit.Input.ButtonState.Pressed);
        }

        private void HandleMouseUp(MouseButton button)
        {
            SetButtonStateTo(button, SharpDX.Toolkit.Input.ButtonState.Released);
        }

        private void HandleWheelDelta(int wheelDelta)
        {
            this.wheelDelta = wheelDelta;
        }

        private void SetButtonStateTo(MouseButton button, SharpDX.Toolkit.Input.ButtonState state)
        {
            switch (button)
            {
                case MouseButton.None:
                    break;
                case MouseButton.Left:
                    left = state;
                    break;
                case MouseButton.Middle:
                    middle = state;
                    break;
                case MouseButton.Right:
                    right = state;
                    break;
                case MouseButton.XButton1:
                    xButton1 = state;
                    break;
                case MouseButton.XButton2:
                    xButton2 = state;
                    break;
                default:
                    throw new ArgumentOutOfRangeException("button");
            }
        }
    }
}
