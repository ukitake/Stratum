using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum
{
    public class GameObject : SceneNode
    {
        private List<Component> components;

        internal GameObject()
            : base(null)
        {
            components = new List<Component>();
        }

        internal GameObject(SceneNode parent)
            : base(parent)
        {
            components = new List<Component>();
        }

        public void AddComponent(Component component)
        {
            components.Add(component);
            component.Object = this;
        }

        public void RemoveComponent(Component component)
        {
            components.Remove(component);
            component.Object = null;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Component component in components)
                if (!component.Updated)
                    component.Update(gameTime);

            // Updates World if necessary
            base.Update(gameTime);

            // Explicitly do not Update children... SceneGraph ensures 
            // a depth-first Update scheme with PostUpdate called on the 
            // way back up
        }

        public override void PostUpdate(GameTime gameTime)
        {
            foreach (Component component in components)
                component.PostUpdate(gameTime);

            // Updates WorldBound if necessary
            base.PostUpdate(gameTime);
        }

        internal override void Delete()
        {
            Parent.RemoveChild(this);

            foreach (Component component in components)
                Engine.Delete(component);
        }

        public override void QueueRenderCommands(GameTime gameTime, Renderer renderer, Graphics.IGraphicsContext context)
        {
            foreach (var component in components)
                component.QueueRenderCommands(gameTime, renderer, context);
        }
    }
}
