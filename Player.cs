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

        private float _movementSpeed = 15.0f;

        private BlockMesh _mesh = new BlockMesh();

        private static readonly ChunkVertex[] _vertices = new ChunkVertex[36]
        {
            new(new (-0.5f, -0.5f, -0.5f), 0, 2, 3.0f),
            new(new (0.5f, -0.5f, -0.5f), 0, 2, 3.0f),
            new(new (0.5f, 0.5f, -0.5f), 0, 2, 3.0f),
            new(new (0.5f, 0.5f, -0.5f), 0, 2, 3.0f),
            new(new (-0.5f, 0.5f, -0.5f), 0, 2, 3.0f),
            new(new (-0.5f, -0.5f, -0.5f), 0, 2, 3.0f),

            new(new (-0.5f, -0.5f, 0.5f), 1, 2, 3.0f),
            new(new (0.5f, -0.5f, 0.5f), 1, 2, 3.0f),
            new(new (0.5f, 0.5f, 0.5f), 1, 2, 3.0f),
            new(new (0.5f, 0.5f, 0.5f), 1, 2, 3.0f),
            new(new (-0.5f, 0.5f, 0.5f), 1, 2, 3.0f),
            new(new (-0.5f, -0.5f, 0.5f), 1, 2, 3.0f),

            new(new (-0.5f, 0.5f, 0.5f), 2, 2, 3.0f),
            new(new (-0.5f, 0.5f, -0.5f), 2, 2, 3.0f),
            new(new (-0.5f, -0.5f, -0.5f), 2, 2, 3.0f),
            new(new (-0.5f, -0.5f, -0.5f), 2, 2, 3.0f),
            new(new (-0.5f, -0.5f, 0.5f), 2, 2, 3.0f),
            new(new (-0.5f, 0.5f, 0.5f), 2, 2, 3.0f),

            new(new (0.5f, 0.5f, 0.5f), 3, 2, 3.0f),
            new(new (0.5f, 0.5f, -0.5f), 3, 2, 3.0f),
            new(new (0.5f, -0.5f, -0.5f), 3, 2, 3.0f),
            new(new (0.5f, -0.5f, -0.5f), 3, 2, 3.0f),
            new(new (0.5f, -0.5f, 0.5f), 3, 2, 3.0f),
            new(new (0.5f, 0.5f, 0.5f), 3, 2, 3.0f),

            new(new (-0.5f, -0.5f, -0.5f), 4, 2, 3.0f),
            new(new (0.5f, -0.5f, -0.5f), 4, 2, 3.0f),
            new(new (0.5f, -0.5f, 0.5f), 4, 2, 3.0f),
            new(new (0.5f, -0.5f, 0.5f), 4, 2, 3.0f),
            new(new (-0.5f, -0.5f, 0.5f), 4, 2, 3.0f),
            new(new (-0.5f, -0.5f, -0.5f), 4, 2, 3.0f),

            new(new (-0.5f, 0.5f, -0.5f), 5, 2, 3.0f),
            new(new (0.5f, 0.5f, -0.5f), 5, 2, 3.0f),
            new(new (0.5f, 0.5f, 0.5f), 5, 2, 3.0f),
            new(new (0.5f, 0.5f, 0.5f), 5, 2, 3.0f),
            new(new (-0.5f, 0.5f, 0.5f), 5, 2, 3.0f),
            new(new (-0.5f, 0.5f, -0.5f), 5, 2, 3.0f),
        };

        public Player()
        {
            _right = Vector3.Normalize(Vector3.Cross(_front, _up));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }

        public void Init()
        {
            _mesh.Init(_vertices);
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

            if (movement.Length > 0)
            {
                movement.Normalize();
            }

            Position = Vector3.Add(Vector3.Multiply(movement, deltaTime * _movementSpeed), Position);
        }

        public void Render()
        {
            _mesh.Render(_vertices.Length);
        }

        private void UpdateDirectionVectors(Vector3 front)
        {
            _front = Vector3.Normalize(new Vector3(front.X, front.Y, front.Z));
            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));
        }
    }
}
