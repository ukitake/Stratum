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
        }

        internal GameObject(SceneNode parent)
            : base(parent)
        {
            components = new List<Component>();
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Component component in components)
                if (!component.Updated)
                    component.Update(gameTime);

            // Updates World if necessary
            base.Update(gameTime);

            foreach (GameObject @object in Children)
                @object.Update(gameTime);

            PostUpdate(gameTime);
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            foreach (Component component in components)
                component.PostUpdate(gameTime);

            // Updates WorldBound if necessary
            base.PostUpdate(gameTime);
        }

        internal override void Delete()
        {
            foreach (Component component in components)
                Engine.Delete(component);
        }
    }
}
