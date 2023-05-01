using Newtonsoft.Json;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpFont;
using Clockwork.Common.Resources;

namespace Clockwork.Common
{
    public class FontPackage : Resource
    {
        private Dictionary<string, Font> _fonts = new Dictionary<string, Font>();

        protected override void Populate(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            string json = reader.ReadToEnd();
            reader.Close();

            List<FontInfo>? fonts = JsonConvert.DeserializeObject<List<FontInfo>>(json);

            if (fonts == null)
                throw new FileLoadException("Could not load font package of name " + ResourceName);

            Library lib = new Library();
            foreach (FontInfo fontInfo in fonts)
            {

            }

        }

        protected override void Dispose()
        {
        }
    }
}
