using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.ResourcesDeprecated
{
    public class ResourceManager
    {
        // Constants
        public const string DEFAULT_NAME = "res.";


        // Static Values
        private static Dictionary<string, ResourceManager> _resourceManagers = new Dictionary<string, ResourceManager>();


        // Values
        public readonly Game Game;
        public readonly Assembly Assembly;

        public readonly string BaseDirectory;

        private string _assemblyName;

        private Dictionary<string, Resource> _resources = new Dictionary<string, Resource>();


        // Properties
        public string AssemblyName => _assemblyName + ".";

        public string ResourcesName => AssemblyName + DEFAULT_NAME;


        // Constructor
        
        public ResourceManager([DisallowNull] Game game, [DisallowNull] Assembly assembly, string baseDirectory)
        {
            string? assemblyName = assembly.GetName().Name;
            if (assemblyName == null)
                throw new Exception("Could not create ResourceManager as name of assembly was null!");

            if (_resourceManagers.ContainsKey(assemblyName))
                throw new ArgumentException("Assembly of name " + assemblyName + " already got a bound ResourceManager!");

            _assemblyName = assemblyName;

            Game = game;
            Assembly = assembly;

            BaseDirectory = baseDirectory;

            Game.Stopped += OnGameStopped;

            _resourceManagers.Add(assemblyName, this);
        }


        // Static Func
        private static T GetResource<T>(string assemblyName, string resourceName) where T : Resource, new()
        {
            return _resourceManagers[assemblyName].Get<T>(resourceName);
        }

        // Loading func
        public T Get<T>(string name) where T : Resource, new()
        {
            if(name.Contains(':'))
            {
                string[] names = name.Split(':');

                if (names.Length > 2)
                    throw new FileLoadException("Symbol ':' can only be used once!");

                if (names.Length < 2)
                    throw new FileLoadException("No resource location was found after ':' symbol!");

                return GetResource<T>(names[0], names[1]);
            }

            if (_resources.ContainsKey(name))
            {
                return (T)_resources[name];
            }
            else
            {
                Stream? stream = Assembly.GetManifestResourceStream(ResourcesName + name);

                if (stream == null)
                    throw new FileLoadException("Could not load resource of type " + typeof(T).FullName + " and of name " + name + " in assembly " + Assembly.FullName);

                T content = new T();
                content.Initialize(this, name, stream);

                _resources[name] = content;

                return content;
            }
        }


        // Event Callbacks
        private void OnGameStopped()
        {
            foreach (KeyValuePair<string, Resource> resource in _resources)
            {
                resource.Value.Dispose();
            }
        }
    }
}
