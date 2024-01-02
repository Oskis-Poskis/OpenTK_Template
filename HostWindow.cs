using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

using System.Runtime.InteropServices;

namespace WindowTemplate
{
    public class HostWindow : GameWindow
    {
        unsafe public HostWindow(NativeWindowSettings settings, bool UseWindowState) : base(GameWindowSettings.Default, settings)
        {
            CenterWindow();
            
            if (UseWindowState)
            {
                state.LoadState(WindowPtr);
                window_size = new(state.properties.width, state.properties.height);
            }

            else window_size = settings.Size;
            mouse_state = MouseState;
            keyboard_state = KeyboardState;
        }

        WindowSaveState state = new WindowSaveState(new WindowProperties());

        public static Vector2 mouse_pos;
        public static MouseState mouse_state;
        public static KeyboardState keyboard_state;

        public static Vector2i window_size = Vector2i.One;
        public static float window_aspect = 1.0f;
        public static string base_directory = AppDomain.CurrentDomain.BaseDirectory;

        private static void OnDebugMessage(
            DebugSource source,
            DebugType type,
            int id,
            DebugSeverity severity,
            int length,
            IntPtr pMessage,
            IntPtr pUserParam)
        {
            string message = Marshal.PtrToStringAnsi(pMessage, length);
            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
            if (type == DebugType.DebugTypeError)
            {
                throw new Exception(message);
            }
        }

        private static readonly DebugProc DebugMessageDelegate = OnDebugMessage;
        unsafe protected override void OnLoad()
        {
            base.OnLoad();

            MakeCurrent();
            GL.Enable(EnableCap.DebugOutput);
            GL.Enable(EnableCap.DepthTest);
            GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            
            IsVisible = true;
        }

        unsafe protected override void OnUnload()
        {
            base.OnUnload();
            
            state.SaveState(WindowPtr);
        }

        unsafe protected override void OnUpdateFrame(FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            base.OnRenderFrame(args);

            mouse_pos = MousePosition;
            mouse_state = MouseState;
        }

        unsafe protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
            state.Resize(e.Width, e.Height);
            window_size = new(e.Width, e.Height);
            window_aspect = (float)window_size.X / window_size.Y;
        }

        bool is_fullscreen = false;
        
        unsafe protected override void OnMaximized(MaximizedEventArgs e)
        {
            base.OnMaximized(e);

            GLFW.GetWindowSize(WindowPtr, out int width, out int height);
            GL.Viewport(0, 0, width, height);
            state.Resize(width, height);

            is_fullscreen = true;

            window_size = new(width, height);
            window_aspect = (float)window_size.X / window_size.Y;
        }

        unsafe protected override void OnKeyDown(KeyboardKeyEventArgs e)
        {
            base.OnKeyDown(e);

            keyboard_state = KeyboardState;

            if (e.Key == Keys.F11)
            {
                if (!is_fullscreen)
                {
                    GLFW.MaximizeWindow(WindowPtr);
                    is_fullscreen = true;
                }
                else
                {
                    GLFW.RestoreWindow(WindowPtr);
                    is_fullscreen = false;
                }
            }
        }
    }
}
