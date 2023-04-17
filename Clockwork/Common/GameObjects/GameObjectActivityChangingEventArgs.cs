using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.GameObjects
{
    public struct GameObjectActivityChangingEventArgs
    {
        public readonly bool RequestedState;
        public bool Handled;

        public GameObjectActivityChangingEventArgs(bool requestedState)
        {
            RequestedState = requestedState;
            Handled = false;
        }
    }
}
