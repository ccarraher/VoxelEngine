using OpenTK.Mathematics;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Voxels
{
    public class World
    {
        public int ViewDistance { get; private set; } = 24;

        private Dictionary<Vector2i, Chunk> _chunks = new Dictionary<Vector2i, Chunk>();
        private Queue<Chunk> _chunkPool = new Queue<Chunk>();
        //private readonly ConcurrentQueue<(Vector2i pos, Chunk chunk)> _completedChunks = new();
        //private readonly ConcurrentDictionary<Vector2i, byte> _pendingChunks = new();
        private FastNoiseLite _noise = new FastNoiseLite();

        public void Init()
        {
            // Initialize noise with random seed
            _noise.SetSeed(new Random().Next());
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            _noise.SetFractalOctaves(8);

            for (int i = 0; i < 1000; i++)
            {
                _chunkPool.Enqueue(new Chunk());
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

            if (!_chunks.ContainsKey(new Vector2i(playerChunkPosX, playerChunkPosZ)))
            {
                LoadChunk(playerChunkPosX, playerChunkPosZ);
            }

            for (int d = 1; d < ViewDistance; d++)
            {
                for (int x = -d; x <= d; x++)
                {
                    for (int z = -d; z <= d; z++)
                    {
                        int chunkPosX = x + playerChunkPosX;
                        int chunkPosZ = z + playerChunkPosZ;

                        if (!_chunks.ContainsKey(new Vector2i(chunkPosX, chunkPosZ)))
                        {
                            LoadChunk(chunkPosX, chunkPosZ);
                            break;
                        }
                    }
                }
            }

            //ProcessCompletedChunks();
        }

        public void LoadChunk(int x, int z)
        {
            var chunkPos = new Vector2i(x, z);

            //if (!_pendingChunks.TryAdd(chunkPos, 0))
            //{
            //    return;
            //}

            Chunk chunk = _chunkPool.Count > 0 ? _chunkPool.Dequeue() : new Chunk();

            //var task = Task.Run(() =>
            //{
            //    chunk.Generate(_noise, chunkPos);
            //    _completedChunks.Enqueue((chunkPos, chunk));
            //    _pendingChunks.Remove(chunkPos, out var _);
            //});

            //task.Wait(1);
            //if (task.IsCompleted)
            //{
            chunk.Generate(_noise, chunkPos);
            chunk.GenerateMesh();
            chunk.UploadMesh();
            _chunks.Add(chunkPos, chunk);
            //}

            //Console.WriteLine($"Chunk generated at position ({x}, {z})");
        }

        //public void ProcessCompletedChunks()
        //{
        //    while (_completedChunks.TryDequeue(out var chunkData))
        //    {
        //        var (pos, chunk) = chunkData;
        //        chunk.GenerateMesh();
        //        chunk.UploadMesh();
        //        _chunks.TryAdd(pos, chunk);
        //    }
        //}

        public void UnloadOutOfRangeChunks(int startX, int endX, int startZ, int endZ)
        {
            foreach (var chunk in _chunks)
            {
                Vector2i pos = chunk.Key;

                if (pos.X < startX || pos.X > endX || pos.Y < startZ || pos.Y > endZ)
                {
                    chunk.Value.Clear();
                    _chunkPool.Enqueue(chunk.Value);
                    //_chunks.Remove(pos);
                }
            }
        }

        public void Render()
        {
            foreach (var chunk in _chunks)
            {
                chunk.Value.Render();
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
