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
        protected Buffer<int> indexBuffer;

        protected Dictionary<string, GraphicsResource> resources;

        public PlanetComponent()
        {
            Initialize();

            rootNodes = new CesiumTerrainNode[2];
            rootNodes[0] = new CesiumTerrainNode(null, Quadrant.BL, new GIS.LatLon(-90.0, -180.0), new GIS.LatLon(90.0, 0.0));
            rootNodes[1] = new CesiumTerrainNode(null, Quadrant.BL, new GIS.LatLon(-90.0, 0.0), new GIS.LatLon(90.0, 180.0));

            resources = new Dictionary<string, GraphicsResource>();
        }

        private Effect wireFrame;

        public virtual void Initialize()
        {
            GraphicsDevice device = Engine.GraphicsContext.Device;

            wireFrame = EffectLoader.Load(@"Graphics/Shaders/Wireframe.fx");

            int[] indices = new int[5000 * 12];

            for (int i = 0; i < 5000; i++)
            {
                fillIndices(i, indices);
            }

            vertexBuffer = Buffer<TerrainVertex>.New<TerrainVertex>(device, SharpDX.Utilities.SizeOf<TerrainVertex>() * 4 * 5000, BufferFlags.VertexBuffer, SharpDX.Direct3D11.ResourceUsage.Dynamic);
            indexBuffer = Buffer<int>.New(device, indices, BufferFlags.IndexBuffer);
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
            return new RenderCommand<TerrainVertex>(
                wireFrame,
                PrimitiveType.PatchList(4),
                vertexBuffer,
                0,
                resources,
                this.Object.World.ToMatrix(),
                Engine.GraphicsContext.Device.BlendStates.Opaque);
        }

        public override void QueueRenderCommands(GameTime gameTime, Renderer renderer, IGraphicsContext context)
        {
            List<TerrainNode> nodesToRender = new List<TerrainNode>();

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
            if (nodeCount > 0)
            {
                TerrainVertex[] vertices = nodesToRender.SelectMany(node => nodeToGeometry(node)).ToArray();

                var command = GenerateRenderCommand();
                (command as RenderCommand<TerrainVertex>).NumVertices = vertices.Length;
                vertexBuffer.SetData(vertices);

                renderer.EnqueueNormal(command);

                //context.Device.SetRasterizerState(context.Device.RasterizerStates.CullBack);
                //context.Device.SetDepthStencilState(context.Device.DepthStencilStates.Default);
                //context.Device.SetBlendState(context.Device.BlendStates.Opaque);

                //context.GBuffer.SetShaderParameters(this.Object);
                //context.GBuffer.SetCameraParameters(context.CurrentCamera);
                //wireFrame.Parameters["World"].SetValue(this.Object.World.ToMatrix());
                //wireFrame.Parameters["View"].SetValue(context.CurrentCamera.View);
                //wireFrame.Parameters["Proj"].SetValue(context.CurrentCamera.Proj);
                //Engine.Renderer.RenderGeometry(PrimitiveType.PatchList(4), vertexBuffer,  nodeCount * 4, wireFrame);
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

        private TerrainVertex[] nodeToGeometry(TerrainNode node)
        {
            return new TerrainVertex[4] 
            {
                new TerrainVertex(new Vector3((float)MathUtilD.DegreesToRadians(node.TL.Longitude), (float)MathUtilD.DegreesToRadians(node.TL.Latitude), 0f), new Vector2(0f, 0f)),
                new TerrainVertex(new Vector3((float)MathUtilD.DegreesToRadians(node.TR.Longitude), (float)MathUtilD.DegreesToRadians(node.TR.Latitude), 0f), new Vector2(1f, 0f)),
                new TerrainVertex(new Vector3((float)MathUtilD.DegreesToRadians(node.BR.Longitude), (float)MathUtilD.DegreesToRadians(node.BR.Latitude), 0f), new Vector2(1f, 1f)),
                new TerrainVertex(new Vector3((float)MathUtilD.DegreesToRadians(node.BL.Longitude), (float)MathUtilD.DegreesToRadians(node.BL.Latitude), 0f), new Vector2(0f, 1f))
            };
        }

        private int[] getIndices(int nodeIndex)
        {
            var indices = new int[12]
            {
                1,2,4,  2,3,4,  3,0,4,  0,1,4
            };

            for (int i = 0; i < 12; i++)
            {
                indices[i] += nodeIndex * 5;
            }

            return indices;
        }

        private void fillIndices(int nodeIndex, int[] indices)
        {
            var ind = getIndices(nodeIndex);
            int startIndex = nodeIndex * 12;

            for (int i = 0; i < 12; i++)
            {
                indices[startIndex + i] = ind[i];
            }
        }
    }
}
