using SharpDX.Toolkit;
using Stratum.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum
{
    public abstract class Component : IUpdate, IRender
    {
        internal Component()
        {
        }

        public GameObject Object { get; internal set; }

        public ulong Id { get; internal set; }

        /// <summary>
        /// Indicates whether this component has had its Update method called this frame
        /// </summary>
        public bool Updated { get; set; }

        public virtual void Update(GameTime gameTime)
        {
            Updated = true;
        }

        public virtual void PostUpdate(GameTime gameTime)
        {
            Updated = false;
        }

        public virtual void QueueRenderCommands(GameTime gameTime, Renderer renderer, IGraphicsContext context)
        {
        }

        /// <summary>
        /// Frees resources used by this component
        /// </summary>
        internal virtual void Delete()
        {
        }
    }
}
