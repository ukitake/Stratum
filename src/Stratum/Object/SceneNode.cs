using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum
{
    public abstract class SceneNode : Spatial
    {
        private ulong id;
        protected SceneNode parent;
        protected List<SceneNode> children;
        
        internal SceneNode()
        {
            // default initial capacity is 4, which is too big in our case
            children = new List<SceneNode>(1);
        }

        internal SceneNode(SceneNode parent)
            : base()
        {
            this.parent = parent;
            children = new List<SceneNode>(1);
        }

        public event Action<SceneNode, ulong, ulong> IdChanged;

        public ulong Id
        {
            get { return id; }
            internal set
            {
                if (id != value)
                {
                    ulong old = id;
                    id = value;

                    if (IdChanged != null)
                        IdChanged(this, old, id);
                }
            }
        }

        public SceneNode Parent
        {
            get { return parent; }
        }

        /// <summary>
        /// Explicitly change this SceneNodes parent... this is broken
        /// out into a seperate method rather than allowing direct setting
        /// of the Parent property because changes of Parent should be 
        /// exceptional and have, at the very least, spatial consequences for 
        /// the entire subtree rooted at this SceneNode
        /// </summary>
        /// <param name="newParent"></param>
        public virtual void ChangeParent(SceneNode newParent)
        {
            if (newParent == null)
                throw new ArgumentNullException();

            if (parent != null)
                parent.RemoveChild(this);

            newParent.AddChild(this);

            parent = newParent;
        }

        public IReadOnlyCollection<SceneNode> Children
        {
            get { return children; }
        }

        public void AddChild(SceneNode newChild)
        {
            // O(N)
            // validate that we aren't adding a duplicate node
            if (children.Contains(newChild)) throw new InvalidOperationException("Node is already a child of this node");

            children.Add(newChild);
        }

        public void RemoveChild(SceneNode childToRemove)
        {
            if (!children.Remove(childToRemove))
                throw new InvalidOperationException("Node you are trying to remove is not a child of this node"); 
        }

        public override void Update(GameTime gameTime)
        {
            if (!WorldIsCurrent)
            {
                // compose parent's world with our local to get our world
                World = MatrixD.Multiply(Parent.World, Local);
                WorldIsCurrent = true;
            }
        }

        protected override void PostUpdate(GameTime gameTime)
        {
            if (parent != null)
            {
                parent.WorldBound = parent.WorldBound.Merge(this.WorldBound);
            }
        }

        internal virtual void Delete()
        {
        }

        public override bool Equals(object obj)
        {
            SceneNode other = obj as SceneNode;
            if (other != null)
                return id == other.id;

            return false;
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public static bool operator ==(SceneNode n1, SceneNode n2)
        {
            return n1.id == n2.id;
        }

        public static bool operator !=(SceneNode n1, SceneNode n2)
        {
            return n1.id != n2.id;
        }
    }
}
