using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace WindowTemplate.Common
{
    public class Shader : IDisposable
    {
        public readonly int Handle;
        int vertexShader, fragmentShader;

        private readonly Dictionary<string, int> _uniformLocations;

        public Shader(string vertPath, string fragPath, string geometryPath = "none")
        {
            string shaderSource;
            using (var stream = File.Open(vertPath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                shaderSource = reader.ReadToEnd();
                reader.Close();
            }

            vertexShader = GL.CreateShader(ShaderType.VertexShader);

            GL.ShaderSource(vertexShader, shaderSource);

            CompileShader(vertexShader);

            using (var stream = File.Open(fragPath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                shaderSource = reader.ReadToEnd();
            }

            fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, shaderSource);
            CompileShader(fragmentShader);

            int geometryShader = 0;
            if (geometryPath != "none")
            {
                shaderSource = File.ReadAllText(geometryPath);
                geometryShader = GL.CreateShader(ShaderType.GeometryShader);
                GL.ShaderSource(geometryShader, shaderSource);
                CompileShader(geometryShader);
            }

            Handle = GL.CreateProgram();

            GL.AttachShader(Handle, vertexShader);
            GL.AttachShader(Handle, fragmentShader);
            if (geometryPath != "none") GL.AttachShader(Handle, geometryShader);

            LinkProgram(Handle);

            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            if (geometryPath != "none") GL.DetachShader(Handle, geometryShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
            if (geometryPath != "none") GL.DeleteShader(geometryShader);

            GL.GetProgram(Handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

            _uniformLocations = new Dictionary<string, int>();

            for (var i = 0; i < numberOfUniforms; i++)
            {
                var key = GL.GetActiveUniform(Handle, i, out _, out _);
                var location = GL.GetUniformLocation(Handle, key);
                _uniformLocations.Add(key, location);
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
            GL.UseProgram(Handle);
        }
        
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }

        public void SetInt(string name, int data)
        {
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetFloat(string name, float data)
        {
            GL.Uniform1(_uniformLocations[name], data);
        }

        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UniformMatrix4(_uniformLocations[name], true, ref data);
        }

        public void SetVector4(string name, Vector4 data)
        {
            GL.Uniform4(_uniformLocations[name], data);
        }

        public void SetVector3(string name, Vector3 data)
        {
            GL.Uniform3(_uniformLocations[name], data);
        }

        public void SetVector2(string name, Vector2 data)
        {
            GL.Uniform2(_uniformLocations[name], data);
        }

        public void Dispose()
        {
            GL.DetachShader(Handle, vertexShader);
            GL.DetachShader(Handle, fragmentShader);
            GL.DeleteShader(fragmentShader);
            GL.DeleteShader(vertexShader);
            GL.DeleteProgram(Handle);
            GL.UseProgram(0);
        }
    }
}