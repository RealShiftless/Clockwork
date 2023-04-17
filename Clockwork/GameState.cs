using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clockwork.Common.GameObjects;
using Clockwork.Rendering;
using Clockwork.ResourcesDeprecated;
using OpenTK.Graphics.OpenGL4;

namespace Clockwork
{
    public abstract class GameState
    {
        // Values
        public Game? Game { get; internal set; }

        private GameObjectManager _entityManager;

        private Camera _camera;


        // Properties
        public bool IsBound => Game != null;

        public Camera Camera => _camera;

        public ResourceManager Resources
        {
            get
            {
                if (Game == null)
                    throw new InvalidOperationException("Could not get ContentManager, as the game state was not bound to a game!");

                return Game.Resources;
            }
        }


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
            _camera = new Camera(this);
            _entityManager = new GameObjectManager(this);
        }
        public virtual void Dispose()
        {
        }
    }
}
