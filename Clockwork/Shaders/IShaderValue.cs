using Clockwork.Common;

namespace Clockwork.Shaders
{
    public interface IShaderValue
    {
        public void Apply(ShaderValue sender, Shader shader);
    }
}
