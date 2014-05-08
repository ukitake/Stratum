using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using Stratum.Graphics;
using Stratum.Input;
using Stratum.Content;
using SharpDX.Toolkit.Graphics;
using System.Collections.Concurrent;
using Stratum.World.Earth;
using Stratum.GIS;

namespace Stratum
{
    public class Engine
    {
        private static Engine instance;
        public static Engine Instance
        {
            get
            {
                if (instance == null)
                    instance = new Engine();

                return instance;
            }
        }

        private bool isInitialized = false;
        private static IdPool idPool;
        private static IInputContext inputContext;
        private static IGraphicsContext graphicsContext;
        private static IContentManager contentManager;
        private static GraphicsDeviceService graphicsDeviceService;

        private static SceneGraph sceneGraph;
        private static Renderer renderer;

        private static ConcurrentDictionary<ulong, GameObject> objects = new ConcurrentDictionary<ulong,GameObject>();
        private static ConcurrentDictionary<ulong, Component> components = new ConcurrentDictionary<ulong,Component>();

        private Engine()
        {
        }

        public static IdPool IdPool { get { return idPool; } }
        public static IInputContext InputContext { get { return inputContext; } }
        public static IGraphicsContext GraphicsContext { get { return graphicsContext; } }
        public static IContentManager ContentManager { get { return contentManager; } }
        public static GraphicsDeviceService GraphicsDeviceService { get { return graphicsDeviceService; } }

        public static SceneGraph SceneGraph { get { return sceneGraph; } }
        public static Renderer Renderer { get { return renderer; } }

        public static IEnumerable<GameObject> Objects { get { return objects.Values; } }
        public static IEnumerable<Component> Components { get { return components.Values; } }

        #region GameObject and Component

        public static GameObject CreateGameObject()
        {
            ulong id = IdPool.Get();
            GameObject @object = new GameObject();
            @object.Id = id;

            // add object to Engine
            objects[id] = @object;

            return @object;
        }

        public static T CreateComponent<T>() where T : Component
        {
            ulong id = IdPool.Get();
            T component = Activator.CreateInstance<T>();
            component.Id = id;

            // add component to Engine
            components[id] = component;

            return component;
        }

        public static GameObject GetGameObject(ulong id)
        {
            if (objects.ContainsKey(id))
                return objects[id];

            return null;
        }

        public static Component GetComponent(ulong id)
        {
            if (components.ContainsKey(id))
                return components[id];

            return null;
        }

        public static T GetComponent<T>(ulong id) where T : Component
        {
            if (components.ContainsKey(id))
                return (T)components[id];

            return null;
        }

        internal static void Delete(SceneNode sceneNode)
        {
            IdPool.Free(sceneNode.Id);

            sceneNode.Delete();

            GameObject o;
            objects.TryRemove(sceneNode.Id, out o);

            foreach (var child in sceneNode.Children)
                Delete(child);
        }

        public static void Delete(GameObject @object)
        {
            Delete((SceneNode)@object);
        }

        internal static void Delete(Component component)
        {
            IdPool.Free(component.Id);

            component.Delete();

            Component o;
            components.TryRemove(component.Id, out o);
        }

        #endregion

        public void Initialize()
        {
            if (isInitialized)
                return;

            idPool = new Stratum.IdPool();
            sceneGraph = new Stratum.SceneGraph();

            // init SharpDX
            BootstrapSharpDX();

            renderer = new Stratum.Renderer();

            var earth = CreateGameObject();
            earth.AddComponent(new PlanetComponent());
            SceneGraph.Insert(earth, null);

            Engine.GraphicsContext.CurrentCamera = new PlanetCamera(RenderWGS84.EarthRadius);

            //worldEngine = new World();
            //Engine.Instance.AddService(worldEngine, typeof(IWorldEngine), typeof(World));

            isInitialized = true;
        }

        public void Initialize(System.Windows.Forms.Form form)
        {
            if (isInitialized)
                return;

            idPool = new Stratum.IdPool();
            sceneGraph = new Stratum.SceneGraph();

            BootstrapSharpDX(form);

            renderer = new Stratum.Renderer();

            var earth = CreateGameObject();
            earth.AddComponent(new PlanetComponent());
            SceneGraph.Insert(earth, null);

            Engine.GraphicsContext.CurrentCamera = new PlanetCamera(RenderWGS84.EarthRadius);

            //worldEngine = new World();
            //Engine.Instance.AddService(worldEngine, typeof(IWorldEngine), typeof(World));

            isInitialized = true;
        }

        //private World worldEngine;

        public void Start()
        {
            GameLoop engineLoop = new GameLoop();
            engineLoop.IsFixedTimeStep = false;
            renderer.StartFramerateCounter();
            engineLoop.Start();
        }

        public void Update(GameTime gameTime)
        {
            inputContext.Update(gameTime);
            sceneGraph.Update(gameTime);

            // update the render context to have current matrices
            graphicsContext.RenderContext.Update(gameTime);

            //worldEngine.Update(gameTime);
        }

        public void Render(GameTime gameTime)
        {
            renderer.Render(gameTime, graphicsContext);
            //worldEngine.Render(gameTime, Resolve<IGraphicsContext>());
        }

        private void BootstrapSharpDX()
        {
            var registry = new GameServiceRegistry();

            var graphicsDeviceService = new GraphicsDeviceService();
            registry.AddService(typeof(IGraphicsDeviceService), graphicsDeviceService);
            Engine.graphicsDeviceService = graphicsDeviceService;

            GraphicsContext graphicsContext = new GraphicsContext(graphicsDeviceService, graphicsDeviceService.Device, graphicsDeviceService.DeviceContext);
            Engine.graphicsContext = graphicsContext;

            ContentManager contentManager = new ContentManager(registry);
            Engine.contentManager = contentManager;

            InputContext inputContext = new InputContext();
            Engine.inputContext = inputContext;

            graphicsContext.GBuffer.Initialize();
        }

        private void BootstrapSharpDX(System.Windows.Forms.Form form)
        {
            var registry = new GameServiceRegistry();

            var graphicsDeviceService = new GraphicsDeviceService(form);
            registry.AddService(typeof(IGraphicsDeviceService), graphicsDeviceService);
            Engine.graphicsDeviceService = graphicsDeviceService;

            GraphicsContext graphicsContext = new GraphicsContext(graphicsDeviceService, graphicsDeviceService.Device, graphicsDeviceService.DeviceContext);
            Engine.graphicsContext = graphicsContext;

            ContentManager contentManager = new ContentManager(registry);
            Engine.contentManager = contentManager;

            InputContext inputContext = new InputContext(form);
            Engine.inputContext = inputContext;

            graphicsContext.GBuffer.Initialize();
        }
    }
}
