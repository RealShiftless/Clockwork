using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clockwork.ResourcesDeprecated;

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
