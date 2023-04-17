using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.GameObjects
{
    internal struct GameObjectPtr
    {
        internal readonly GameObjectManager Manager;

        internal readonly int ID;
        internal readonly string Name;

        internal GameObjectPtr(GameObjectManager manager, int id, string name)
        {
            Manager = manager;
            ID = id;
            Name = name;
        }
    }
}
