using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Resources
{
    internal class ResourcePtr
    {
        public readonly ResourceAssembly Assembly;
        public readonly ResourceManager Manager;
        public readonly uint Id;

        internal ResourcePtr(Resource resource)
        {
            Assembly = resource.ResourceAssembly;
            Id = resource.ResourceId;

            resource.References++;
        }

        internal void Dispose()
        {
            Manager.GetAt(Id).References--;
        }
    }
}
