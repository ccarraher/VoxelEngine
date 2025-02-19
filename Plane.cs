using OpenTK.Mathematics;

namespace Voxels
{
    public struct Plane
    {
        public Vector3 Normal { get; set; }

        public float Distance { get; set; }

        public float DistanceTo(Vector3 point)
        {
            return Vector3.Dot(Normal, point) + Distance;
        }

        public Plane(Vector4 coefficients)
        {
            Vector3 normal = new Vector3(coefficients.X, coefficients.Y, coefficients.Z);
            float magnitude = normal.Length;

            // Avoid division by zero
            if (magnitude > 0.0001f)
            {
                Normal = normal / magnitude;
                Distance = coefficients.W / magnitude;
            }
            else
            {
                Normal = Vector3.UnitZ;  // Default normal
                Distance = 0;
            }
        }
    }
}
