using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Resources
{
    public sealed class ResourceLibrary
    {
        // Values
        public string DefaultLocation = "res.";

        private ResourceManager _resourceManager;

        private Dictionary<string, ResourcePtr> _resources = new Dictionary<string, ResourcePtr>();


        // Properties
        public ResourceManager Resources => _resourceManager;
        public Game Game => _resourceManager.Game;


        // Constructor
        internal ResourceLibrary(ResourceManager manager)
        {
            _resourceManager = manager;
        }


        // Func
        public T Load<T>(ResourceAssembly assembly, string name) where T : Resource, new()
        {
            if(_resources.ContainsKey(name))
            {
                return _resourceManager.GetAt<T>(_resources[name].Id);
            }
            else
            {
                T resource = Resources.Load<T>(assembly, DefaultLocation + name);
                _resources.Add(name, resource.Bind(this));

                return resource;
            }
            
        }
        public void Reference(Resource resource)
        {
            if (_resources.ContainsKey(resource.ResourceName))
                return;

            _resources.Add(resource.ResourceName, new ResourcePtr(resource));
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
