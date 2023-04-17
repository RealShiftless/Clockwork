using Clockwork.ResourcesDeprecated;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Shaders
{
    public sealed class ShaderValue
    {
        // Values
        public string Name;

        public IShaderValue? Value;


        // Properties
        [JsonIgnore]
        public bool IsNull => Value == null;


        // Constructors
        public ShaderValue()
        {

        }
        internal ShaderValue(string name, IShaderValue? value)
        {
            Name = name;

            Value = value;
        }

        internal void Apply(Shader shader)
        {
            if (Value != null)
                Value.Apply(this, shader);
        }
    }
}
