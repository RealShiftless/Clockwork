using Clockwork.Common.GameObjects;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Common.Mathematics
{
    public class Transform
    {
        // Values
        public Vector3 Position = Vector3.Zero;
        public Quaternion Rotation = Quaternion.Identity;
        public Vector3 Scale = Vector3.One;

        private GameObject _entity;


        // Properties
        public GameObject GameObject => _entity;


        // Constructor
        internal Transform(GameObject entity)
        {
            _entity = entity;
        }


        // Func
        public Matrix4 GetMatrix()
        {
            Matrix4 transform = Matrix4.Identity;
            transform *= Matrix4.CreateScale(Scale);
            transform *= Matrix4.CreateFromQuaternion(Rotation);
            transform *= Matrix4.CreateTranslation(Position);

            return transform;
        }
    }
}
