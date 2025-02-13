using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Voxels
{
    public class Shader : IDisposable
    {
        public int Handle { get; set; }
        private bool DisposedValue = false;

        public Shader(string fragmentPath, string vertexPath)
        {
            string fragmentShaderSource = File.ReadAllText(fragmentPath);
            string vertexShaderSource = File.ReadAllText(vertexPath);

            int fragmentShaderHandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShaderHandle, fragmentShaderSource);

            int vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderSource);

            GL.CompileShader(fragmentShaderHandle);
            GL.GetShader(fragmentShaderHandle, ShaderParameter.CompileStatus, out int fragmentSuccess);
            if (fragmentSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShaderHandle);
                Console.WriteLine(infoLog);
            }

            GL.CompileShader(vertexShaderHandle);
            GL.GetShader(vertexShaderHandle, ShaderParameter.CompileStatus, out int vertexSuccess);
            if (fragmentSuccess == 0)
            {
                string infoLog = GL.GetShaderInfoLog(fragmentShaderHandle);
                Console.WriteLine(infoLog);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, fragmentShaderHandle);
            GL.AttachShader(Handle, vertexShaderHandle);

            GL.LinkProgram(Handle);

            GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int programSuccess);
            if (programSuccess == 0)
            {
                string infoLog = GL.GetProgramInfoLog(Handle);
                Console.WriteLine(infoLog);
            }

            GL.DetachShader(Handle, fragmentShaderHandle);
            GL.DetachShader(Handle, vertexShaderHandle);
            GL.DeleteShader(fragmentShaderHandle);
            GL.DeleteShader(vertexShaderHandle);
        }

        public void Use()
        {
            GL.UseProgram(Handle);
        }

        public void SetMatrix4(string name, Matrix4 value)
        {
            int location = GL.GetUniformLocation(Handle, name);
            GL.UniformMatrix4(location, false, ref value);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!DisposedValue)
            {
                GL.DeleteProgram(Handle);
                DisposedValue = true;
            }
        }

        ~Shader()
        {
            if (DisposedValue == false)
            {
                Console.WriteLine("GPU Resource Leak! Did you forget to call Dispose()?");
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
