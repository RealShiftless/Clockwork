using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.ResourcesDeprecated
{
    public abstract class Resource
    {
        // Values
        private ResourceManager _resources;

        private string _resourceName;


        // Properties
        public ResourceManager Resources => _resources;

        public string ResourceName => _resourceName;


        // Func
        public abstract void Populate(Stream stream);
        public abstract void Dispose();

        internal void Initialize(ResourceManager parentResources, string resourceName, Stream stream)
        {
            _resources = parentResources;
            _resourceName = resourceName;

            Populate(stream);
        }
    }
}
