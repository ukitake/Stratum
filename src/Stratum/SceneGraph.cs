using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum
{
    public class SceneGraph : IUpdate, IRender
    {
        private class NodeVisitor
        {
            public NodeVisitor(SceneNode node)
            {
                Node = node;
                Visited = false;
            }

            public SceneNode Node { get; private set; }
            public bool Visited;
        }

        private Stack<NodeVisitor> updateStack;

        public SceneGraph()
        {
            updateStack = new Stack<NodeVisitor>();
        }

        public SceneNode Root { get; private set; }

        /// <summary>
        /// 'node' becomes a child of 'insertLocation' or the Root of the 
        /// SceneGraph if 'insertLocation' is null
        /// </summary>
        /// <param name="node"></param>
        /// <param name="insertLocation"></param>
        public void Insert(SceneNode node, SceneNode insertLocation)
        {
            if (insertLocation == null)
            {
                Root = node;
            }
            else
            {
                insertLocation.AddChild(node);
            }
        }

        /// <summary>
        /// Moves the subtree rooted at 'node' such that 'node' is a child of 'newParent'
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newParent"></param>
        public void ChangeParent(SceneNode node, SceneNode newParent)
        {
            node.ChangeParent(newParent);
        }

        public void Update(GameTime gameTime)
        {
            if (Root == null) return;

            updateStack.Push(new NodeVisitor(Root));

            while (updateStack.Count > 0)
            {
                // note the peek... we only pop after PostUpdate has been called for a node
                var visitor = updateStack.Peek();

                if (!visitor.Visited)
                {
                    visitor.Node.Update(gameTime);
                    visitor.Visited = true;
                }
                else
                {
                    // already visited, so PostUpdate and pop
                    visitor.Node.PostUpdate(gameTime);
                    updateStack.Pop();
                    continue;
                }

                var children = visitor.Node.Children;
                int size = children.Count;
                for (int i = size - 1; i >= 0; i--)
                    updateStack.Push(new NodeVisitor(children.ElementAt(i)));
            }
        }

        public void QueueRenderCommands(GameTime gameTime, Renderer renderer, Graphics.IGraphicsContext context)
        {
            Stack<SceneNode> dfs = new Stack<SceneNode>();
            dfs.Push(Root);

            while (dfs.Count > 0)
            {
                var node = dfs.Pop();

                // view frustum cull
                if (node.WorldBound.Test(context.RenderContext.CameraFrustum))
                {
                    node.QueueRenderCommands(gameTime, renderer, context);

                    foreach (var child in node.Children)
                        dfs.Push(child);
                }
            }
        }

        public void BFTraversal(SceneNode root, Action<SceneNode> visit)
        {
            Queue<SceneNode> bfs = new Queue<SceneNode>();
            bfs.Enqueue(root);

            while (bfs.Count > 0)
            {
                var obj = bfs.Dequeue();

                visit(obj);

                foreach (var child in obj.Children)
                    bfs.Enqueue(child);
            }
        }

        public void DFTraversal(SceneNode root, Action<SceneNode> visit)
        {
            Stack<SceneNode> dfs = new Stack<SceneNode>();
            dfs.Push(root);

            while (dfs.Count > 0)
            {
                var obj = dfs.Pop();

                visit(obj);

                foreach (var child in obj.Children)
                    dfs.Push(child);
            }
        }
    }
}
