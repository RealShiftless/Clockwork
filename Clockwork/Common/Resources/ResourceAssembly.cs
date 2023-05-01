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


        // Properties
        public string Name => _name;


        // Constructor
        internal ResourceAssembly(Assembly assembly, string name)
        {
            _assembly = assembly;

            _name = name;
        }


        // Func
        public Stream GetManifestResourceStream(string location)
        {
            Stream? stream = _assembly.GetManifestResourceStream(Name + "." + location);

            if (stream == null)
                throw new FileLoadException("Could not load resource " + location + ", in assembly " + Name);

            return stream;
        }
    }
}
