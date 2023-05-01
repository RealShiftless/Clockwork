using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clockwork.Common;
using Clockwork.Common.GameObjects;
using Clockwork.Common.Resources;
using Clockwork.Rendering;
using OpenTK.Graphics.OpenGL4;

namespace Clockwork
{
    public abstract class GameState
    {
        // Values
        public Game? Game { get; internal set; }

        private GameObjectManager _entityManager;

        private Camera _camera;

        private ResourceLibrary _resources;


        // Properties
        public bool IsBound => Game != null;

        public Camera Camera => _camera;

        public ResourceLibrary Resources => _resources;


        // Events
        public event FrameEventHandler Update;
        public event FrameEventHandler LateUpdate;

        public event FrameEventHandler Render;


        // Lifetime Func
        internal void OnUpdate(object sender, FrameEventArgs e)
        {
            Update?.Invoke(this, e);
            LateUpdate?.Invoke(this, e);
        }
        internal void OnRender(object sender, FrameEventArgs e)
        {
            Render?.Invoke(this, e);
        }


        // Func
        public GameObject InstantiateGameObject<T>(string name) where T : BaseBehavior, new()
        {
            return _entityManager.InstantiateGameObject<T>(name);
        }


        // Virtual Func
        public virtual void Initialize()
        {
            _resources = new ResourceLibrary(Game.ResourceManager);

            _camera = new Camera(this);
            _entityManager = new GameObjectManager(this);
        }
        public virtual void Dispose()
        {
        }
    }
}
