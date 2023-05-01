using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Logging
{
    public interface ILoggerType
    {
        public void Initialize(Logger logger);
    }
}
