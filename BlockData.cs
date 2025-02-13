using OpenTK.Mathematics;

namespace Voxels
{
    public static class BlockData
    {
        public enum BlockType
        {
            Air,
            Dirt,
            Stone,
            Grass,
            BlockTypeCount
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

        public static readonly Dictionary<BlockType, Vector3> BlockTypeColorLookup = new Dictionary<BlockType, Vector3>()
        {
            { BlockType.Air, new Vector3(0.0f, 0.0f, 0.0f) },
            { BlockType.Dirt, new Vector3(0.4f, 0.2f, 0.1f) },
            { BlockType.Stone, new Vector3(0.5f, 0.5f, 0.5f) },
            { BlockType.Grass, new Vector3(0.0f, 0.8f, 0.0f) }
        };

        public static readonly Dictionary<BlockFace, List<Vector3>> BlockFaceVerticesLookup = new Dictionary<BlockFace, List<Vector3>>()
        {
            { 
                BlockFace.Front, 
                new List<Vector3>() 
                {
                    new Vector3(-0.5f, -0.5f, -0.5f),               
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, 0.5f, -0.5f),
                    new Vector3(0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, 0.5f, -0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f)                                
                } 
            },
            {
                BlockFace.Back,
                new List<Vector3>()
                {
                    new Vector3(-0.5f, -0.5f,  0.5f),
                    new Vector3(0.5f, -0.5f,  0.5f),
                    new Vector3(0.5f,  0.5f,  0.5f),
                    new Vector3(0.5f,  0.5f,  0.5f),
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
                    new Vector3(0.5f,  0.5f,  0.5f),
                    new Vector3(0.5f,  0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f,  0.5f),
                    new Vector3(0.5f,  0.5f,  0.5f)
                }
            },
            {
                BlockFace.Bottom,
                new List<Vector3>
                {
                    new Vector3(-0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f, -0.5f),
                    new Vector3(0.5f, -0.5f,  0.5f),
                    new Vector3(0.5f, -0.5f,  0.5f),
                    new Vector3(-0.5f, -0.5f,  0.5f),
                    new Vector3(-0.5f, -0.5f, -0.5f)
                }
            },
            {
                BlockFace.Top,
                new List<Vector3>
                {
                    new Vector3(-0.5f,  0.5f, -0.5f),
                    new Vector3(0.5f,  0.5f, -0.5f),
                    new Vector3(0.5f,  0.5f,  0.5f),
                    new Vector3(0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f,  0.5f),
                    new Vector3(-0.5f,  0.5f, -0.5f)
                }
            }
        };
    }
}
