using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using System.Runtime.InteropServices;

namespace Voxels
{
    public class Game : GameWindow
    {
        private readonly Player _player;
        private readonly Camera _camera;
        private readonly World _world;
        private Shader _shader;

        private readonly int _width;
        private readonly int _height;

        private int _frameCounter = 0;

        private static DebugProc DebugMessageDelegate = OnDebugMessage;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, int width, int height)
            : base(gameWindowSettings, nativeWindowSettings)
        {
            _width = width;
            _height = height;
            CenterWindow(new Vector2i(width, height));

            _player = new Player();
            _camera = new Camera(new Vector3(0.0f, 0.0f, 0.0f), _width, _height);
            _world = new World();
        }

        protected override void OnLoad()
        {
            base.OnLoad();


            _player.Init();
            _world.Init();

            //GL.Enable(EnableCap.DepthTest);
            //GL.DebugMessageCallback(DebugMessageDelegate, IntPtr.Zero);
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.ClearColor(new Color4(0.2f, 0.3f, 0.3f, 1.0f));
            GL.Enable(EnableCap.DepthTest);
            CursorState = CursorState.Grabbed;
            _shader = new Shader(Path.Combine(Environment.CurrentDirectory, "resources\\shaders\\fragment_shader.glsl"), Path.Combine(Environment.CurrentDirectory, "resources\\shaders\\vertex_shader.glsl"));
            _shader.Use();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _player.Update((float)e.Time, _camera.Front, KeyboardState);
            _camera.Update((float)e.Time, _player.Position, MousePosition);
            _world.Update(_player.Position);

            _frameCounter++;
            if (_frameCounter == 100)
            {
                Console.WriteLine($"{UpdateTime.ToString()}s");
                _frameCounter = 0;
            }
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _shader.Use();

            Matrix4.CreateTranslation(_player.Position.X, _player.Position.Y, _player.Position.Z, out var model);
            Matrix4 view = _camera.GetViewMatrix();
            Matrix4 projection = _camera.GetProjectionMatrix();

            _shader.SetMatrix4("view", view);
            _shader.SetMatrix4("projection", projection);
            _shader.SetMatrix4("model", model);

            _player.Render();

            Matrix4.CreateTranslation(0.0f, -ChunkData.Height / 2, 0.0f, out var worldModel);
            _shader.SetMatrix4("model", worldModel);

            _world.Render();

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            _shader.Dispose();
            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, Size.X, Size.Y);

            //_camera.AspectRatio = Size.X / (float)Size.Y;
        }

        //protected override void OnFramebufferResize(FramebufferResizeEventArgs e)
        //{
        //    base.OnFramebufferResize(e);

        //    GL.Viewport(0, 0, FramebufferSize.X, FramebufferSize.Y);
        //}

        private static void OnDebugMessage(
            DebugSource source,
            DebugType type,
            int id,
            DebugSeverity severity,
            int length,
            IntPtr pMessage,
            IntPtr pUserParam
        )
        {
            string message = Marshal.PtrToStringAnsi(pMessage, length);

            Console.WriteLine("[{0} source={1} type={2} id={3}] {4}", severity, source, type, id, message);
        }
    }
}
