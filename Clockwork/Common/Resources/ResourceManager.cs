using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Clockwork.Common.Resources
{
    public delegate void ResourceEventHandler(ResourceManager sender, ResourceEventArgs e);

    public class ResourceManager
    {
        // Values
        private Game _game;
        private ResourceAssemblyManager _assemblies;

        private Dictionary<uint, Resource> _resources = new Dictionary<uint, Resource>();
        private Dictionary<string, uint> _resourceDict = new Dictionary<string, uint>();

        private List<ResourceLibrary> _libraries = new List<ResourceLibrary>();

        private uint _nextId = 0;


        // Properties
        public Game Game => _game;
        public ResourceAssemblyManager ResourceAssemblies => _assemblies;


        // Events
        public event Action Disposing;

        public event ResourceEventHandler ResourceLoaded;
        public event ResourceEventHandler ResourceGot;
        public event ResourceEventHandler ResourceUnloaded;


        // Constructor
        internal ResourceManager(Game game)
        {
            _game = game;

            _assemblies = new ResourceAssemblyManager();
        }


        // Func
        internal T Load<T>(ResourceAssembly assembly, string location) where T : Resource, new()
        {
            string resourceLocation = assembly.Name + "." + location;

            if (_resourceDict.ContainsKey(resourceLocation))
            {
                return Get<T>(resourceLocation);
            }
            else
            {
                // Load the stream
                Stream stream = assembly.GetManifestResourceStream(location);

                // Create a new resource
                T resource = new T();

                // Add it to the manager
                _resources.Add(_nextId, resource);
                _resourceDict[assembly.Name + "." + location] = _nextId;

                // Populate the resource
                resource.Populate(this, assembly, _nextId, location, stream);

                // Up the next id so the next content will not overwrite the last
                _nextId++;

                ResourceLoaded?.Invoke(this, new ResourceEventArgs(resource));

                // Return the resource
                return resource;
            }
        }
        internal void RemoveAt(uint id)
        {
            Resource resource = _resources[id];

            _resourceDict.Remove(resource.ResourceName);
            _resources.Remove(resource.ResourceId);

            ResourceUnloaded?.Invoke(this, new ResourceEventArgs(resource));
        }

        internal void Dispose()
        {
            Disposing?.Invoke();
        }

        public ResourceLibrary CreateLibrary()
        {
            ResourceLibrary library = new ResourceLibrary(this);
            _libraries.Add(library);

            return library;
        }
        internal void DisposeLibrary(ResourceLibrary library)
        {
            _libraries.Remove(library);
        }


        // Getters
        public Resource GetAt(uint id)
        {
            Resource resource = _resources[id];
            ResourceGot?.Invoke(this, new ResourceEventArgs(resource));

            return resource;
        }
        public Resource? TryGetAt(uint id)
        {
            try
            {
                return GetAt(id);
            }
            catch
            {
                return null;
            }
        }

        public T GetAt<T>(uint id) where T : Resource, new()
        {
            return (T)GetAt(id);
        }
        public T? TryGetAt<T>(uint id) where T : Resource, new()
        {
            try
            {
                return GetAt<T>(id);
            }
            catch
            {
                return null;
            }
        }

        public Resource Get(string location)
        {
            return GetAt(_resourceDict[location]);
        }
        public Resource? TryGet(string location)
        {
            try
            {
                return Get(location);
            }
            catch
            {
                return null;
            }
        }

        public T Get<T>(string location) where T : Resource, new()
        {
            return (T)Get(location);
        }
        public T? TryGet<T>(string location) where T : Resource, new()
        {
            try
            {
                return Get<T>(location);
            }
            catch
            {
                return null;
            }
        }
    }
}
