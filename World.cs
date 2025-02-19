using OpenTK.Mathematics;
using System.Collections.Concurrent;

namespace Voxels
{
    public class World
    {
        public int ViewDistance { get; private set; } = 16;

        private readonly ConcurrentDictionary<Vector2i, Chunk> _chunksToRender = new ConcurrentDictionary<Vector2i, Chunk>();
        private readonly Queue<Chunk> _chunkPool = new Queue<Chunk>();
        private readonly ConcurrentQueue<Chunk> _chunksToGenerate = new();
        private FastNoiseLite _noise = new FastNoiseLite();

        public void Init()
        {
            // Initialize noise with random seed
            _noise.SetSeed(new Random().Next());
            _noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2S);
            _noise.SetFractalOctaves(8);

            for (int i = 0; i < 250; i++)
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

            for (int d = 0; d < ViewDistance; d++)
            {
                for (int x = -d; x <= d; x++)
                {
                    for (int z = -d; z <= d; z++)
                    {
                        int chunkPosX = x + playerChunkPosX;
                        int chunkPosZ = z + playerChunkPosZ;

                        if (!_chunksToRender.ContainsKey(new Vector2i(chunkPosX, chunkPosZ)))
                        {
                            LoadChunk(chunkPosX, chunkPosZ);
                            //break;
                        }
                    }
                }
            }

            Parallel.ForEach(_chunksToGenerate, chunk =>
            {
                chunk.Generate(_noise);
                _chunksToRender.TryAdd(chunk.Position, chunk);
                _chunksToGenerate.TryDequeue(out _);
            });

            foreach (var chunk in _chunksToRender.Values)
            {
                if (chunk.NeedsToBeMeshed)
                {
                    chunk.GenerateMesh();
                    chunk.UploadMesh();
                    chunk.NeedsToBeMeshed = false;
                }
            }
        }

        public void LoadChunk(int x, int z)
        {
            var chunkPos = new Vector2i(x, z);

            Chunk chunk = _chunkPool.Count > 0 ? _chunkPool.Dequeue() : new Chunk();
            chunk.Position = chunkPos;

            _chunksToGenerate.Enqueue(chunk);

            //Console.WriteLine($"Chunk generated at position ({x}, {z})");
        }

        public void UnloadOutOfRangeChunks(int startX, int endX, int startZ, int endZ)
        {
            foreach (var chunk in _chunksToRender)
            {
                Vector2i pos = chunk.Key;

                if (pos.X < startX || pos.X > endX || pos.Y < startZ || pos.Y > endZ)
                {
                    chunk.Value.Clear();
                    _chunkPool.Enqueue(chunk.Value);
                    _chunksToRender.Remove(pos, out var _);
                }
            }
        }

        public void Render(Camera camera)
        {
            int renderedCount = 0;

            foreach (var chunk in _chunksToRender)
            {
                if (camera.BoundingBoxInFrustum(Chunk.GetBoundingBox(chunk.Key.X, chunk.Key.Y)))
                {
                    renderedCount++;
                    chunk.Value.Render();
                }
            }
            Console.WriteLine($"{renderedCount} out of {_chunksToRender.Count} rendered");
        }
    }
}
