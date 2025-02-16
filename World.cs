using OpenTK.Mathematics;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Voxels
{
    public class World
    {
        public int ViewDistance { get; private set; } = 16;

        private List<Chunk> _chunks = new List<Chunk>();
        //private Queue<Chunk> _chunkPool = new Queue<Chunk>();
        //private readonly ConcurrentQueue<(Vector2i pos, Chunk chunk)> _completedChunks = new();
        //private readonly ConcurrentDictionary<Vector2i, byte> _pendingChunks = new();
        private FastNoiseLite _noise = new FastNoiseLite();

        public void Init()
        {
            // Initialize noise with random seed
            _noise.SetSeed(new Random().Next());
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            _noise.SetFractalOctaves(8);

            for (int i = 0; i < (ViewDistance * ViewDistance * ViewDistance * ViewDistance) + ViewDistance; i++)
            {
                _chunks.Add(new Chunk());
            }
        }

        public void Update(Vector3 playerPosition)
        {
            int playerChunkPosX = (int)playerPosition.X / ChunkData.Size;
            int playerChunkPosZ = (int)playerPosition.Z / ChunkData.Size;
            //Console.WriteLine($"({playerChunkPosX}, {playerChunkPosZ})");

            int startX = playerChunkPosX - ViewDistance;
            int endX = playerChunkPosX + ViewDistance;
            int startZ = playerChunkPosZ - ViewDistance;
            int endZ = playerChunkPosZ + ViewDistance;

            UnloadOutOfRangeChunks(startX, endX, startZ, endZ);

            var playerChunkPos = new Vector2i(playerChunkPosX, playerChunkPosZ);
            if (!_chunks.Any(c => c.Position == playerChunkPos) && _chunks.Any(c => !c.IsLoaded))
            {
                LoadChunk(playerChunkPos);
            }

            for (int d = 1; d < ViewDistance; d++)
            {
                for (int x = -d; x <= d; x++)
                {
                    for (int z = -d; z <= d; z++)
                    {
                        int chunkPosX = x + playerChunkPosX;
                        int chunkPosZ = z + playerChunkPosZ;

                        var chunkPosition = new Vector2i(chunkPosX, chunkPosZ);
                        if (!_chunks.Any(c => c.Position == chunkPosition) && _chunks.Any(c => !c.IsLoaded))
                        {
                            LoadChunk(chunkPosition);
                            break;
                        }
                    }
                }
            }
        }

        public void LoadChunk(Vector2i chunkPosition)
        {
            var chunk = _chunks.FirstOrDefault(x => x.IsLoaded == false);
            if (chunk is not null)
            {
                chunk.Generate(_noise, chunkPosition);
                chunk.GenerateMesh();
                chunk.UploadMesh();
                Console.WriteLine($"Chunk generated at position ({chunkPosition.X}, {chunkPosition.Y})");
            }

        }

        public void UnloadOutOfRangeChunks(int startX, int endX, int startZ, int endZ)
        {
            foreach (var chunk in _chunks.Where(c => c.IsLoaded))
            {
                Vector2i pos = chunk.Position!.Value;

                if (pos.X < startX || pos.X > endX || pos.Y < startZ || pos.Y > endZ)
                {
                    chunk.Clear();
                }
            }
        }

        public void Render()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Render();
            }
        }

        public int GetChunkIndex(int x, int z)
        {
            if (x < 0)
            {
                x = x + WorldData.MaxChunks;
            }

            if (z < 0)
            {
                z = z + WorldData.MaxChunks;
            }

            return z * WorldData.MaxChunks + x;
        }
    }
}
