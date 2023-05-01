using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Resources
{
    public abstract class Resource
    {
        // Values
        private ResourceManager _manager;
        private ResourceAssembly _assembly;
        private uint _id;
        private string _name;

        private uint _references;


        // Properties
        public ResourceManager ResourceManager => _manager;
        public ResourceAssembly ResourceAssembly => _assembly;
        public uint ResourceId => _id;
        public string ResourceName => _name;

        public bool IsManaged => ResourceManager != null;

        internal uint References
        {
            get => _references;
            set
            {
                _references = value;

                if(value == 0)
                    Destroy();
            }
        }


        // Func
        internal void Populate(ResourceManager manager, ResourceAssembly assembly, uint id, string name, Stream stream)
        {
            _manager = manager;
            _assembly = assembly;
            _id = id;
            _name = name;

            manager.Disposing += () => Destroy();

            Populate(stream);
        }
        internal void Destroy()
        {
            Dispose();
            ResourceManager.RemoveAt(ResourceId);
        }
        internal ResourcePtr Bind(ResourceLibrary sender)
        {
            Bound(sender);
            return new ResourcePtr(this);
        }


        // Abstracts
        protected abstract void Populate(Stream stream);


        // Virtuals
        protected virtual void Bound(ResourceLibrary sender) { }
        protected virtual void Dispose() { }


        // Overrides
        public override string ToString()
        {
            return "{ Name: " + ResourceName + ", Id: " + ResourceId + ", Assembly: " + ResourceAssembly.Name + " }";
        }
    }
}
