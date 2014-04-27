using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpDX;
using Cursor = System.Windows.Forms.Cursor;
using MouseEventArgs = System.Windows.Forms.MouseEventArgs;

namespace Stratum.Input
{
    public class StratumMousePlatformDesktop : StratumMousePlatform
    {
        private Control control;

        public StratumMousePlatformDesktop(object nativeControl)
            : base(nativeControl)
        {
        }

        internal override void SetLocation(Vector2 point)
        {
            point.Saturate();
            var clientSize = control.ClientSize;
            Cursor.Position = control.PointToScreen(new System.Drawing.Point((int)(point.X * clientSize.Width), (int)(point.Y * clientSize.Height)));
        }

        protected override void BindWindow(object nativeControl)
        {
            if (nativeControl == null) throw new ArgumentNullException("nativeWindow");

            control = nativeControl as Control;
            if (control == null && nativeControl is IntPtr)
            {
                control = Control.FromHandle((IntPtr)nativeControl);
            }

            if (control == null)
                throw new InvalidOperationException(string.Format("Unsupported native window: {0}", nativeControl));

            control.MouseDown += HandleMouseDown;
            control.MouseUp += HandleMouseUp;
            control.MouseMove += HandleMouseMove;
            control.MouseWheel += HandleMouseWheel;
        }

        protected override Vector2 GetLocationInternal()
        {
            System.Drawing.Point p = System.Drawing.Point.Empty;
            if (control.InvokeRequired)
            {
                control.Invoke(new Action(delegate { p = control.PointToClient(Cursor.Position); }));
            }
            else
            {
                p = control.PointToClient(Cursor.Position);
            }

            var clientSize = control.ClientSize;
            var position = new Vector2((float)p.X / clientSize.Width, (float)p.Y / clientSize.Height);
            position.Saturate();
            return position;
        }

        private void HandleMouseDown(object sender, MouseEventArgs e)
        {
            OnMouseDown(TranslateButton(e.Button));
            OnMouseWheel(e.Delta);
        }

        private void HandleMouseUp(object sender, MouseEventArgs e)
        {
            OnMouseUp(TranslateButton(e.Button));
            OnMouseWheel(e.Delta);
        }

        private void HandleMouseMove(object sender, MouseEventArgs e)
        {
            OnMouseWheel(e.Delta);
        }

        private void HandleMouseWheel(object sender, MouseEventArgs e)
        {
            OnMouseWheel(e.Delta);
        }

        private static MouseButton TranslateButton(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return MouseButton.Left;
                case MouseButtons.None:
                    return MouseButton.None;
                case MouseButtons.Right:
                    return MouseButton.Right;
                case MouseButtons.Middle:
                    return MouseButton.Middle;
                case MouseButtons.XButton1:
                    return MouseButton.XButton1;
                case MouseButtons.XButton2:
                    return MouseButton.XButton2;
                default:
                    throw new ArgumentOutOfRangeException("button");
            }
        }
    }
}
