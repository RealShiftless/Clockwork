using Clockwork.Mathematics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Common.Input;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using static OpenTK.Windowing.GraphicsLibraryFramework.GLFWCallbacks;
using Box2 = Clockwork.Mathematics.Box2;
using Vector2 = Clockwork.Mathematics.Vector2;

namespace Clockwork.Windowing
{
    // Delegates
    public delegate void WindowEventHandler(Window sender);

    public delegate void WindowMovedEventHandler(Window sender, WindowPositionEventArgs e);
    public delegate void WindowResizedEventHandler(Window sender, WindowResizedEventArgs e);
    public delegate void WindowClosingEventHandler(Window sender, CancelEventArgs e);
    public delegate void WindowMinimizedEventHandler(Window sender, WindowMinimizedEventArgs e);


    // class
    public class Window : IDisposable
    {
        // Static values
        private static ConcurrentQueue<ExceptionDispatchInfo> _callbackExceptions = new ConcurrentQueue<ExceptionDispatchInfo>();


        // Values
        public bool IsEventDriven;

        internal unsafe OpenTK.Windowing.GraphicsLibraryFramework.Window* WindowPtr;

        private unsafe Cursor* _glfwCursor;
        private MouseCursor _managedCursor = MouseCursor.Default;

        private ContextAPI _api;
        private ContextProfile _profile;
        private ContextFlags _flags;
        private Version _apiVersion;
        private IGLFWGraphicsContext _context;

        private MonitorHandle _currentMonitor;

        private WindowIcon _windowIcon;
        private string _title;

        private WindowState _windowState;
        private WindowBorder _windowBorder;

        private Vector2 _position;
        private Vector2 _size;

        private Vector2 _clientSize;

        private Vector2? _minSize;
        private Vector2? _maxSize;

        private (int numerator, int denominator)? _aspectRatio;

        private bool _isFocused;
        private bool _isVisible;

        private bool _exists;

        private Vector2 _cachedPosition;
        private Vector2 _cachedSize;

        private Vector2 _lastMousePos;

        private VSyncMode _vsyncMode;


        // Properties
        public ContextAPI API => _api;
        public ContextProfile Profile => _profile;
        public ContextFlags Flags => _flags;
        public Version APIVersion => _apiVersion;
        public IGLFWGraphicsContext Context => _context;

        public MonitorHandle CurrentMonitor
        {
            get => _currentMonitor;
            set
            {
                unsafe
                {
                    OpenTK.Windowing.GraphicsLibraryFramework.Monitor* monitor = value.ToUnsafePtr<OpenTK.Windowing.GraphicsLibraryFramework.Monitor>();
                    var mode = GLFW.GetVideoMode(monitor);
                    GLFW.SetWindowMonitor(
                        WindowPtr,
                        monitor,
                        (int)_position.X,
                        (int)_position.Y,
                        (int)_size.X,
                        (int)_size.Y,
                        mode->RefreshRate);

                    _currentMonitor = value;
                }
            }
        }

        public WindowIcon WindowIcon
        {
            get => _windowIcon;
            set
            {
                unsafe
                {
                    var images = value.Images;
                    Span<GCHandle> handles = stackalloc GCHandle[images.Length];
                    Span<OpenTK.Windowing.GraphicsLibraryFramework.Image> glfwImages =
                        stackalloc OpenTK.Windowing.GraphicsLibraryFramework.Image[images.Length];

                    for (var i = 0; i < images.Length; i++)
                    {
                        var image = images[i];
                        handles[i] = GCHandle.Alloc(image.Data, GCHandleType.Pinned);
                        var addrOfPinnedObject = (byte*)handles[i].AddrOfPinnedObject();
                        glfwImages[i] =
                            new OpenTK.Windowing.GraphicsLibraryFramework.Image(image.Width, image.Height, addrOfPinnedObject);
                    }

                    GLFW.SetWindowIcon(WindowPtr, glfwImages);

                    foreach (var handle in handles)
                    {
                        handle.Free();
                    }
                }

                _windowIcon = value;
            }
        }
        public string Title
        {
            get => _title;
            set
            {
                unsafe
                {
                    GLFW.SetWindowTitle(WindowPtr, value);

                    _title = value;
                }
            }
        }

        public MouseCursor Cursor
        {
            get => _managedCursor;
            set
            {
                _managedCursor = value ??
                                 throw new ArgumentNullException(
                                 nameof(value),
                                 "Cursor cannot be null. To reset to default cursor, set it to MouseCursor.Default instead.");

                unsafe
                {
                    var oldCursor = _glfwCursor;
                    _glfwCursor = null;

                    // Create the new GLFW cursor
                    if (value.Shape == MouseCursor.StandardShape.CustomShape)
                    {
                        // User provided mouse cursor.
                        fixed (byte* ptr = value.Data)
                        {
                            var cursorImg = new OpenTK.Windowing.GraphicsLibraryFramework.Image(value.Width, value.Height, ptr);
                            _glfwCursor = GLFW.CreateCursor(cursorImg, value.X, value.Y);
                        }
                    }

                    // If this is the default cursor, we don't need to run CreateStandardCursor.
                    // GLFW will reset the window to default if we assign null as cursor.
                    else if (value != MouseCursor.Default)
                    {
                        // Standard mouse cursor.
                        _glfwCursor = GLFW.CreateStandardCursor(MapStandardCursorShape(value.Shape));
                    }

                    GLFW.SetCursor(WindowPtr, _glfwCursor);

                    if (oldCursor != null)
                    {
                        // Make sure to destroy the old cursor AFTER assigning the new one.
                        // Otherwise the user might briefly see their OS cursor during the reassignment.
                        GLFW.DestroyCursor(oldCursor);
                    }
                }
            }
        }
        public unsafe CursorState CursorState
        {
            get
            {
                CursorModeValue inputMode = GLFW.GetInputMode(WindowPtr, CursorStateAttribute.Cursor);

                switch (inputMode)
                {
                    case CursorModeValue.CursorNormal:
                        return CursorState.Normal;
                    case CursorModeValue.CursorHidden:
                        return CursorState.Hidden;
                    case CursorModeValue.CursorDisabled:
                        return CursorState.Grabbed;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set
            {
                CursorModeValue inputMode;
                switch (value)
                {
                    case CursorState.Normal:
                        inputMode = CursorModeValue.CursorNormal;
                        break;
                    case CursorState.Hidden:
                        inputMode = CursorModeValue.CursorHidden;
                        break;
                    case CursorState.Grabbed:
                        inputMode = CursorModeValue.CursorDisabled;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                GLFW.SetInputMode(WindowPtr, CursorStateAttribute.Cursor, inputMode);
            }
        }

        public WindowState WindowState
        {
            get => _windowState;
            set
            {
                unsafe
                {
                    if (_windowState == WindowState.Fullscreen && value != WindowState.Fullscreen)
                    {
                        // We are going from fullscreen to something else.
                        GLFW.SetWindowMonitor(WindowPtr, null, (int)_cachedPosition.X, (int)_cachedPosition.Y, (int)_cachedSize.X, (int)_cachedSize.Y, 0);
                    }

                    switch (value)
                    {
                        case WindowState.Normal:
                            GLFW.RestoreWindow(WindowPtr);
                            break;

                        case WindowState.Minimized:
                            GLFW.IconifyWindow(WindowPtr);
                            break;

                        case WindowState.Maximized:
                            GLFW.MaximizeWindow(WindowPtr);
                            break;

                        case WindowState.Fullscreen:
                            // Cache the window size so we can reset to it when we go out of fullscreen.
                            _cachedSize = ClientSize;
                            _cachedPosition = Position;
                            var monitor = CurrentMonitor.ToUnsafePtr<OpenTK.Windowing.GraphicsLibraryFramework.Monitor>();
                            var modePtr = GLFW.GetVideoMode(monitor);
                            GLFW.SetWindowMonitor(WindowPtr, monitor, 0, 0, modePtr->Width, modePtr->Height, modePtr->RefreshRate);
                            break;
                    }

                    _windowState = value;
                }
            }
        }
        public WindowBorder WindowBorder
        {
            get => _windowBorder;
            set
            {
                unsafe
                {
                    GLFW.GetVersion(out var major, out var minor, out _);

                    // It isn't possible to implement this in versions of GLFW older than 3.3,
                    // as SetWindowAttrib didn't exist before then.
                    if ((major == 3) && (minor < 3))
                    {
                        throw new NotSupportedException("Cannot be implemented in GLFW 3.2.");
                    }

                    switch (value)
                    {
                        case WindowBorder.Hidden:
                            GLFW.SetWindowAttrib(WindowPtr, WindowAttribute.Decorated, false);
                            break;
                        case WindowBorder.Resizable:
                            GLFW.SetWindowAttrib(WindowPtr, WindowAttribute.Decorated, true);
                            GLFW.SetWindowAttrib(WindowPtr, WindowAttribute.Resizable, true);
                            break;
                        case WindowBorder.Fixed:
                            GLFW.SetWindowAttrib(WindowPtr, WindowAttribute.Decorated, true);
                            GLFW.SetWindowAttrib(WindowPtr, WindowAttribute.Resizable, false);
                            break;
                    }

                    _windowBorder = value;
                }
            }
        }

        public bool IsFullscreen => WindowState == WindowState.Fullscreen;

        public Vector2 Position
        {
            get => _position;
            set
            {
                unsafe
                {
                    GLFW.SetWindowPos(WindowPtr, (int)value.X, (int)value.Y);
                    Position = value;
                }
            }
        }
        public Vector2 Size
        {
            get => _size;
            set
            {
                unsafe
                {
                    GLFW.SetWindowSize(WindowPtr, (int)value.X, (int)value.Y);
                    _size = value;
                }
            }
        }
        public Vector2 ClientSize => _clientSize;

        public Vector2? MinSize
        {
            get => _minSize;
            set
            {
                unsafe
                {
                    GLFW.SetWindowSizeLimits(WindowPtr, (int)(value?.X ?? GLFW.DontCare), (int)(value?.Y ?? GLFW.DontCare), (int)(_maxSize?.X ?? GLFW.DontCare), (int)(_maxSize?.Y ?? GLFW.DontCare));
                    _minSize = value;
                }
            }
        }
        public Vector2? MaxSize
        {
            get => _maxSize;
            set
            {
                unsafe
                {
                    GLFW.SetWindowSizeLimits(WindowPtr, (int)(_minSize?.X ?? GLFW.DontCare), (int)(_minSize?.Y ?? GLFW.DontCare), (int)(value?.X ?? GLFW.DontCare), (int)(value?.Y ?? GLFW.DontCare));
                    _maxSize = value;
                }
            }
        }

        public Box2 ClientRectangle
        {
            get => new Box2(Position, Position + Size);
            set
            {
                Position = value.Min;
                Size = value.Size;
            }
        }

        public (int numerator, int denominator)? AspectRatio
        {
            get => _aspectRatio;
            set
            {
                unsafe
                {
                    GLFW.SetWindowAspectRatio(WindowPtr, value?.numerator ?? GLFW.DontCare, value?.denominator ?? GLFW.DontCare);
                }
            }
        }

        public bool IsFocused => _isFocused;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                unsafe
                {
                    if(value !=  _isVisible)
                    {
                        _isVisible = value;

                        if (value)
                            GLFW.ShowWindow(WindowPtr);
                        else
                            GLFW.HideWindow(WindowPtr);
                    }
                }
            }
        }

        public bool Exists => _exists;
        public unsafe bool IsExiting => GLFW.WindowShouldClose(WindowPtr);

        public Vector2 MousePosition
        {
            get => _lastMousePos;
            set
            {
                unsafe
                {
                    GLFW.SetCursorPos(WindowPtr, value.X, value.Y);
                }
            }
        }

        public unsafe string ClipboardString
        {
            get
            {
                return GLFW.GetClipboardString(WindowPtr);
            }

            set
            {
                GLFW.SetClipboardString(WindowPtr, value);
            }
        }

        public VSyncMode VSyncMode
        {
            get
            {
                if (Context == null)
                    throw new InvalidOperationException("Cannot control vsync when running with ContextAPI.NoAPI.");

                return _vsyncMode;
            }
            set
            {
                if (Context == null)
                    throw new InvalidOperationException("Cannot control vsync when running with ContextAPI.NoAPI.");

                switch (value)
                {
                    case VSyncMode.Disabled:
                        Context.SwapInterval = 0;
                        break;

                    case VSyncMode.Enabled:
                        Context.SwapInterval = 1;
                        break;
                }
            }
        }


        // Glfw Callbacks
        private WindowPosCallback _windowPosCallback;
        private WindowSizeCallback _windowSizeCallback;
        private WindowIconifyCallback _windowIconifyCallback;
        private WindowMaximizeCallback _windowMaximizeCallback;
        private WindowFocusCallback _windowFocusCallback;
        private WindowRefreshCallback _windowRefreshCallback;
        private WindowCloseCallback _windowCloseCallback;
        private KeyCallback _keyCallback;
        private CharCallback _charCallback;
        private MouseButtonCallback _mouseButtonCallback;
        private CursorPosCallback _cursorPosCallback;
        private CursorEnterCallback _cursorEnterCallback;
        private ScrollCallback _scrollCallback;
        private JoystickCallback _joystickCallback;
        private DropCallback _dropCallback;


        // Events
        public event WindowMovedEventHandler Moved;
        public event WindowResizedEventHandler Resized;
        public event Action<MinimizedEventArgs> Minimized;
        public event Action<MaximizedEventArgs> Maximized;
        public event Action<FocusedChangedEventArgs> FocusedChanged;
        public event WindowEventHandler Refreshed;
        public event WindowClosingEventHandler Closing;
        public event WindowEventHandler Closed;
        public event Action<KeyboardKeyEventArgs> KeyUp;
        public event Action<KeyboardKeyEventArgs> KeyDown;
        public event Action<TextInputEventArgs> TextInput;
        public event Action<MouseButtonEventArgs> MouseDown;
        public event Action<MouseButtonEventArgs> MouseUp;
        public event Action<MouseMoveEventArgs> MouseMove;
        public event Action MouseLeave;
        public event Action MouseEnter;
        public event Action<MouseWheelEventArgs> MouseWheel;
        public event Action<JoystickEventArgs> JoystickConnected;
        public event Action<FileDropEventArgs> FileDrop;


        // Constructor
        public unsafe Window(WindowSettings settings)
        {
            GLFWProvider.EnsureInitialized();

            // Set the windows title up
            _title = settings.Title;

            // Set the hint for the border style
            switch (settings.WindowBorder)
            {
                case WindowBorder.Hidden:
                    GLFW.WindowHint(WindowHintBool.Decorated, false);
                    break;

                case WindowBorder.Resizable:
                    GLFW.WindowHint(WindowHintBool.Resizable, true);
                    break;

                case WindowBorder.Fixed:
                    GLFW.WindowHint(WindowHintBool.Resizable, false);
                    break;
            }

            // Set the api hint
            var isOpenGl = false;
            _api = settings.API;
            switch (settings.API)
            {
                case ContextAPI.NoAPI:
                    GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.NoApi);
                    break;

                case ContextAPI.OpenGLES:
                    GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlEsApi);
                    isOpenGl = true;
                    break;

                case ContextAPI.OpenGL:
                    GLFW.WindowHint(WindowHintClientApi.ClientApi, ClientApi.OpenGlApi);
                    isOpenGl = true;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            // Set the api version
            GLFW.WindowHint(WindowHintInt.ContextVersionMajor, settings.APIVersion.Major);
            GLFW.WindowHint(WindowHintInt.ContextVersionMinor, settings.APIVersion.Minor);

            _apiVersion = settings.APIVersion;

            // Set the flags
            _flags = settings.Flags;
            if (settings.Flags.HasFlag(ContextFlags.ForwardCompatible))
            {
                GLFW.WindowHint(WindowHintBool.OpenGLForwardCompat, true);
            }

            if (settings.Flags.HasFlag(ContextFlags.Debug))
            {
                GLFW.WindowHint(WindowHintBool.OpenGLDebugContext, true);
            }

            // Set the profile
            _profile = settings.Profile;
            switch (settings.Profile)
            {
                case ContextProfile.Any:
                    GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Any);
                    break;
                case ContextProfile.Compatability:
                    GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Compat);
                    break;
                case ContextProfile.Core:
                    GLFW.WindowHint(WindowHintOpenGlProfile.OpenGlProfile, OpenGlProfile.Core);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            // General settings
            GLFW.WindowHint(WindowHintBool.Focused, settings.StartFocused);
            _windowBorder = settings.WindowBorder;

            _isVisible = settings.StartVisible;
            GLFW.WindowHint(WindowHintBool.Visible, _isVisible);

            GLFW.WindowHint(WindowHintInt.Samples, settings.NumberOfSamples);

            GLFW.WindowHint(WindowHintBool.SrgbCapable, settings.SrgbCapable);

            if (settings.TransparentFramebuffer is bool transparent)
            {
                GLFW.WindowHint(WindowHintBool.TransparentFramebuffer, transparent);
            }

            // We do the work to set the hint bits outside of the CreateWindow conditional
            // so that the window will get the correct fullscreen red/green/blue bits stored
            // in its hidden fields regardless of how it gets created.  (The extra curly
            // braces here keep the local `monitor` definition from conflicting with the
            // _monitorCallback lambda below.)
            {
                var monitor = settings.CurrentMonitor.ToUnsafePtr<OpenTK.Windowing.GraphicsLibraryFramework.Monitor>();
                var modePtr = GLFW.GetVideoMode(monitor);
                GLFW.WindowHint(WindowHintInt.RedBits, settings.RedBits ?? modePtr->RedBits);
                GLFW.WindowHint(WindowHintInt.GreenBits, settings.GreenBits ?? modePtr->GreenBits);
                GLFW.WindowHint(WindowHintInt.BlueBits, settings.BlueBits ?? modePtr->BlueBits);
                if (settings.AlphaBits.HasValue)
                {
                    GLFW.WindowHint(WindowHintInt.AlphaBits, settings.AlphaBits.Value);
                }

                if (settings.DepthBits.HasValue)
                {
                    GLFW.WindowHint(WindowHintInt.DepthBits, settings.DepthBits.Value);
                }

                if (settings.StencilBits.HasValue)
                {
                    GLFW.WindowHint(WindowHintInt.StencilBits, settings.StencilBits.Value);
                }

                GLFW.WindowHint(WindowHintInt.RefreshRate, modePtr->RefreshRate);

                if (settings.WindowState == WindowState.Fullscreen && _isVisible)
                {
                    _windowState = WindowState.Fullscreen;
                    _cachedPosition = settings.Position ?? new Vector2(32, 32);  // Better than nothing.
                    _cachedSize = settings.Size;
                    WindowPtr = GLFW.CreateWindow(modePtr->Width, modePtr->Height, _title, monitor, (OpenTK.Windowing.GraphicsLibraryFramework.Window*)(settings.SharedContext?.WindowPtr ?? IntPtr.Zero));
                }
                else
                {
                    WindowPtr = GLFW.CreateWindow((int)settings.Size.X, (int)settings.Size.Y, _title, null, (OpenTK.Windowing.GraphicsLibraryFramework.Window*)(settings.SharedContext?.WindowPtr ?? IntPtr.Zero));
                }
            }

            // For Vulkan, we need to pass ContextAPI.NoAPI, otherwise we will get an exception.
            // See https://github.com/glfw/glfw/blob/56a4cb0a3a2c7a44a2fd8ab3335adf915e19d30c/src/vulkan.c#L320
            //
            // But Calling MakeCurrent while using NoApi, we will get an exception from GLFW,
            // because Vulkan does not have that concept.
            // See https://github.com/glfw/glfw/blob/fd79b02840a36b74e4289cc53dc332de6403b8fd/src/context.c#L618
            if (settings.API != ContextAPI.NoAPI)
            {
                _context = new GLFWGraphicsContext(WindowPtr);
            }

            _exists = true;

            if (isOpenGl)
            {
                Context?.MakeCurrent();

                if (settings.AutoLoadBindings)
                {
                    InitializeGlBindings();
                }
            }

            // Enables the caps lock modifier to be detected and updated
            GLFW.SetInputMode(WindowPtr, LockKeyModAttribute.LockKeyMods, true);

            RegisterWindowCallbacks();

            InitialiseJoystickStates();

            _isFocused = settings.StartFocused;
            if (settings.StartFocused)
            {
                Focus();
            }

            // Setting WindowState to e.g. Normal while the
            // window is hidden will show the window
            // So if we don't set WindowState when StartVisible is false.
            if (settings.StartVisible)
            {
                WindowState = settings.WindowState;
            }

            IsEventDriven = settings.IsEventDriven;

            if (settings.Icon != null)
            {
                _windowIcon = settings.Icon;
            }

            if (settings.Position.HasValue)
            {
                Position = settings.Position.Value;
            }

            GLFW.GetWindowSize(WindowPtr, out var width, out var height);

            HandleResize(width, height);

            AspectRatio = settings.AspectRatio;
            _minSize = settings.MinimumSize;
            _maxSize = settings.MaximumSize;

            GLFW.SetWindowSizeLimits(WindowPtr, (int)(_minSize?.X ?? GLFW.DontCare), (int)(_minSize?.Y ?? GLFW.DontCare), (int)(_maxSize?.X ?? GLFW.DontCare), (int)(_maxSize?.Y ?? GLFW.DontCare));

            GLFW.GetWindowPos(WindowPtr, out int x, out int y);
            _position = new Vector2(x, y);

            GLFW.GetCursorPos(WindowPtr, out double mousex, out double mousey);
            _lastMousePos = new Vector2((float)mousex, (float)mousey);
            //MouseState.Position = _lastReportedMousePos;

            _isFocused = GLFW.GetWindowAttrib(WindowPtr, WindowAttributeGetBool.Focused);

            // We can't set Vsync if we are using ContextAPI.NoAPI.
            if (API != ContextAPI.NoAPI)
            {
                VSyncMode = settings.VSyncMode;
            }
        }


        // Static Func
        private static void InitializeGlBindings()
        {
            // We don't put a hard dependency on OpenTK.Graphics here.
            // So we need to use reflection to initialize the GL bindings, so users don't have to.

            // Try to load OpenTK.Graphics assembly.
            Assembly assembly;
            try
            {
                assembly = Assembly.Load("OpenTK.Graphics");
            }
            catch
            {
                // Failed to load graphics, oh well.
                // Up to the user I guess?
                // TODO: Should we expose this load failure to the user better?
                return;
            }

            var provider = new GLFWBindingsContext();

            void LoadBindings(string typeNamespace)
            {
                var type = assembly.GetType($"OpenTK.Graphics.{typeNamespace}.GL");
                if (type == null)
                {
                    return;
                }

                MethodInfo? load = type.GetMethod("LoadBindings");
                load?.Invoke(null, new object[] { provider });
            }

            LoadBindings("ES11");
            LoadBindings("ES20");
            LoadBindings("ES30");
            LoadBindings("OpenGL");
            LoadBindings("OpenGL4");
        }


        // Func
        public unsafe void Focus()
        {
            GLFW.FocusWindow(WindowPtr);
        }

        private unsafe void RegisterWindowCallbacks()
        {
            // These must be assigned to fields even when they're methods

            _windowPosCallback = WindowPosCallback;
            _windowSizeCallback = WindowSizeCallback;
            _windowIconifyCallback = WindowIconifyCallback;
            _windowMaximizeCallback = WindowMaximizeCallback;
            _windowFocusCallback = WindowFocusCallback;
            _windowRefreshCallback = WindowRefreshCallback;
            _windowCloseCallback = WindowCloseCallback;
            _keyCallback = KeyCallback;
            _charCallback = CharCallback;
            _mouseButtonCallback = MouseButtonCallback;
            _cursorPosCallback = CursorPosCallback;
            _cursorEnterCallback = CursorEnterCallback;
            _scrollCallback = ScrollCallback;
            _joystickCallback = JoystickCallback;
            _dropCallback = DropCallback;

            // FIXME: Add FramebufferSizeCallback and WindowContentsScaleCallback
            // FIXME: CharModsCallback

            GLFW.SetWindowPosCallback(WindowPtr, _windowPosCallback);
            GLFW.SetWindowSizeCallback(WindowPtr, _windowSizeCallback);
            GLFW.SetWindowIconifyCallback(WindowPtr, _windowIconifyCallback);
            GLFW.SetWindowMaximizeCallback(WindowPtr, _windowMaximizeCallback);
            GLFW.SetWindowFocusCallback(WindowPtr, _windowFocusCallback);
            GLFW.SetWindowRefreshCallback(WindowPtr, _windowRefreshCallback);
            GLFW.SetWindowCloseCallback(WindowPtr, _windowCloseCallback);
            GLFW.SetMouseButtonCallback(WindowPtr, _mouseButtonCallback);
            GLFW.SetKeyCallback(WindowPtr, _keyCallback);
            GLFW.SetCharCallback(WindowPtr, _charCallback);

            GLFW.SetCursorPosCallback(WindowPtr, _cursorPosCallback);
            GLFW.SetCursorEnterCallback(WindowPtr, _cursorEnterCallback);
            GLFW.SetScrollCallback(WindowPtr, _scrollCallback);


            GLFW.SetDropCallback(WindowPtr, _dropCallback);

            Joysticks.JoystickCallback += _joystickCallback;
        }

        private unsafe void HandleResize(int width, int height)
        {
            _size.X = width;
            _size.Y = height;

            GLFW.GetFramebufferSize(WindowPtr, out width, out height);

            _clientSize = new Vector2(width, height);
        }

        private unsafe WindowState GetWindowStateFromGLFW()
        {
            if (GLFW.GetWindowAttrib(WindowPtr, WindowAttributeGetBool.Iconified))
            {
                return WindowState.Minimized;
            }

            if (GLFW.GetWindowAttrib(WindowPtr, WindowAttributeGetBool.Maximized))
            {
                return WindowState.Maximized;
            }

            if (GLFW.GetWindowMonitor(WindowPtr) != null)
            {
                return WindowState.Fullscreen;
            }

            return WindowState.Normal;
        }


        // Callback Func
        private unsafe void WindowPosCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, int x, int y)
        {
            try
            {
                OnMove(new WindowPositionEventArgs((int)Position.X, (int)Position.Y, x, y));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void WindowSizeCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, int width, int height)
        {
            try
            {
                OnResize(new WindowResizedEventArgs((int)Size.X, (int)Size.Y, width, height));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void WindowCloseCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window)
        {
            try
            {
                var c = new CancelEventArgs();
                OnClosing(c);
                if (c.Cancel)
                {
                    GLFW.SetWindowShouldClose(WindowPtr, false);
                }
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void WindowRefreshCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window)
        {
            try
            {
                OnRefresh();
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void WindowFocusCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, bool focused)
        {
            try
            {
                OnFocusedChanged(new FocusedChangedEventArgs(focused));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void WindowIconifyCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, bool iconified)
        {
            try
            {
                OnMinimized(new MinimizedEventArgs(iconified));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void WindowMaximizeCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, bool maximized)
        {
            try
            {
                OnMaximized(new MaximizedEventArgs(maximized));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void MouseButtonCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, MouseButton button, InputAction action, KeyModifiers mods)
        {
            try
            {
                var args = new MouseButtonEventArgs(button, action, mods);

                if (action == InputAction.Release)
                {
                    MouseState[button] = false;
                    OnMouseUp(args);
                }
                else
                {
                    MouseState[button] = true;
                    OnMouseDown(args);
                }
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void CursorPosCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, double posX, double posY)
        {
            try
            {
                var newPos = new Vector2((float)posX, (float)posY);
                var delta = newPos - _lastReportedMousePos;

                _lastReportedMousePos = newPos;

                OnMouseMove(new MouseMoveEventArgs(newPos, delta));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void CursorEnterCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, bool entered)
        {
            try
            {
                if (entered)
                {
                    OnMouseEnter();
                }
                else
                {
                    OnMouseLeave();
                }
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void ScrollCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, double offsetX, double offsetY)
        {
            try
            {
                // GLFW says this function can be called not only in response to functions like glfwPollEvents();
                // There might be a function like glfwSetWindowSize what will trigger a scroll event to trigger inside that function.
                // We ignore this case for now and just accept that the scroll value will change after such a function call.
                var offset = new Vector2((float)offsetX, (float)offsetY);

                MouseState.Scroll += offset;

                OnMouseWheel(new MouseWheelEventArgs(offset));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void KeyCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, Keys key, int scancode, InputAction action, KeyModifiers mods)
        {
            try
            {
                var args = new KeyboardKeyEventArgs(key, scancode, mods, action == InputAction.Repeat);

                if (action == InputAction.Release)
                {
                    if (key != Keys.Unknown)
                    {
                        KeyboardState.SetKeyState(key, false);
                    }

                    OnKeyUp(args);
                }
                else
                {
                    if (key != Keys.Unknown)
                    {
                        KeyboardState.SetKeyState(key, true);
                    }

                    OnKeyDown(args);
                }
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void CharCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, uint codepoint)
        {
            try
            {
                OnTextInput(new TextInputEventArgs((int)codepoint));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void DropCallback(OpenTK.Windowing.GraphicsLibraryFramework.Window* window, int count, byte** paths)
        {
            try
            {
                var arrayOfPaths = new string[count];

                for (var i = 0; i < count; i++)
                {
                    arrayOfPaths[i] = MarshalUtility.PtrToStringUTF8(paths[i]);
                }

                OnFileDrop(new FileDropEventArgs(arrayOfPaths));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        private unsafe void JoystickCallback(int joy, ConnectedState eventCode)
        {
            try
            {
                if (eventCode == ConnectedState.Connected)
                {
                    // Initialize the first joystick state.
                    GLFW.GetJoystickHatsRaw(joy, out var hatCount);
                    GLFW.GetJoystickAxesRaw(joy, out var axisCount);
                    GLFW.GetJoystickButtonsRaw(joy, out var buttonCount);
                    var name = GLFW.GetJoystickName(joy);

                    _joystickStates[joy] = new JoystickState(hatCount, axisCount, buttonCount, joy, name);
                }
                else
                {
                    // Remove the joystick state from the array of joysticks.
                    _joystickStates[joy] = null;
                }

                OnJoystickConnected(new JoystickEventArgs(joy, eventCode == ConnectedState.Connected));
            }
            catch (Exception e)
            {
                _callbackExceptions.Enqueue(ExceptionDispatchInfo.Capture(e));
            }
        }

        public void MakeCurrent()
        {
            if (Context == null)
                throw new InvalidOperationException("Cannot make a context current when running with ContextAPI.NoAPI.");

            Context.MakeCurrent();
        }

        public bool ProcessEvents(double timeout)
        {
            GLFW.WaitEventsTimeout(timeout);

            ProcessInputEvents();

            RethrowCallbackExceptionsIfNeeded();

            // FIXME: Remove this return and the documentation comment about it
            return true;
        }


        // Virtual
        public virtual unsafe void Close()
        {
            // We don't have to catch exceptions here as this code isn't called directly from unmanaged code
            CancelEventArgs c = new CancelEventArgs();
            OnClosing(c);
            if (c.Cancel == false)
            {
                GLFW.SetWindowShouldClose(WindowPtr, true);
            }
        }

        protected virtual void OnMove(WindowPositionEventArgs e)
        {
            Moved?.Invoke(this, e);

            _position.X = e.X;
            _position.Y = e.Y;
        }
        protected virtual void OnResize(WindowResizedEventArgs e)
        {
            HandleResize(e.Width, e.Height);

            Resized?.Invoke(this, e);
        }
        protected virtual void OnRefresh()
        {
            Refreshed?.Invoke(this);
        }


        // Interface
        public void Dispose()
        {
        }
    }
}
