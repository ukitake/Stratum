using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using Stratum.CompGeom;
using Stratum.Components;
using Stratum.Graphics;
using Stratum.Graphics.RenderCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Stratum.World.Earth
{
    public class PlanetComponent : RenderableComponent
    {
        protected TerrainNode[] rootNodes;
        protected Buffer<TerrainVertex> vertexBuffer;

        protected Dictionary<string, GraphicsResource> resources;

        public PlanetComponent()
        {
            Initialize();

            rootNodes = new HemisphereTerrainNode[2];
            rootNodes[0] = new HemisphereTerrainNode(null, Quadrant.BL, new GIS.LatLon(-90.0, -180.0), new GIS.LatLon(90.0, 0.0));
            rootNodes[1] = new HemisphereTerrainNode(null, Quadrant.BL, new GIS.LatLon(-90.0, 0.0), new GIS.LatLon(90.0, 180.0));

            resources = new Dictionary<string, GraphicsResource>();
        }

        private Effect wireFrame;

        public int NodeCount { get; private set; }

        public virtual void Initialize()
        {
            GraphicsDevice device = Engine.GraphicsContext.Device;

            wireFrame = EffectLoader.Load(@"World/Earth/Shaders/DeferredTerrain.fx");

            vertexBuffer = Buffer<TerrainVertex>.New<TerrainVertex>(device, SharpDX.Utilities.SizeOf<TerrainVertex>() * 4 * 1000, BufferFlags.VertexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
        }

        public override void Update(GameTime gameTime)
        {
            IGraphicsContext context = Engine.GraphicsContext;

            for (int i = 0; i < rootNodes.Length; i++)
            {
                rootNodes[i].Update(context.CurrentCamera);
            }

            base.Update(gameTime);

            this.Object.WorldBound = new AxisAlignedBoundingBox(rootNodes[0].AABB);
        }

        protected override IRenderCommand GenerateRenderCommand()
        {
            return new RenderDeferredCommand<TerrainVertex>(
                wireFrame,
                PrimitiveType.PatchList(4),
                vertexBuffer,
                0,
                resources,
                this.Object.World.ToMatrix(),
                Engine.GraphicsContext.Device.BlendStates.Opaque, rasterizer: Engine.GraphicsContext.Device.RasterizerStates.CullBack);
        }

        private Stratum.Graphics.DirectionalLight dlight = new Graphics.DirectionalLight(new Vector3(0, 0, -1), Color.White);

        public override void QueueRenderCommands(GameTime gameTime, Renderer renderer, IGraphicsContext context)
        {
            List<TerrainNode> nodesToRender = new List<TerrainNode>();

            renderer.EnqueueLight(dlight);

            // todo: maybe can parallelize
            for (int i = 0; i < rootNodes.Length; i++)
            {
                if (rootNodes[i] != null)
                {
                    List<TerrainNode> nodesFromThisCubeFace = new List<TerrainNode>();
                    findNodesToRender(nodesFromThisCubeFace, rootNodes[i], context.CurrentCamera);
                    nodesToRender.AddRange(nodesFromThisCubeFace);
                }
            }

            int nodeCount = nodesToRender.Count;
            NodeCount = nodeCount;
            if (nodeCount > 0)
            {
                TerrainVertex[] vertices = nodesToRender.SelectMany(node => node.Geometry).ToArray();

                var command = GenerateRenderCommand();
                (command as RenderDeferredCommand<TerrainVertex>).NumVertices = vertices.Length;
                vertexBuffer.SetData(vertices);

                renderer.EnqueueDeferred(command as IRenderDeferredCommand);
            }
        }

        private void findNodesToRender(List<TerrainNode> toRender, TerrainNode root, Camera camera)
        {
            if (root.IsVisible(camera))
            {
                if (root.IsSplit)
                {
                    foreach (var child in root.Children)
                        findNodesToRender(toRender, child, camera);
                }
                else
                {
                    //root.LastRendered = DateTime.Now;
                    toRender.Add(root);
                }
            }
        }
    }
}
