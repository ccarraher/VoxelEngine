using OpenTK.Mathematics;
using System;

namespace Voxels
{
    public class Camera
    {
        private float _mouseSensitivity = 0.075f;
        private float _yaw = -90.0f;
        private float _pitch = 0.0f;
        private float _followRadius = 10.0f;
        public float AspectRatio { get; set; }
        private float _fov = 90.0f;
        private Vector2 _lastMousePosition = new (0.0f, 0.0f);
        private Vector3 _position = new (0.0f, 0.0f, 0.0f);
        public Vector3 Front { get; private set; } = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        private readonly int _width;
        private readonly int _height;

        public Camera(Vector3 position, int width, int height)
        {
            _position = position;
            _width = width;
            _height = height;
        }

        public void Update(float deltaTime, Vector3 playerPosition, Vector2 mousePosition)
        {
            ProcessMouseMovement(mousePosition);
            UpdateCameraVectors(playerPosition);
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(_position, _position + Front, _up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), _width / _height, 0.1f, 1000f);
        }

        private void ProcessMouseMovement(Vector2 mousePosition)
        {
            float xOffset = mousePosition.X - _lastMousePosition.X;
            float yOffset = _lastMousePosition.Y - mousePosition.Y;
            _lastMousePosition = mousePosition;

            xOffset *= _mouseSensitivity;
            yOffset *= _mouseSensitivity;

            _yaw += xOffset;
            _pitch -= yOffset;

            if (_pitch > 89.0f)
            {
                _pitch = 89.0f;
            }
            else if (_pitch < -89.0f)
            {
                _pitch = -89.0f;
            }
        }

        private void UpdateCameraVectors(Vector3 playerPosition)
        {
            float offsetX = _followRadius * MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Sin(MathHelper.DegreesToRadians(_yaw));
            float offsetY = _followRadius * MathF.Sin(MathHelper.DegreesToRadians(_pitch));
            float offsetZ = _followRadius * MathF.Cos(MathHelper.DegreesToRadians(_pitch)) * MathF.Cos(MathHelper.DegreesToRadians(_yaw));

            _position.X = playerPosition.X - offsetX;
            _position.Y = playerPosition.Y + offsetY;
            _position.Z = playerPosition.Z - offsetZ;

            Front = Vector3.Normalize(playerPosition - _position);

            _right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));  // normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            _up = Vector3.Normalize(Vector3.Cross(_right, Front));
        }
    }
}
