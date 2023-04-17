using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.Common;
using OpenTK.Graphics.OpenGL4;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Clockwork.Rendering
{
    public class ClockworkWindow : Renderer
    {
        // Constants
        internal const string DEFAULT_TITLE = "Clockwork Game";


        // Override properties
        public override Vector2i Size => _window.Size;

        public override float AspectRatio => _window.Size.X / (float)_window.Size.Y;

        public override KeyboardState Keyboard => _window.KeyboardState;
        public override MouseState Mouse => _window.MouseState;

        public override CursorState CursorState { get => _window.CursorState; set => _window.CursorState = value; }

        public override bool IsFocused => _window.IsFocused; 


        // Properties
        public string Title { get => _window.Title; set => _window.Title = value; }

        public WindowBorder Border { get => _window.WindowBorder; set => _window.WindowBorder = value; }
        public WindowState State { get => _window.WindowState; set => _window.WindowState = value; }
        public VSyncMode VSync { get => _window.VSync; set => _window.VSync = value; }


        // Values
        private GameWindow _window;

        private Color4 _initialClearColor;


        // Constructor
        public ClockworkWindow(string title = DEFAULT_TITLE, Vector2i? size = null, WindowState state = WindowState.Normal, WindowBorder border = WindowBorder.Fixed, Color4? clearColor = null, VSyncMode vSync = VSyncMode.On)
        {
            size = size == null ? new Vector2i(1280, 720) : size.Value;
            _initialClearColor = clearColor == null ? Color4.Black : clearColor.Value;

            _window = new GameWindow(new GameWindowSettings(), new NativeWindowSettings() { Title = title, Size = size.Value, WindowState = state, WindowBorder = border, Vsync = vSync, NumberOfSamples=16 });

            SubscribeCallbacks();
        }
        private void SubscribeCallbacks()
        {
            // Subscribe to the windows events
            _window.UpdateFrame += UpdateFrame;
            _window.RenderFrame += RenderFrame;

            _window.Load += OnLoad;
            _window.Unload += OnUnload;
            _window.Refresh += OnRefresh;

            _window.Closing += OnClosing;

            _window.RenderThreadStarted += OnRenderThreadStarted;

            _window.Resize += OnResized;
            _window.Move += OnMove;
            _window.Minimized += OnMinimized;
            _window.Maximized += OnMaximized;

            _window.FocusedChanged += OnFocusChanged;

            _window.TextInput += OnTextInput;

            _window.KeyDown += OnKeyDown;
            _window.KeyUp += OnKeyUp;

            _window.JoystickConnected += OnJoystickConnected;

            _window.MouseEnter += OnMouseEnter;
            _window.MouseLeave += OnMouseLeave;
            _window.MouseMove += OnMouseMove;
            _window.MouseWheel += OnMouseWheel;
            _window.MouseDown += OnMouseDown;
            _window.MouseUp += OnMouseUp;

            _window.FileDrop += OnFileDropped;
        }


        // Initialization func
        public override void Start()
        {
            _window.Run();
        }
        public override void Stop()
        {
            _window.Close();
        }


        // Lifetime func
        protected override void OnLoad()
        {
            base.OnLoad();

            ClearColor = _initialClearColor;
        }
        protected override void UpdateFrame(OpenTK.Windowing.Common.FrameEventArgs e)
        {
            base.UpdateFrame(e);
        }
        protected override void RenderFrame(OpenTK.Windowing.Common.FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            base.RenderFrame(e);

            _window.SwapBuffers();
        }


        // Func
        public void SetSize(Vector2i vector)
        {
            _window.Size = vector;
        }
    }
}
