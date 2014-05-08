using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics
{
    public interface IRenderContext : IUpdate
    {
        GameTime Time { get; }

        Stratum.CompGeom.BoundingFrustum CameraFrustum { get; } 

        Matrix View { get; }
        Matrix ViewNoTrans { get; }
        Matrix Proj { get; }
        Matrix ViewProj { get; }

        MatrixD ViewD { get; }
        MatrixD ViewNoTransD { get; }
        MatrixD ProjD { get; }
        MatrixD ViewProjD { get; }
    }
}
