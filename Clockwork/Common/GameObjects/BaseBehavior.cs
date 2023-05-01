using Clockwork.Common.Resources;
using Clockwork.Mathematics;

namespace Clockwork.Common.GameObjects
{
    public abstract class BaseBehavior
    {
        // Reference Values
        public GameObject GameObject { get; private set; }


        // Properties
        public Transform Transform => GameObject.Transform;

        public Game Game
        {
            get
            {
                if (GameObject.Game == null)
                    throw new Exception("Game Object was not bound when Game was called!");

                return GameObject.Game;
            }
        }

        public ResourceLibrary Resources
        {
            get
            {
                return GameObject.ParentState.Resources;
            }
        }


        // Func
        internal void Initialize(GameObject entity)
        {
            GameObject = entity;

            Initialize();
        }


        // Virtuals
        protected virtual void Initialize() { }
    }
}
