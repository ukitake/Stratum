using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.XInput;

namespace Stratum.Input
{
    public interface IInputContext : IUpdate
    {
        GamepadContext Gamepad { get; }
        MouseContext Mouse { get; }
        KeyboardContext Keyboard { get; }
    }
}
