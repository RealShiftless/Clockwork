using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.GameObjects
{
    public abstract class BaseComponent
    {
        // Values
        public GameObject GameObject { get; private set; }

        private bool _enabled;


        // Properties
        public bool Enabled
        {
            get
            {
                if (!IsBound)
                    throw new InvalidOperationException("Cannot enable component which is not bound!");

                return _enabled;
            }
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke(this);
                }
            }
        }

        public Game Game
        {
            get
            {
                if (GameObject.Game == null)
                    throw new InvalidOperationException("GameObject was not bound to Game when Game was called!");

                return GameObject.Game;
            }
        }

        public bool IsBound => GameObject != null;


        // Events
        public event Action<BaseComponent> Bound;
        public event Action<BaseComponent> EnabledChanged;


        // Func
        internal void Bind(GameObject gameObject)
        {
            GameObject = gameObject;
            Enabled = true;

            Bound?.Invoke(this);
        }
    }
}
