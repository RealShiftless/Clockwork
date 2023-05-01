using Clockwork.Common.Resources;
using Clockwork.Shaders;
using Newtonsoft.Json;
using OpenTK.Mathematics;

namespace Clockwork.Common
{
    public class Material : Resource, ISerializableResource
    {
        // Values
        private string _name;
        private string _shaderLocation;

        private Shader _shader;

        private Dictionary<string, ShaderValue> _values = new Dictionary<string, ShaderValue>();

        
        // Properties
        public string Name => _name;

        public Shader Shader => _shader;


        // Static Func
        public static Material Create(string name, Shader shader)
        {
            Material material = new Material();
            material._name = name;
            material._shader = shader;

            return material;
        }
        public static T Copy<T>(T material, string newName) where T : Material
        {
            return (T)material.Copy(newName);
        }


        // Func
        public Material Copy(string newName)
        {
            Material materialCopy = new Material();
            materialCopy._name = newName;
            materialCopy._shader = Shader;

            materialCopy._values = new Dictionary<string, ShaderValue>(_values);

            return materialCopy;
        }

        public void SetInt(string name, int data)
        {
            CheckValueName(name);
            _values[name] = new ShaderValue(name, new ShaderInt(data));
        }
        public void SetFloat(string name, float data)
        {
            CheckValueName(name);
            _values[name] = new ShaderValue(name, new ShaderFloat(data));
        }
        public void SetMatrix4(string name, Matrix4 data)
        {
            CheckValueName(name);
            _values[name] = new ShaderValue(name, new ShaderMat4(data));
        }
        public void SetVector3(string name, Vector3 data)
        {
            CheckValueName(name);
            _values[name] = new ShaderValue(name, new ShaderVec3(data));
        }
        public void SetVector4(string name, Vector4 data)
        {
            CheckValueName(name);
            _values[name] = new ShaderValue(name, new ShaderVec4(data));
        }
        public void SetColor(string name, Color4 color)
        {
            SetVector4(name, (Vector4)color);
        }

        private void CheckValueName(string name)
        {
            if (!Shader.Uniforms.ContainsKey(name))
                throw new KeyNotFoundException("Value of name " + name + " was not found in shader of name " + Shader.ResourceName + " in material of name " + ResourceName);
        }

        public void Apply()
        {
            Shader.Bind();

            foreach(ShaderValue value in _values.Values)
            {
                value.Apply(Shader);
            }
        }


        // Overrides
        protected override void Populate(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            reader.Close();

            MaterialInfo info = JsonConvert.DeserializeObject<MaterialInfo>(json, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });

            _shaderLocation = info.Shader;
            _values = info.Values;
        }
        protected override void Bound(ResourceLibrary sender)
        {
            if(_shader == null)
            {
                _shader = sender.Load<Shader>(ResourceAssembly, _shaderLocation);
            }
            else
            {

            }
        }
        protected override void Dispose()
        {
        }


        // Interface
        public string Serialize()
        {
            MaterialInfo info = new MaterialInfo() { Name = _name, Shader = _shader.ResourceName, Values = _values };

            return JsonConvert.SerializeObject(info, Formatting.Indented, new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Auto,
            });
        }


        // Structs
        public struct MaterialInfo
        {
            public string Name;

            public string Shader;
            public Dictionary<string, ShaderValue> Values;
        }
    }
}
