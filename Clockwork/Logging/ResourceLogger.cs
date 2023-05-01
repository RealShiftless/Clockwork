using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Logging
{
    public class ResourceLogger : ILoggerType
    {
        public void Initialize(Logger logger)
        {
            logger.Game.ResourceManager.ResourceLoaded += (s, e) => logger.Log(logger.Game.ResourceManager, "Loaded " + e.Resource.GetType().Name + " " + e.Resource);
            logger.Game.ResourceManager.ResourceUnloaded += (s, e) => logger.Log(logger.Game.ResourceManager, "Unloaded " + e.Resource.GetType().Name + " " + e.Resource);
            logger.Game.ResourceManager.ResourceGot += (s, e) => logger.Log(logger.Game.ResourceManager, "Got " + e.Resource);
        }
    }
}
