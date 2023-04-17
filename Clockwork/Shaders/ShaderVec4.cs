using Clockwork.ResourcesDeprecated;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Shaders
{
    public struct ShaderVec4 : IShaderValue
    {
        // Values
        public float X, Y, Z, W;


        // Constructors
        public ShaderVec4(Vector4 value)
        {
            X = value.X; 
            Y = value.Y;
            Z = value.Z; 
            W = value.W;
        }
        public ShaderVec4(Color4 color) : this((Vector4)color) { }


        // Interface
        public void Apply(ShaderValue sender, Shader shader)
        {
            shader.SetVector4(sender.Name, new Vector4(X, Y, Z, W));
        }
    }
}
