using Clockwork.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.GameObjects
{
    public abstract class UpdatingBehavior : BaseBehavior
    {
        protected override void Initialize()
        {
            GameObject.Update += Update;
            GameObject.LateUpdate += LateUpdate;
        }

        protected virtual void Update(object sender, FrameEventArgs e) { }
        protected virtual void LateUpdate(object sender, FrameEventArgs e) { }
    }
}
