using Clockwork.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.GameObjects
{
    public abstract class RenderingComponent : BaseComponent
    {
        public FrameEventHandler Render;

        public RenderingComponent()
        {
            Bound += (obj) =>
            {
                GameObject.Render += OnObjectRender;
            };
        }

        private void OnObjectRender(object sender, FrameEventArgs e)
        {
            if (Enabled)
            {
                OnRender(sender, e);
                Render?.Invoke(this, e);
            }
        }

        protected abstract void OnRender(object sender, FrameEventArgs e);
    }
}
