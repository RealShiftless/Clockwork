using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Resources
{
    public class ResourceAssemblyManager
    {
        private Dictionary<string, ResourceAssembly> _resourceAssemblies = new Dictionary<string, ResourceAssembly>();

        public ResourceAssembly this[string name] => GetAssembly(name);

        internal ResourceAssemblyManager()
        {

        }

        public ResourceAssembly Create(Assembly assembly)
        {
            string? assemblyName = assembly.GetName().Name;

            if (assemblyName == null)
                throw new Exception("Name of assembly was null!");

            ResourceAssembly resourceAssembly = new ResourceAssembly(assembly, assemblyName);

            _resourceAssemblies.Add(assemblyName, resourceAssembly);

            return resourceAssembly;
        }

        public ResourceAssembly GetAssembly(string name)
        {
            return _resourceAssemblies[name];
        }
        public ResourceAssembly? TryGetAssembly(string name)
        {
            try
            {
                return GetAssembly(name);
            }
            catch
            {
                return null;
            }
        }
    }
}
