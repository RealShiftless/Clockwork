using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Rendering
{
    public class Camera
    {
        // Values
        public readonly GameState ParentState;

        private Vector3 _position;

        private Matrix4 _viewMatrix;
        private Matrix4 _projectionMatrix;

        private Vector3 _front = -Vector3.UnitZ;
        private Vector3 _up = Vector3.UnitY;
        private Vector3 _right = Vector3.UnitX;

        private float _pitch;
        private float _yaw = -MathHelper.PiOver2;

        private float _fov = MathHelper.PiOver2;

        private bool _orthographic = false;


        // Properties
        public Matrix4 ViewMatrix => _viewMatrix;
        public Matrix4 ProjectionMatrix => _projectionMatrix;

        public Vector3 Front => _front;
        public Vector3 Up => _up;
        public Vector3 Right => _right;

        public Vector3 Forward => new Vector3(_front.X, 0, _front.Z).Normalized();

        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                UpdateMatrices();
            }
        }

        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 90f);
                _fov = MathHelper.DegreesToRadians(angle);
                UpdateMatrices();
            }
        }

        public float AspectRatio
        {
            get
            {
                if (ParentState.Game == null)
                    throw new InvalidOperationException("Camera was not attached to a game before AspectRatio was called!");

                return ParentState.Game.Renderer.AspectRatio;
            }
        }

        public bool Orthographic
        {
            get => _orthographic;
            set
            {
                if (value == Orthographic)
                    return;

                _orthographic = value;
                UpdateMatrices();
            }
        }


        // Constructor
        internal Camera(GameState state)
        {
            ParentState = state;

            _position = Vector3.UnitZ * 10;

            UpdateVectors();
        }


        // Func
        private void UpdateVectors()
        {
            _front.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            _front.Y = MathF.Sin(_pitch);
            _front.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            _front = Vector3.Normalize(_front);

            _right = Vector3.Normalize(Vector3.Cross(_front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, _front));

            UpdateMatrices();
        }
        private void UpdateMatrices()
        {
            _viewMatrix = Matrix4.LookAt(_position, _position + _front, _up);
            _projectionMatrix = !_orthographic ? Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f) : Matrix4.CreateOrthographic(ParentState.Game.Renderer.Size.X, ParentState.Game.Renderer.Size.Y, 0.1f, 100f);
        }


        /*
        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix()
        {
            return Matrix4.LookAt(Position, Position + _front, _up);
        }

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix()
        {
            Matrix4.CreateOrthographic(ParentState.Game.Renderer.Size.X, ParentState.Game.Renderer.Size.Y, 0.1f, 100f);
            return Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.01f, 100f);
        }
        */
    }
}
