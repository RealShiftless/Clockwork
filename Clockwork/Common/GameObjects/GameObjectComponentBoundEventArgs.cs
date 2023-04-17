using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.GameObjects
{
    public struct GameObjectComponentBoundEventArgs
    {
        public readonly BaseComponent Component;

        public GameObjectComponentBoundEventArgs(BaseComponent component)
        {
            Component = component;
        }
    }
}
