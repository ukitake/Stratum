using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Toolkit;

namespace Stratum
{
    public interface IUpdate
    {
        void Update(GameTime gameTime);
    }
}
