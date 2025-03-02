using OpenTK.Mathematics;
using System.Runtime.InteropServices;

namespace Voxels
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct ChunkVertex
    {
        public Vector3 Position;
        public int Normal;
        public int Color;
        public float AmbientOcclusion;

        public ChunkVertex(Vector3 position, int normal, int color, float ao) 
        {
            Position = position;
            Normal = normal;
            Color = color;
            AmbientOcclusion = ao;
        }

        public static int SizeInBytes => Marshal.SizeOf<ChunkVertex>();
    }
}
