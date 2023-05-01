using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Logging
{
    public class GeneralLogger : ILoggerType
    {
        public void Initialize(Logger logger)
        {
            new ResourceLogger().Initialize(logger);
            new GameLogger().Initialize(logger);
            new RendererLogger().Initialize(logger);
        }
    }
}
