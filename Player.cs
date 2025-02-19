using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Voxels
{
    public class Player
    {
        public Vector3 Position { get; private set; } = Vector3.Zero;

        private Vector3 _front = new (0.0f, 0.0f, 0.0f);
        private Vector3 _right = new (0.0f, 0.0f, 0.0f);
        private Vector3 _up = new (0.0f, 1.0f, 0.0f);

        private float _movementSpeed = 100.0f;

        private int _vboHandle = 0;
        private int _vaoHandle = 0;

        private readonly float[] _vertices =
        {
            -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f, // Front face
             0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
             0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
             0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,

            -0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f, // Back face
             0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,

            -0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f, // Left face
            -0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,

             0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f, // Right face
             0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
             0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,

            -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f, // Bottom face
             0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
             0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f, -0.5f, -0.5f,    1.0f, 0.0f, 0.0f,

            -0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f, // Top face
             0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
             0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f,  0.5f,  0.5f,    1.0f, 0.0f, 0.0f,
            -0.5f,  0.5f, -0.5f,    1.0f, 0.0f, 0.0f,
        };

        public Player()
        {
            _right = Vector3.Normalize(Vector3.Cross(_front, _up));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public void Init()
        {
            _vboHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Length * sizeof(float), _vertices, BufferUsageHint.StaticDraw);

            _vaoHandle = GL.GenVertexArray();
            GL.BindVertexArray(_vaoHandle);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Update(float deltaTime, Vector3 front, KeyboardState keyboardState)
        {
            UpdateDirectionVectors(front);

            Vector3 movement = Vector3.Zero;

            if (keyboardState.IsKeyDown(Keys.W))
            {
                movement += Vector3.Normalize(_front);
            }

            if (keyboardState.IsKeyDown(Keys.S))
            {
                movement -= Vector3.Normalize(_front);
            }

            if (keyboardState.IsKeyDown(Keys.A))
            {
                movement -= Vector3.Normalize(_right);
            }

            if (keyboardState.IsKeyDown(Keys.D))
            {
                movement += Vector3.Normalize(_right);
            }

            //if (movement.LengthFast > 0.0f)
            //{
            //    movement = Vector3.Normalize(movement);
            //}

            Position = Vector3.Add(Vector3.Multiply(movement, deltaTime * _movementSpeed), Position);
        }

        public void Render()
        {
            GL.BindVertexArray(_vaoHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);
            GL.BindVertexArray(0);
        }

        private void UpdateDirectionVectors(Vector3 front)
        {
            _front = Vector3.Normalize(new Vector3(front.X, front.Y, front.Z));
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}
