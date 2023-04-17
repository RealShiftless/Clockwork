using Clockwork.ResourcesDeprecated;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
