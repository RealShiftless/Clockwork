using Clockwork.Common;
using Newtonsoft.Json;

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
