using Clockwork.Common.Mathematics;
using Clockwork.Rendering;
using Clockwork.ResourcesDeprecated;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public ResourceManager Resources
        {
            get
            {
                if (GameObject.Game == null)
                    throw new InvalidOperationException("The GameState that the entity was bound to was not bound to a game when getting ResourceManager!");

                return GameObject.Game.Resources;
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
