using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.Graphics
{
    public class RenderContext : IRenderContext
    {
        public RenderContext()
        {
            CameraFrustum = new CompGeom.BoundingFrustum();
        }

        public GameTime Time { get; private set; }

        public Stratum.CompGeom.BoundingFrustum CameraFrustum { get; private set; }

        public Matrix View { get; private set; }
        public Matrix ViewNoTrans { get; private set; }
        public Matrix Proj { get; private set; }
        public Matrix ViewProj { get; private set; }

        public MatrixD ViewD { get; private set; }
        public MatrixD ViewNoTransD { get; private set; }
        public MatrixD ProjD { get; private set; }
        public MatrixD ViewProjD { get; private set; }

        public void Update(GameTime gameTime)
        {
            var gc = Engine.GraphicsContext;

            Time = gameTime;

            CameraFrustum.sbf = gc.CurrentCamera.Frustum;

            ViewD = gc.CurrentCamera.ViewD;
            ProjD = gc.CurrentCamera.ProjD;
            View = ViewD.ToMatrix();
            Proj = ProjD.ToMatrix();

            var viewCopy = View;
            viewCopy.TranslationVector = Vector3.Zero;
            ViewNoTrans = viewCopy;

            var viewDCopy = ViewD;
            viewDCopy.TranslationVector = Vector3D.Zero;
            ViewNoTransD = viewDCopy;

            ViewProj = Matrix.Multiply(View, Proj);
            ViewProjD = MatrixD.Multiply(ViewD, ProjD);
        }
    }
}
