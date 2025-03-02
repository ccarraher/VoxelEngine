using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace Voxels
{
    public class BlockMesh
    {
        private int _vbo;
        private int _vao;

        public void Init(ChunkVertex[] vertices)
        {
            _vao = GL.GenVertexArray();
            GL.BindVertexArray(_vao);

            _vbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);

            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * ChunkVertex.SizeInBytes, vertices, BufferUsageHint.StaticDraw);

            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, ChunkVertex.SizeInBytes, 0);
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribIPointer(1, 1, VertexAttribIntegerType.Int, ChunkVertex.SizeInBytes, Marshal.OffsetOf<ChunkVertex>("Normal").ToInt32());
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribIPointer(2, 1, VertexAttribIntegerType.Int, ChunkVertex.SizeInBytes, Marshal.OffsetOf<ChunkVertex>("Color").ToInt32());
            GL.EnableVertexAttribArray(2);

            GL.VertexAttribPointer(3, 1, VertexAttribPointerType.Float, false, ChunkVertex.SizeInBytes, Marshal.OffsetOf<ChunkVertex>("AmbientOcclusion").ToInt32());
            GL.EnableVertexAttribArray(3);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Render(int numTriangles)
        {
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, numTriangles);
            GL.BindVertexArray(0);
        }

        public void Delete()
        {
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
        }
    }
}
