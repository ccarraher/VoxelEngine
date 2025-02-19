using OpenTK.Mathematics;

namespace Voxels
{
    public struct BoundingBox
    {
        public Vector3 Center { get; set; }
        public Vector3 Extents { get; set; }
        public Vector3[] Points { get; set; } = new Vector3[8];

        public BoundingBox(Vector3 center, Vector3 extents)
        {
            Center = center;
            Extents = extents;

            // Calculate the 8 corner points of the AABB
            // Points are ordered as follows:
            // 0: min.x, min.y, min.z
            // 1: max.x, min.y, min.z
            // 2: min.x, max.y, min.z
            // 3: max.x, max.y, min.z
            // 4: min.x, min.y, max.z
            // 5: max.x, min.y, max.z
            // 6: min.x, max.y, max.z
            // 7: max.x, max.y, max.z

            Points[0] = new Vector3(center.X - extents.X, center.Y - extents.Y, center.Z - extents.Z); // bottom-left-back
            Points[1] = new Vector3(center.X + extents.X, center.Y - extents.Y, center.Z - extents.Z); // bottom-right-back
            Points[2] = new Vector3(center.X - extents.X, center.Y + extents.Y, center.Z - extents.Z); // top-left-back
            Points[3] = new Vector3(center.X + extents.X, center.Y + extents.Y, center.Z - extents.Z); // top-right-back
            Points[4] = new Vector3(center.X - extents.X, center.Y - extents.Y, center.Z + extents.Z); // bottom-left-front
            Points[5] = new Vector3(center.X + extents.X, center.Y - extents.Y, center.Z + extents.Z); // bottom-right-front
            Points[6] = new Vector3(center.X - extents.X, center.Y + extents.Y, center.Z + extents.Z); // top-left-front
            Points[7] = new Vector3(center.X + extents.X, center.Y + extents.Y, center.Z + extents.Z); // top-right-front
        }
    }
}
