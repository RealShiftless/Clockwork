using Clockwork.Common;
using OpenTK.Mathematics;

namespace Clockwork.Shaders
{
    public struct ShaderVec3 : IShaderValue
    {
        // Values
        public Vector3 Value;

        
        // Constructor
        public ShaderVec3(Vector3 value)
        {
            Value = value;
        }


        // Interface
        public void Apply(ShaderValue sender, Shader shader)
        {
            shader.SetVector3(sender.Name, Value);
        }
    }
}
