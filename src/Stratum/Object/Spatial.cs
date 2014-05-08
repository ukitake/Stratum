using SharpDX.Toolkit;
using Stratum.CompGeom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum
{
    public abstract class Spatial : IUpdate
    {
        private MatrixD local;
        private MatrixD world;

        internal Spatial()
        {
            Local = MatrixD.Identity;
            World = MatrixD.Identity;
        }

        public MatrixD Local 
        {
            get { return local; }
            set
            {
                if (local == value) return;

                local = value;
                WorldIsCurrent = false;
            }
        }

        public MatrixD World
        {
            get { return world; }
            protected set
            {
                if (world == value) return;

                world = value;
                WorldBoundIsCurrent = false;
            }
        }

        /// <summary>
        /// False when Local has been changed and thus World needs to be changed
        /// </summary>
        protected bool WorldIsCurrent { get; set; }

        public BoundingVolume WorldBound { get; set; }
        protected bool WorldBoundIsCurrent { get; set; }

        public abstract void Update(GameTime gameTime);
        public abstract void PostUpdate(GameTime gameTime);
    }
}
