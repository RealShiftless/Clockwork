using Clockwork.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Shaders
{
    public struct ShaderFloat : IShaderValue
    {
        // Values
        public float Value;


        // Constructors
        public ShaderFloat(float value)
        {
            Value = value;
        }


        // Interface
        public void Apply(ShaderValue sender, Shader shader)
        {
            shader.SetFloat(sender.Name, Value);
        }
    }
}
