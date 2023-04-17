using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.ResourceManagement
{
    public abstract class Resource
    {
        // Values
        private ResourceAssembly _assembly;
        private uint _id;
        private string _name;

        private uint _references;


        // Properties
        public ResourceAssembly ResourceAssembly => _assembly;
        public uint ResourceId => _id;
        public string ResourceName => _name;

        internal uint References
        {
            get => _references;
            set
            {
                _references = value;

                if(value == 0)
                {
                    _assembly.RemoveAt(ResourceId);
                    Dispose();
                }
            }
        }


        // Func
        internal void Initialize(ResourceAssembly assembly, uint id, string name, Stream stream)
        {
            _assembly = assembly;
            _id = id;
            _name = name;

            Populate(stream);
        }


        // Abstracts
        protected abstract void Populate(Stream stream);
        protected abstract void Dispose();
    }
}
