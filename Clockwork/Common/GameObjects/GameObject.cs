using Clockwork.Mathematics;
using Clockwork.Rendering;
using System.Diagnostics;

namespace Clockwork.Common.GameObjects
{
    public delegate void GameObjectActivityChangingEventHandler(GameObject sender, GameObjectActivityChangingEventArgs e);
    public delegate void GameObjectActivityChangedEventHandler(GameObject sender);

    public delegate void GameObjectComponentBoundEventHandler(GameObject sender, GameObjectComponentBoundEventArgs e);

    public sealed class GameObject
    {
        // Reference values
        private GameObjectManager _parentManager;


        // Values
        public Transform Transform;

        private GameObjectPtr _ptr;

        private BaseBehavior _behavior;

        private bool _enabled = false;
        private bool? _requestedEnabled = null;


        // Properties
        public Game? Game => ParentState.Game;
        public GameState ParentState => _parentManager.ParentState;

        public int ID => _ptr.ID;
        public string Name => _ptr.Name;

        public BaseBehavior Behavior => _behavior;

        /// <summary>
        /// Gets or sets the enabled state of the entity, when an entity is set to disabled, it will change state after the entity invokes LateUpdate.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                if (_enabled != value)
                {
                    _requestedEnabled = value;
                }
            }
        }

        public bool RequestingStateChange => _requestedEnabled != null;
        public bool? RequestedEnabled => _requestedEnabled;


        // Events
        public event GameObjectActivityChangingEventHandler ActivityChanging;
        public event GameObjectActivityChangedEventHandler ActivityChanged;

        public event GameObjectComponentBoundEventHandler ComponentBound;

        public event FrameEventHandler Update;
        public event FrameEventHandler LateUpdate;

        public event FrameEventHandler Render;


        // Constructor
        internal GameObject(GameObjectManager entityManager, GameObjectPtr ptr, BaseBehavior behavior)
        {
            _parentManager = entityManager;
            _ptr = ptr;
            _behavior = behavior;

            Transform = new Transform(this);
        }


        // Func
        internal void OnUpdate(object sender, FrameEventArgs e)
        {
            Update?.Invoke(this, e);
        }
        internal void OnLateUpdate(object sender, FrameEventArgs e)
        {
            LateUpdate?.Invoke(this, e);
        }
        internal void OnRender(object sender, FrameEventArgs e)
        {
            Render?.Invoke(this, e);
        }

        internal void UpdateState()
        {
            SetEnabled(_requestedEnabled.Value);
            _requestedEnabled = null;
        }
        internal void SetEnabled(bool enabled)
        {
            // Generates a args for the activity changing event
            GameObjectActivityChangingEventArgs args = new GameObjectActivityChangingEventArgs(enabled);
            ActivityChanging?.Invoke(this, args);

            // Checks if the activity has already been handled
            if (!args.Handled)
            {
                _enabled = enabled;

                ActivityChanged?.Invoke(this);
            }
        }

        public T BindComponent<T>(T component) where T : BaseComponent
        {
            if (component.IsBound)
                throw new InvalidOperationException("Component of type " + typeof(T).FullName + " was already bound to object of name " + component.GameObject.Name + "!");

            component.Bind(this);

            ComponentBound?.Invoke(this, new GameObjectComponentBoundEventArgs(component));

            return component;
        }
    }
}
