using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.ComponentModel;
using System.Diagnostics;

namespace Clockwork.Rendering
{
    public delegate void FrameEventHandler(object sender, FrameEventArgs e);

    public abstract class Renderer
    {
        // CONSTANTS
        private const bool DEFAULT_DEPTH_TEST_STATE = true;

        private const bool DEFAULT_FACE_CULLING = true;
        private const CullFaceMode DEFAULT_CULL_FACE_MODE = CullFaceMode.Back;

        private const bool DEFAULT_MSAA_STATE = true;


        // Values
        private Stopwatch _updateStopwatch = new Stopwatch();
        private Stopwatch _renderStopwatch = new Stopwatch();

        private long _elapsedUpdateFrames;
        private long _elapsedRenderFrames;

        private long _lastUpdateTime;
        private long _lastRenderTime;

        private bool _depthTest;

        private bool _faceCulling;
        private CullFaceMode _faceCullMode;

        private bool _msaa;

        private Color4 _clearColor;


        // Properties
        //  Input
        public abstract KeyboardState Keyboard { get; }
        public abstract MouseState Mouse { get; }

        public abstract CursorState CursorState { get; set; }

        public abstract Vector2i Size { get; }

        public abstract float AspectRatio { get; }

        public abstract bool IsFocused { get; }


        // GL Properties
        public bool DepthTest
        {
            get => _depthTest;
            set
            {
                if (value != _depthTest)
                {
                    _depthTest = value;

                    if (value) GL.Enable(EnableCap.DepthTest);
                    else GL.Disable(EnableCap.DepthTest);
                }
            }
        }

        public bool FaceCulling
        {
            get => _faceCulling;
            set
            {
                if (value != _faceCulling)
                {
                    _faceCulling = value;

                    if (value) GL.Enable(EnableCap.CullFace);
                    else GL.Disable(EnableCap.CullFace);
                }
            }
        }
        public CullFaceMode FaceCullMode
        {
            get => _faceCullMode;
            set
            {
                if (value != _faceCullMode)
                {
                    _faceCullMode = value;

                    GL.CullFace(FaceCullMode);
                }
            }
        }

        public bool MSAA
        {
            get => _msaa;

            set
            {
                if (value != _msaa)
                {
                    _msaa = value;

                    if (value) GL.Enable(EnableCap.Multisample);
                    else GL.Disable(EnableCap.Multisample);
                }
            }
        }

        public Color4 ClearColor
        {
            get => _clearColor;
            set
            {
                if (value != _clearColor)
                {
                    _clearColor = value;

                    GL.ClearColor(_clearColor);
                }
            }
        }

        #region Events
        public event FrameEventHandler Update;
        public event FrameEventHandler Render;

        public event Action? Load;
        public event Action? Unload;
        public event Action? Refresh;

        public event Action<CancelEventArgs>? Closing;

        public event Action? RenderThreadStarted;

        public event Action<ResizeEventArgs>? Resized;
        public event Action<WindowPositionEventArgs>? Move;
        public event Action<MinimizedEventArgs>? Minimized;
        public event Action<MaximizedEventArgs>? Maximized;

        public event Action<FocusedChangedEventArgs>? FocusChanged;

        public event Action<TextInputEventArgs>? TextInput;

        public event Action<KeyboardKeyEventArgs>? KeyDown;
        public event Action<KeyboardKeyEventArgs>? KeyUp;

        public event Action<JoystickEventArgs>? JoystickConnected;

        public event Action? MouseEnter;
        public event Action? MouseLeave;
        public event Action<MouseMoveEventArgs>? MouseMove;
        public event Action<MouseWheelEventArgs>? MouseWheel;
        public event Action<MouseButtonEventArgs>? MouseDown;
        public event Action<MouseButtonEventArgs>? MouseUp;

        public event Action<FileDropEventArgs>? FileDrop;
        #endregion

        public Renderer()
        {
            _updateStopwatch.Start();
            _renderStopwatch.Start();
        }


        // Abstracts
        public abstract void Start();
        public abstract void Stop();


        // Virtuals
        protected virtual void UpdateFrame(OpenTK.Windowing.Common.FrameEventArgs e)
        {
            long currentTime = _updateStopwatch.ElapsedMilliseconds;
            Update?.Invoke(this, new FrameEventArgs(this, new GameTime(currentTime, _lastUpdateTime, _elapsedUpdateFrames), Keyboard, Mouse));
            _lastUpdateTime = currentTime;

            _elapsedUpdateFrames++;
        }
        protected virtual void RenderFrame(OpenTK.Windowing.Common.FrameEventArgs e)
        {
            long currentTime = _renderStopwatch.ElapsedMilliseconds;
            Render?.Invoke(this, new FrameEventArgs(this, new GameTime(currentTime, _lastRenderTime, _elapsedRenderFrames), Keyboard, Mouse));
            _lastRenderTime = currentTime;

            _elapsedRenderFrames++;
        }

        protected virtual void OnLoad()
        {
            DepthTest = true;
            FaceCulling = true;

            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Blend);

            MSAA = true;

            FaceCullMode = CullFaceMode.Back;

            GL.ClearColor(ClearColor);

            Load?.Invoke();
        }
        protected virtual void OnUnload()
        {
            Unload?.Invoke();
        }
        protected virtual void OnRefresh()
        {
            Refresh?.Invoke();
        }

        protected virtual void OnClosing(CancelEventArgs e)
        {
            Closing?.Invoke(e);
        }

        protected virtual void OnRenderThreadStarted()
        {
            RenderThreadStarted?.Invoke();
        }

        protected virtual void OnResized(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, Size.X, Size.Y);

            Resized?.Invoke(e);
        }
        protected virtual void OnMove(WindowPositionEventArgs e)
        {
            Move?.Invoke(e);
        }
        protected virtual void OnMinimized(MinimizedEventArgs e)
        {
            Minimized?.Invoke(e);
        }
        protected virtual void OnMaximized(MaximizedEventArgs e)
        {
            Maximized?.Invoke(e);
        }

        protected virtual void OnFocusChanged(FocusedChangedEventArgs e)
        {
            FocusChanged?.Invoke(e);
        }

        protected virtual void OnTextInput(TextInputEventArgs e)
        {
            TextInput?.Invoke(e);
        }

        protected virtual void OnKeyDown(KeyboardKeyEventArgs e)
        {
            KeyDown?.Invoke(e);
        }
        protected virtual void OnKeyUp(KeyboardKeyEventArgs e)
        {
            KeyUp?.Invoke(e);
        }

        protected virtual void OnJoystickConnected(JoystickEventArgs e)
        {
            JoystickConnected?.Invoke(e);
        }

        protected virtual void OnMouseEnter()
        {
            MouseEnter?.Invoke();
        }
        protected virtual void OnMouseLeave()
        {
            MouseLeave?.Invoke();
        }
        protected virtual void OnMouseMove(MouseMoveEventArgs e)
        {
            MouseMove?.Invoke(e);
        }
        protected virtual void OnMouseWheel(MouseWheelEventArgs e)
        {
            MouseWheel?.Invoke(e);
        }
        protected virtual void OnMouseDown(MouseButtonEventArgs e)
        {
            MouseDown?.Invoke(e);
        }
        protected virtual void OnMouseUp(MouseButtonEventArgs e)
        {
            MouseUp?.Invoke(e);
        }

        protected virtual void OnFileDropped(FileDropEventArgs e)
        {
            FileDrop?.Invoke(e);
        }
    }
}
