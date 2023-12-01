using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace WindowTemplate.Common
{
    public class ComputeShader : IDisposable
    {
        public readonly int handle;
        int compute_shader;

        private readonly Dictionary<string, int> uniform_locations;

        public ComputeShader(string path)
        {
            string shader_source;
            using (var stream = File.Open(path, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                shader_source = reader.ReadToEnd();
            }

            compute_shader = GL.CreateShader(ShaderType.ComputeShader);

            GL.ShaderSource(compute_shader, shader_source);
            CompileShader(compute_shader);

            handle = GL.CreateProgram();

            GL.AttachShader(handle, compute_shader);

            LinkProgram(handle);

            GL.DetachShader(handle, compute_shader);

            GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            uniform_locations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(handle, i, out _, out _);

                var location = GL.GetUniformLocation(handle, key);

                uniform_locations.Add(key, location);
            }
        }

        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);
            GL.GetShader(shader, ShaderParameter.CompileStatus, out var code);
            if (code != (int)All.True)
            {
                var infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }

        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out var code);
            if (code != (int)All.True)
            {
                throw new Exception($"Error occurred whilst linking Program({program})");
            }
        }

        public void Use()
        {
            GL.UseProgram(handle);
        }
        
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(handle, attribName);
        }

        public void SetInt(string name, int data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(uniform_locations[name], data);
        }

        public void SetFloat(string name, float data)
        {
            GL.UseProgram(handle);
            GL.Uniform1(uniform_locations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(handle);
            GL.UniformMatrix4(uniform_locations[name], true, ref data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(handle);
            GL.Uniform3(uniform_locations[name], data);
        }

        public void SetVector2(string name, Vector2 data)
        {
            GL.UseProgram(handle);
            GL.Uniform2(uniform_locations[name], data);
        }

        public void Dispose()
        {
            GL.DetachShader(handle, compute_shader);
            GL.DeleteShader(compute_shader);
            GL.DeleteProgram(handle);
            GL.UseProgram(0);
        }
    }
}