using Clockwork.ResourcesDeprecated;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
