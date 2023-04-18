using Clockwork.Common.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common
{
    public class Shader : Resource
    {
        // Values
        internal Dictionary<string, int> Uniforms;

        private int _handle;


        // Properties
        public int ProgramHandle => _handle;


        // Events
        public event Action<Shader> Populated;


        // Interface
        protected override void Populate(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string str = reader.ReadToEnd();
            reader.Close();

            ShaderInfo info = JsonConvert.DeserializeObject<ShaderInfo>(str);

            switch (info.Type)
            {
                case "Embedded":
                    Populate(info.VertexShader, info.FragmentShader);
                    break;

                case "Resource":
                    Stream? vertexStream = ResourceAssembly.GetManifestResourceStream(ResourceAssembly.Name + info.VertexShader);

                    if (vertexStream == null)
                        throw new FileLoadException("Could not load ResourceStream for vertex shader at location: " + info.VertexShader);

                    StreamReader vertexReader = new StreamReader(vertexStream);
                    string vertexShader = vertexReader.ReadToEnd();
                    vertexReader.Close();

                    Stream? fragmentStream = ResourceAssembly.GetManifestResourceStream(ResourceAssembly.Name + info.FragmentShader);

                    if (fragmentStream == null)
                        throw new FileLoadException("Could not load ResourceStream for fragment shader at location: " + info.FragmentShader);

                    StreamReader fragmentReader = new StreamReader(fragmentStream);
                    string fragmentShader = fragmentReader.ReadToEnd();
                    fragmentReader.Close();

                    Populate(vertexShader, fragmentShader);
                    break;

                case "Local":
                    Populate(File.ReadAllText(info.VertexShader), File.ReadAllText(info.FragmentShader));
                    break;

                default: throw new FileLoadException("Shader could not load due to unkown content type: " + info.Type);
            }
        }
        protected virtual void Populate(string vertexShader, string fragmentShader)
        {
            // Create the shaders
            int vertexPtr = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexPtr, vertexShader);
            CompileShader(vertexPtr);

            int fragmentPtr = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentPtr, fragmentShader);
            CompileShader(fragmentPtr);

            // Create the program
            _handle = GL.CreateProgram();

            // Attach and link
            GL.AttachShader(ProgramHandle, vertexPtr);
            GL.AttachShader(ProgramHandle, fragmentPtr);

            LinkProgram(ProgramHandle);

            // Detach and delete useless leftovers
            GL.DetachShader(ProgramHandle, vertexPtr);
            GL.DetachShader(ProgramHandle, fragmentPtr);
            GL.DeleteShader(fragmentPtr);
            GL.DeleteShader(vertexPtr);

            // Get the uniforms count
            GL.GetProgram(ProgramHandle, GetProgramParameterName.ActiveUniforms, out var uniformCount);

            // Index uniforms
            Uniforms = new Dictionary<string, int>();

            for (var i = 0; i < uniformCount; i++)
            {
                var key = GL.GetActiveUniform(ProgramHandle, i, out _, out _);
                var ptr = GL.GetUniformLocation(ProgramHandle, key);

                Uniforms.Add(key, ptr);
            }

            Populated?.Invoke(this);
        }
        protected override void Dispose()
        {
            GL.DeleteProgram(ProgramHandle);
        }


        // Func
        private static void CompileShader(int shader)
        {
            GL.CompileShader(shader);

            // Handle exceptions
            GL.GetShader(shader, ShaderParameter.CompileStatus, out int code);
            if (code != (int)All.True)
            {
                string infoLog = GL.GetShaderInfoLog(shader);
                throw new Exception($"Error occurred whilst compiling Shader({shader}).\n\n{infoLog}");
            }
        }
        private static void LinkProgram(int program)
        {
            GL.LinkProgram(program);

            // Handle exceptions
            GL.GetProgram(program, GetProgramParameterName.LinkStatus, out int code);
            if (code != (int)All.True)
                throw new Exception($"Error occurred whilst linking Program({program})");
        }

        public void Bind()
        {
            GL.UseProgram(Handle);
        }
        public int GetAttribLocation(string attribName)
        {
            return GL.GetAttribLocation(Handle, attribName);
        }


        // Setters
        public void SetInt(string name, int data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(Uniforms[name], data);
        }
        public void SetFloat(string name, float data)
        {
            GL.UseProgram(Handle);
            GL.Uniform1(Uniforms[name], data);
        }
        public void SetMatrix4(string name, Matrix4 data)
        {
            GL.UseProgram(Handle);
            GL.UniformMatrix4(Uniforms[name], true, ref data);
        }
        public void SetVector3(string name, Vector3 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform3(Uniforms[name], data);
        }
        public void SetVector4(string name, Vector4 data)
        {
            GL.UseProgram(Handle);
            GL.Uniform4(Uniforms[name], data);
        }


        // Enums
        internal enum ContentType
        {
            Embedded,
            Resource,
            Local
        }


        // Structs
        private struct ShaderInfo
        {
            public string Type;

            public string VertexShader;
            public string FragmentShader;
        }
    }


}
