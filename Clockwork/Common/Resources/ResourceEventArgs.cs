using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Resources
{
    public struct ResourceEventArgs
    {
        public readonly Resource Resource;

        public ResourceEventArgs(Resource resource) 
        { 
            Resource = resource;
        }
    }
}
