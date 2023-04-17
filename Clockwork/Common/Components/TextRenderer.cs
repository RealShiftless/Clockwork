using Clockwork.Common.GameObjects;
using Clockwork.Rendering;
using Clockwork.ResourcesDeprecated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Components
{
    internal class TextRenderer : RenderingComponent
    {
        public string Text = "";
        public Font Font;

        public TextRenderer(string text, Font font)
        {
            Text = text;
            Font = font;
        }

        protected override void OnRender(object sender, FrameEventArgs e)
        {

        }
    }
}
