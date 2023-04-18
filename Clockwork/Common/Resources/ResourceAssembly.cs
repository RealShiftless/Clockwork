using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Resources
{
    public sealed class ResourceAssembly
    {
        // Values
        private Assembly _assembly;
        private string _name;

        private Dictionary<uint, Resource> _resources = new Dictionary<uint, Resource>();
        private Dictionary<string, uint> _resourceDict = new Dictionary<string, uint>();

        private uint _nextId = 0;


        // Properties
        public string Name => _name;


        // Events
        public event Action Disposing;


        // Constructor
        internal ResourceAssembly(Assembly assembly)
        {
            _assembly = assembly;

            AssemblyName nameObj = _assembly.GetName();

            if (nameObj.Name == null)
                throw new Exception("Name of assembly is null!");

            _name = nameObj.Name;
        }


        // Func
        internal T Load<T>(string name) where T : Resource, new()
        {
            // Check if the resource was already loaded
            if (_resourceDict.ContainsKey(name))
            {
                // Return that resource
                return (T)_resources[_resourceDict[name]];
            }
            else
            {
                // Create a new resource stream
                Stream stream = GetManifestResourceStream(name);

                // Create and initialize the object
                T resource = new T();
                _resources.Add(_nextId, resource);

                resource.Initialize(this, _nextId, name, stream);

                // Up the next id so the next content will not overwrite the last
                _nextId++;

                // Return the resource
                return resource;
            }
        }

        public Stream GetManifestResourceStream(string name)
        {
            Stream? stream = _assembly.GetManifestResourceStream(name);

            if (stream == null)
                throw new FileLoadException("Could not load resource " + name + ", in assembly " + Name);

            return stream;
        }

        internal void RemoveAt(uint id)
        {
            _resourceDict.Remove(_resources[id].ResourceName);
            _resources.Remove(id);
        }

        internal void Dispose()
        {
            Disposing?.Invoke();
        }


        // Getters
        public Resource GetAt(uint id)
        {
            return _resources[id];
        }
        public T GetAt<T>(uint id) where T : Resource, new()
        {
            return (T)GetAt(id);
        }

        public Resource Get(string name)
        {
            return GetAt(_resourceDict[name]);
        }
        public T Get<T>(string name) where T : Resource, new()
        {
            return (T)Get(name);
        }
    }
}
