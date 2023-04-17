using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.ResourceManagement
{
    public class ResourcePtr
    {
        public readonly ResourceAssembly Assembly;
        public readonly uint Id;

        internal ResourcePtr(Resource resource)
        {
            Assembly = resource.ResourceAssembly;
            Id = resource.ResourceId;

            resource.References++;
        }

        internal void Dispose()
        {
            Assembly.GetAt(Id).References--;
        }
    }
}
