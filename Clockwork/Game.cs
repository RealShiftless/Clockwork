using Clockwork.Rendering;
using Clockwork.ResourcesDeprecated;
using OpenTK.Windowing.Desktop;
using System.Reflection;

namespace Clockwork
{
    public delegate void GameStateChangedEventHandler(Game sender, GameStateChangedEventArgs e);

    public class Game
    {
        // Values
        /// <summary>
        /// When left as null the game will initialize the Renderer as a standard ClockworkWindow.
        /// Use Game.SetRenderer() before initialization to set a custom renderer.
        /// </summary>
        public Renderer Renderer { get; private set; }

        public ResourceManager Resources { get; private set; }

        public GameState State { get; private set; }

        internal ResourceManager EngineResources { get; private set; }

        private GameState? _nextState;

        private bool _isInitialized = false;
        private bool _isStarted = false;


        // Resources
        public Texture2D MissingTexture { get; private set; }

        public Material DefaultMaterial { get; private set; }
        public Shader DefaultShader { get; private set; }

        public FontPackage DefaultFonts { get; private set; }


        // Events
        public event Action Initialized;
        public event Action Starting;

        public event Action Stopped;

        public event GameStateChangedEventHandler StateChanged;

        public event FrameEventHandler Update;
        public event FrameEventHandler Render;


        // Initialization Func
        public void Initialize()
        {
            // Error handling
            if (_nextState == null)
                throw new InvalidOperationException("No state was set before Initialize was called!");

            // Initialize the renderer if nothing was set
            if (Renderer == null)
                Renderer = new ClockworkWindow();

            // Initialize the Content Manager
            Assembly? assembly = Assembly.GetEntryAssembly();

            if (assembly == null)
                throw new Exception("Error while initializing Clockwork, could not get entry assembly!");

            Resources = new ResourceManager(this, assembly, Environment.CurrentDirectory);
            EngineResources = new ResourceManager(this, Assembly.GetExecutingAssembly(), Environment.CurrentDirectory);

            // Load the engines resources
            LoadEngineResources();

            // Initialize the initial state
            State = _nextState;
            _nextState = null;

            State.Game = this;
            State.Initialize();

            // Hook into the update loop to update the current state
            Renderer.Update += OnUpdate;
            Renderer.Render += OnRender;

            // Set the flag that the game is initialized
            _isInitialized = true;

            // Invoke events 
            Initialized?.Invoke();
        }
        public void Start()
        {
            // Handle exceptions
            if (!_isInitialized)
                throw new Exception("Start was called before game was initialized!");

            if (_isStarted)
                throw new Exception("Game was already started!");

            // Invoke events
            Starting?.Invoke();

            // Start renderer
            Renderer.Start();

            // Invoke stopped after renderer retunrs
            Stopped?.Invoke();

            State.Dispose();
        }
        private void LoadEngineResources()
        {
            MissingTexture = EngineResources.Get<Texture2D>("textures.missing.png");
            MissingTexture.TextureMinFilter = OpenTK.Graphics.OpenGL4.TextureMinFilter.Nearest;
            MissingTexture.TextureMagFilter = OpenTK.Graphics.OpenGL4.TextureMagFilter.Nearest;

            DefaultMaterial = EngineResources.Get<Material>("materials.default_material.cwmat");

            DefaultFonts = EngineResources.Get<FontPackage>("fonts.clockwork_fonts.cwtfp");

            DefaultShader = EngineResources.Get<Shader>("shaders.default_shader.cwshd");
        }


        // Func
        internal void OnUpdate(object sender, FrameEventArgs e)
        {
            // Update the current state
            State.OnUpdate(this, e);

            // Invoke events
            Update?.Invoke(this, e);

            // Check if the state should be swapped
            if (_nextState != null)
            {
                // Dispose the current state
                State.Dispose();
                State.Game = null;

                // Store the current state for later use
                GameState oldState = State;

                // Set and initialize the current state
                State = _nextState;
                State.Game = this;
                State.Initialize();

                // Reset the next state variable so next frame the game wont try to swap states again
                _nextState = null;

                // Invoke events
                StateChanged?.Invoke(this, new GameStateChangedEventArgs(oldState, State, e));
            }
        }
        internal void OnRender(object sender, FrameEventArgs e)
        {
            State.OnRender(this, e);

            Render?.Invoke(this, e);
        }
        public void Stop()
        {
            Renderer.Stop();
        }


        // Setters
        public void SetRenderer(Renderer renderer)
        {
            // Handle exceptions
            if (_isInitialized)
                throw new InvalidOperationException("Game was already initialized when trying to set renderer!");

            Renderer = renderer;
        }
        public void SetState(GameState state)
        {
            if (state.Game != null)
            {
                if (state.Game != this)
                    throw new InvalidOperationException("State of type " + state.GetType().FullName + " was already bound to another game, of type " + state.Game.GetType().FullName + "!");
            }
            else
            {
                _nextState = state;
            }
        }
    }
}