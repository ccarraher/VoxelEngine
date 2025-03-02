using OpenTK.Mathematics;

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

        private Plane[] FrustumPlanes = new Plane[6];

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
            UpdateFrustum();
        }

        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(_position, _position + Front, _up);
        }

        public Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(_fov), _width / _height, 0.1f, 1000f);
        }

        public bool BoundingBoxInFrustum(BoundingBox box)
        {
            foreach (var plane in FrustumPlanes)
            {
                Vector3 absNormal = new Vector3(MathF.Abs(plane.Normal.X), MathF.Abs(plane.Normal.Y), MathF.Abs(plane.Normal.Z));

                float distance = plane.DistanceTo(box.Center);
                float projection = box.Extents.X * absNormal.X + box.Extents.Y * absNormal.Y + box.Extents.Z * absNormal.Z;

                if (distance + projection < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public void UpdateFollowRadius(float offset)
        {
            _followRadius = MathHelper.Clamp(_followRadius - offset, 10, 50);
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

        private void UpdateFrustum()
        {
            Matrix4 vp = GetViewMatrix() * GetProjectionMatrix();

            // Left plane
            FrustumPlanes[1] = new Plane(new Vector4(
                vp.M14 + vp.M11, vp.M24 + vp.M21, vp.M34 + vp.M31, vp.M44 + vp.M41));

            // Right plane
            FrustumPlanes[0] = new Plane(new Vector4(
                vp.M14 - vp.M11, vp.M24 - vp.M21, vp.M34 - vp.M31, vp.M44 - vp.M41));

            // Bottom plane
            FrustumPlanes[2] = new Plane(new Vector4(
                vp.M14 + vp.M12, vp.M24 + vp.M22, vp.M34 + vp.M32, vp.M44 + vp.M42));

            // Top plane
            FrustumPlanes[3] = new Plane(new Vector4(
                vp.M14 - vp.M12, vp.M24 - vp.M22, vp.M34 - vp.M32, vp.M44 - vp.M42));

            // Near plane
            FrustumPlanes[4] = new Plane(new Vector4(
                vp.M13, vp.M23, vp.M33, vp.M43));

            // Far plane
            FrustumPlanes[5] = new Plane(new Vector4(
                vp.M14 - vp.M13, vp.M24 - vp.M23, vp.M34 - vp.M33, vp.M44 - vp.M43));
        }
    }
}
