using OpenTK.Mathematics;
using Clockwork.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Clockwork.Common.Mathematics;

namespace Clockwork.Common.GameObjects
{
    internal class GameObjectManager
    {
        // Values
        internal readonly GameState ParentState;

        private Dictionary<int, GameObject> _entities;
        private Dictionary<string, int> _namePtrs;


        // Constructor
        internal GameObjectManager(GameState parentState)
        {
            ParentState = parentState;

            _entities = new Dictionary<int, GameObject>();
            _namePtrs = new Dictionary<string, int>();

            // Bind events
            ParentState.Update += OnUpdate;
            ParentState.LateUpdate += OnLateUpdate;
            ParentState.Render += OnRender;
        }


        // Func
        internal GameObject InstantiateGameObject<T>(string name) where T : BaseBehavior, new()
        {
            // Checks if an object with the given name already exists
            if (_namePtrs.ContainsKey(name))
                throw new ArgumentException("Object of name " + name + " was already created in state of type " + ParentState.GetType().FullName + "!");


            int id = 0;
            while (id == 0 || _entities.ContainsKey(id))
                id = Rng.Next();

            T behavior = new T();

            GameObject entity = new GameObject(this, new GameObjectPtr(this, id, name), behavior);

            _entities[id] = entity;
            _namePtrs[name] = id;

            behavior.Initialize(entity);

            entity.SetEnabled(true);

            return entity;
        }


        // Lifetime func
        private void OnUpdate(object sender, FrameEventArgs e)
        {
            foreach (GameObject entity in _entities.Values)
            {
                if (entity.Enabled)
                    entity.OnUpdate(this, e);
            }
        }
        private void OnLateUpdate(object sender, FrameEventArgs e)
        {
            foreach (GameObject entity in _entities.Values)
            {
                if (entity.Enabled)
                    entity.OnLateUpdate(this, e);

                if (entity.RequestingStateChange)
                    entity.UpdateState();
            }
        }
        private void OnRender(object sender, FrameEventArgs e)
        {
            foreach (GameObject entity in _entities.Values)
            {
                if (entity.Enabled)
                    entity.OnRender(this, e);
            }
        }
    }
}
