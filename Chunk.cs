using OpenTK.Mathematics;
using static Voxels.BlockData;

namespace Voxels
{
    public class Chunk
    {
        public BlockData.BlockType[] _blocks = new BlockData.BlockType[ChunkData.GenerationSize * ChunkData.GenerationSize * ChunkData.GenerationHeight];

        private Vector2i _position = Vector2i.Zero;
        public Vector2i Position 
        { 
            get => _position; 
            set 
            {
                _position = value;
                CalculateBoundBox(value);
            } 
        }

        private BoundingBox _boundingBox = new BoundingBox();
        public BoundingBox BoundingBox => _boundingBox;

        public bool NeedsToBeGenerated = true;
        public bool NeedsToBeMeshed = true;

        private BlockMesh _mesh = new BlockMesh();
        private List<ChunkVertex> _vertices = new List<ChunkVertex>();

        private int _vertexCount = 0;

        public void Generate(FastNoiseLite noise)
        {
            for (int x = 0; x < ChunkData.GenerationSize; x++)
            {
                for (int z = 0; z < ChunkData.GenerationSize; z++)
                {
                    int worldX = Position.X * ChunkData.Size + (x - 1);
                    int worldZ = Position.Y * ChunkData.Size + (z - 1);

                    var height = GetHeight(noise, worldX, worldZ);

                    for (int y = 0; y < ChunkData.GenerationHeight; ++y)
                    {
                        int index = GetBlockAt(x, y, z);

                        if (y - 1 < height)
                        {
                            _blocks[index] = BlockData.BlockType.Dirt;
                        }
                        else if (y == 32)
                        {
                            _blocks[index] = BlockData.BlockType.Water;
                        }
                        else
                        {
                            _blocks[index] = BlockData.BlockType.Air;
                        }
                    }
                }
            }
        }

        public void GenerateMesh()
        {
            _vertices.Clear();

            for (int x = 1; x < ChunkData.GenerationSize - 1; x++)
            {
                for (int y = 1; y < ChunkData.GenerationHeight - 1; y++)
                {
                    for (int z = 1; z < ChunkData.GenerationSize - 1; z++)
                    {
                        int index = GetBlockAt(x, y, z);
                        BlockData.BlockType blockType = _blocks[index];

                        if (blockType == BlockData.BlockType.Air)
                        {
                            continue;
                        }

                        int localX = x - 1;
                        int localY = y - 1;
                        int localZ = z - 1;


                        if (_blocks[GetBlockAt(x - 1, y, z)] == BlockData.BlockType.Air)
                        {
                            AddFace(localX, localY, localZ, BlockData.BlockFace.Left, blockType);
                        }
                        if (_blocks[GetBlockAt(x + 1, y, z)] == BlockData.BlockType.Air)
                        {
                            AddFace(localX, localY, localZ, BlockData.BlockFace.Right, blockType);
                        }
                        if (_blocks[GetBlockAt(x, y - 1, z)] == BlockData.BlockType.Air)
                        {
                            AddFace(localX, localY, localZ, BlockData.BlockFace.Bottom, blockType);
                        }
                        if (_blocks[GetBlockAt(x, y + 1, z)] == BlockData.BlockType.Air)
                        {
                            AddFace(localX, localY, localZ, BlockData.BlockFace.Top, blockType);
                        }
                        if (_blocks[GetBlockAt(x, y, z - 1)] == BlockData.BlockType.Air)
                        {
                            AddFace(localX, localY, localZ, BlockData.BlockFace.Front, blockType);
                        }
                        if (_blocks[GetBlockAt(x, y, z + 1)] == BlockData.BlockType.Air)
                        {
                            AddFace(localX, localY, localZ, BlockData.BlockFace.Back, blockType);
                        }
                    }
                }
            }
        }

        public void AddFace(int x, int y, int z, BlockData.BlockFace blockFace, BlockData.BlockType blockType)
        {
            List<Vector3> vertices = BlockData.BlockFaceVerticesLookup[blockFace];
            for (int i = 0; i < vertices.Count; i++)
            {
                var worldPosition = new Vector3(
                    vertices[i].X + x + Position.X * ChunkData.Size,
                    vertices[i].Y + y,
                    vertices[i].Z + z + Position.Y * ChunkData.Size
                );

                float ao = CalculateAoForEachFaceVertex(x, y, z, blockFace, i);

                ChunkVertex vertex = new ChunkVertex(worldPosition, (int)blockFace, (int)blockType, ao);
                _vertices.Add(vertex);
            }
        }

        private float CalculateAoForEachFaceVertex(int x, int y, int z, BlockData.BlockFace blockFace, int vertexIndex)
        {
            int localX = x + 1;
            int localY = y + 1;
            int localZ = z + 1;

            switch (blockFace)
            {
                case BlockData.BlockFace.Front: // -Z face
                    switch (vertexIndex)
                    {
                        case 0: // Bottom-left
                        case 5:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX - 1, localY, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY - 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX, localY - 1, localZ - 1)] != BlockData.BlockType.Air
                            );
                        case 1: // Bottom-right
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX, localY - 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY - 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY, localZ - 1)] != BlockData.BlockType.Air
                            );
                        case 2:
                        case 3: // Top-right
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX + 1, localY, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX, localY + 1, localZ - 1)] != BlockData.BlockType.Air
                            );
                        case 4:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX, localY + 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY, localZ - 1)] != BlockData.BlockType.Air
                            );
                        default:
                            return 1.0f;
                    }

                case BlockData.BlockFace.Back: // +Z face
                    switch (vertexIndex)
                    {
                        case 0: // Bottom-left
                        case 5:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX - 1, localY, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY - 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX, localY - 1, localZ + 1)] != BlockData.BlockType.Air
                            );
                        case 1: // Bottom-right
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX, localY - 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY - 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY, localZ + 1)] != BlockData.BlockType.Air
                            );
                        case 2: // Top-right
                        case 3: // Top-right (duplicated)
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX + 1, localY, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX, localY + 1, localZ + 1)] != BlockData.BlockType.Air
                            );
                        case 4: // Top-left (duplicated)
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX, localY + 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY, localZ + 1)] != BlockData.BlockType.Air
                            );
                        default:
                            return 1.0f;
                    }

                case BlockData.BlockFace.Left: // -X face
                    switch (vertexIndex)
                    {
                        case 0: // Top-back
                        case 5:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY, localZ + 1)] != BlockData.BlockType.Air
                            );
                        case 1: // Top-front
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY, localZ - 1)] != BlockData.BlockType.Air
                            );
                        case 2: // Bottom-front
                        case 3:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX - 1, localY, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY - 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY - 1, localZ)] != BlockData.BlockType.Air
                            );
                        case 4: // Bottom back
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX - 1, localY - 1, localZ)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY - 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY, localZ + 1)] != BlockData.BlockType.Air
                            );
                        default:
                            return 1.0f;
                    }

                case BlockData.BlockFace.Right: // +X face
                    switch (vertexIndex)
                    {
                        case 0: // Top-back
                        case 5:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY, localZ + 1)] != BlockData.BlockType.Air
                            );
                        case 1: // Top-front
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY, localZ - 1)] != BlockData.BlockType.Air
                            );
                        case 2: // Bottom-front
                        case 3: // Bottom-front (duplicated)
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX + 1, localY, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY - 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY - 1, localZ)] != BlockData.BlockType.Air
                            );
                        case 4: // Bottom-back
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX + 1, localY - 1, localZ)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY - 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY, localZ + 1)] != BlockData.BlockType.Air
                            );
                        default:
                            return 1.0f;
                    }

                //case BlockData.BlockFace.Bottom: // -Y face
                //    switch (i)
                //    {
                //        case 0: // Front-left
                //        case 5:
                //            return CalculateAoForVertex(
                //                _blocks[GetBlockAt(x - 1, y - 1, z)] != BlockData.BlockType.Air,   // left
                //                _blocks[GetBlockAt(x - 1, y - 1, z - 1)] != BlockData.BlockType.Air, // left-front corner
                //                _blocks[GetBlockAt(x, y - 1, z - 1)] != BlockData.BlockType.Air    // front
                //            );
                //            break;
                //        case 1: // Front-right
                //            return CalculateAoForVertex(
                //                _blocks[GetBlockAt(x, y - 1, z - 1)] != BlockData.BlockType.Air,   // front
                //                _blocks[GetBlockAt(x + 1, y - 1, z - 1)] != BlockData.BlockType.Air, // right-front corner
                //                _blocks[GetBlockAt(x + 1, y - 1, z)] != BlockData.BlockType.Air    // right
                //            );
                //            break;
                //        case 2: // Back-right
                //        case 3: // Back-right (duplicated)
                //            return CalculateAoForVertex(
                //                _blocks[GetBlockAt(x + 1, y - 1, z)] != BlockData.BlockType.Air,   // right
                //                _blocks[GetBlockAt(x + 1, y - 1, z + 1)] != BlockData.BlockType.Air, // right-back corner
                //                _blocks[GetBlockAt(x, y - 1, z + 1)] != BlockData.BlockType.Air    // back
                //            );
                //            break;
                //        case 4: // Back-left (duplicated)
                //            return CalculateAoForVertex(
                //                _blocks[GetBlockAt(x, y - 1, z + 1)] != BlockData.BlockType.Air,   // back
                //                _blocks[GetBlockAt(x - 1, y - 1, z + 1)] != BlockData.BlockType.Air, // left-back corner
                //                _blocks[GetBlockAt(x - 1, y - 1, z)] != BlockData.BlockType.Air    // left
                //            );
                //            break;
                //        default:
                //            return 3;
                //            break;
                //    }
                //    break;

                case BlockData.BlockFace.Top: // +Y face
                    switch (vertexIndex)
                    {
                        case 0: // Back left
                        case 5:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX, localY + 1, localZ - 1)] != BlockData.BlockType.Air
                            );
                        case 1:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX, localY + 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ - 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ)] != BlockData.BlockType.Air
                            );
                        case 2: // Front right (first instance)
                        case 3:
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX + 1, localY + 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX, localY + 1, localZ + 1)] != BlockData.BlockType.Air
                            );
                        case 4: // Front left
                            return CalculateAoForVertex(
                                _blocks[GetBlockAt(localX, localY + 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ + 1)] != BlockData.BlockType.Air,
                                _blocks[GetBlockAt(localX - 1, localY + 1, localZ)] != BlockData.BlockType.Air
                            );
                        default:
                            return 1.0f;
                    }

                default:
                    return 1.0f; // Default to no occlusion
            }
        }

        private float CalculateAoForVertex(bool sideOne, bool corner, bool sideTwo)
        {
            if (sideOne && sideTwo)
            {
                return 0.75f;
            }
            else if ((sideOne && corner) || (sideTwo && corner))
            {
                return 0.85f;
            }
            else if (sideOne || sideTwo || corner)
            {
                return 0.95f;
            }
            else {
                return 1.0f;
            }
        }

        public void InitMesh()
        {
            _mesh.Init(_vertices.ToArray());

            _vertexCount = _vertices.Count();
            _vertices.Clear();
        }

        public void Render()
        {
            _mesh.Render(_vertexCount);
        }

        public int GetBlockAt(int x, int y, int z)
        {
            return x + (ChunkData.GenerationSize * z) + (ChunkData.GenerationSize * ChunkData.GenerationSize * y);
        }

        public void Clear()
        {
            Array.Clear(_blocks, 0, _blocks.Length);
            _vertices.Clear();
            NeedsToBeMeshed = true;
            NeedsToBeGenerated = true;
        }

        public void CalculateBoundBox(Vector2i position)
        {
            float worldMinX = position.X * ChunkData.Size;
            float worldMinZ = position.Y * ChunkData.Size;

            _boundingBox.Center = new Vector3(
                worldMinX + ChunkData.Size / 2f,
                0,
                worldMinZ + ChunkData.Size / 2f
            );

            _boundingBox.Extents = new Vector3(
                ChunkData.Size / 2f,
                ChunkData.Height / 2f,
                ChunkData.Size / 2f
            );
        }

        private float GetHeight(FastNoiseLite noise, int x, int z)
        {
            float h1 = (noise.GetNoise((float)x, (float)z) + 1) * (ChunkData.GenerationHeight / 2);
            noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
            noise.SetFractalType(FastNoiseLite.FractalType.FBm);
            noise.SetFrequency(0.005f);
            float h2 = (noise.GetNoise((float)x, (float)z) + 1) * (ChunkData.GenerationHeight / 32);

            return h1 + h2;
        }
    }
}
