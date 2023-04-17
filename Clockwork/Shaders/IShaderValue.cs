using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Clockwork.ResourcesDeprecated;

namespace Clockwork.Shaders
{
    public interface IShaderValue
    {
        public void Apply(ShaderValue sender, Shader shader);
    }
}
