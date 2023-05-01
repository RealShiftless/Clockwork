using Clockwork.Common;

namespace Clockwork.Shaders
{
    public struct ShaderInt : IShaderValue
    {
        // Values
        public int Value;


        // Constructors
        public ShaderInt(int value)
        {
            Value = value;
        }


        // Interface
        public void Apply(ShaderValue sender, Shader shader)
        {
            shader.SetInt(sender.Name, Value);
        }
    }
}
