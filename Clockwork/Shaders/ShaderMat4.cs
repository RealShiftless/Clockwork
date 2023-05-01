using Clockwork.Common;
using OpenTK.Mathematics;

namespace Clockwork.Shaders
{
    public struct ShaderMat4 : IShaderValue
    {
        // Values
        public Matrix4 Value;

        
        // Constructor
        public ShaderMat4(Matrix4 value)
        {
            Value = value;
        }


        // Interface
        public void Apply(ShaderValue sender, Shader shader)
        {
            shader.SetMatrix4(sender.Name, Value);
        }
    }
}
