using OpenTK.Mathematics;

namespace Voxels
{
    public static class BlockData
    {
        public enum BlockType
        {
            Air,
            Dirt,
            Water,
            Grass
        }

        public enum BlockFace
        {
            Front,
            Back,
            Left,
            Right,
            Bottom,
            Top
        }

        public static readonly Dictionary<BlockFace, List<Vector3>> BlockFaceVerticesLookup = new Dictionary<BlockFace, List<Vector3>>()
        {
            {
                BlockFace.Front,
                new List<Vector3>()
                {
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f,  0.5f, -0.5f),
                    new Vector3(0.5f,  0.5f, -0.5f),
                    new Vector3(-0.5f,  0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f)
                }
            },
            {
                BlockFace.Back,
                new List<Vector3>()
                {
                    new Vector3(-0.5f, -0.5f,  0.5f),
                    new Vector3( 0.5f, -0.5f,  0.5f),
                    new Vector3( 0.5f,  0.5f,  0.5f),
                    new Vector3( 0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f, -0.5f,  0.5f)
                }
            },
            {
                BlockFace.Left,
                new List<Vector3>
                {
                    new Vector3(-0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f,  0.5f)
                }
            },
            {
                BlockFace.Right,
                new List<Vector3>
                {
                    new Vector3( 0.5f,  0.5f,  0.5f),
                    new Vector3( 0.5f,  0.5f, -0.5f),
                    new Vector3( 0.5f, -0.5f, -0.5f),
                    new Vector3( 0.5f, -0.5f, -0.5f),
                    new Vector3( 0.5f, -0.5f,  0.5f),
                    new Vector3( 0.5f,  0.5f,  0.5f)
                }
            },
            {
                BlockFace.Bottom,
                new List<Vector3>
                {
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3( 0.5f, -0.5f, -0.5f),
                    new Vector3( 0.5f, -0.5f,  0.5f),
                    new Vector3( 0.5f, -0.5f,  0.5f),
                    new Vector3(-0.5f, -0.5f,  0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f)
                }
            },
            {
                BlockFace.Top,
                new List<Vector3>
                {
                    new Vector3(-0.5f,  0.5f, -0.5f),
                    new Vector3( 0.5f,  0.5f, -0.5f),
                    new Vector3( 0.5f,  0.5f,  0.5f),
                    new Vector3( 0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f, -0.5f)
                }
            }
        };
    }
}
