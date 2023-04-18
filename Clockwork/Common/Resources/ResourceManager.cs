using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Resources
{
    public sealed class ResourceManager
    {
        // Values
        private ResourceAssembly _assembly;

        private Dictionary<string, ResourcePtr> _resources = new Dictionary<string, ResourcePtr>();


        // Constructor
        public ResourceManager([NotNull] ResourceAssembly assembly)
        {
            _assembly = assembly;

            _assembly.Disposing += Dispose;
        }


        // Func
        public T Load<T>(string name) where T : Resource, new()
        {
            if(_resources.ContainsKey(name))
            {
                return _assembly.GetAt<T>(_resources[name].Id);
            }
            else
            {
                T resource = _assembly.Load<T>(name);
                _resources.Add(name, new ResourcePtr(resource));
                return resource;
            }
            
        }
        public void Dispose()
        {
            foreach(ResourcePtr ptr in _resources.Values)
            {
                ptr.Dispose();
            }

            _resources.Clear();
        }
    }
}
