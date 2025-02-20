﻿using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxels
{
    public class Chunk
    {
        public BlockData.BlockType[] _blocks = new BlockData.BlockType[ChunkData.GenerationSize * ChunkData.GenerationSize * ChunkData.GenerationHeight];
        public List<float> _vertices = new List<float>();
        public Vector2i Position { get; set; } = Vector2i.Zero;
        public bool NeedsToBeGenerated = true;
        public bool NeedsToBeMeshed = true;

        private int _vaoHandle = 0;
        private int _vboHandle = 0;

        public void Generate(FastNoiseLite noise)
        {
            // Generate terrain for the full area including padding
            for (int x = 0; x < ChunkData.GenerationSize; x++)
            {
                for (int z = 0; z < ChunkData.GenerationSize; z++)
                {
                    // Convert to world coordinates correctly
                    int worldX = Position.X * ChunkData.Size + (x - 1);  // Subtract 1 for padding
                    int worldZ = Position.Y * ChunkData.Size + (z - 1);  // Subtract 1 for padding

                    float height = (noise.GetNoise((float)worldX, (float)worldZ) + 1) * (ChunkData.GenerationHeight / 16);

                    for (int y = 0; y < ChunkData.GenerationHeight; ++y)
                    {
                        int index = GetBlockAt(x, y, z);

                        if (y - 1 < height)  // Subtract 1 for padding
                        {
                            _blocks[index] = BlockData.BlockType.Dirt;
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

            // Generate faces for the main chunk area (excluding padding)
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

                        // Convert from padded coordinates to chunk-local coordinates when adding faces
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

            Vector3 color = BlockData.BlockTypeColorLookup[blockType];

            vertices.ForEach(vertex =>
            {
                _vertices.Add(vertex.X + x  + Position.X * ChunkData.Size);
                _vertices.Add(vertex.Y + y);
                _vertices.Add(vertex.Z + z + Position.Y * ChunkData.Size);

                _vertices.Add(color.X);
                _vertices.Add(color.Y); 
                _vertices.Add(color.Z);
            });
        }

        public void UploadMesh()
        {
            if (_vboHandle == 0)
            {
                _vboHandle = GL.GenBuffer();
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vboHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, _vertices.Count() * sizeof(float), _vertices.ToArray(), BufferUsageHint.StaticDraw);

            if (_vaoHandle == 0)
            {
                _vaoHandle = GL.GenVertexArray();
            }
            GL.BindVertexArray(_vaoHandle);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, 6 * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
        }

        public void Render()
        {
            GL.BindVertexArray(_vaoHandle);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertices.Count() / 6);
            GL.BindVertexArray(0);
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

        public static BoundingBox GetBoundingBox(int chunkPosX, int chunkPosZ)
        {
            float worldMinX = chunkPosX * ChunkData.Size;
            float worldMinZ = chunkPosZ * ChunkData.Size;

            Vector3 center = new Vector3(
                worldMinX + ChunkData.Size / 2f,
                0,
                worldMinZ + ChunkData.Size / 2f
            );

            Vector3 extents = new Vector3(
                ChunkData.Size / 2f,
                ChunkData.Height / 2f,
                ChunkData.Size / 2f
            );

            return new BoundingBox(center, extents);
        }
    }
}
