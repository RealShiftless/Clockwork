using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Mathematics
{
    public struct Vector2
    {
        // Default values
        private static readonly Vector2 _zero = new Vector2(0, 0);
        private static readonly Vector2 _one = new Vector2(1, 1);
        private static readonly Vector2 _up = new Vector2(0, 1);
        private static readonly Vector2 _down = new Vector2(0, -1);
        private static readonly Vector2 _left = new Vector2(-1, 0);
        private static readonly Vector2 _right = new Vector2(1, 0);


        // Values
        public float X;
        public float Y;


        // Default Properties
        public static Vector2 Zero => _zero;
        public static Vector2 One => _one;
        public static Vector2 Up => _up;
        public static Vector2 Down => _down;
        public static Vector2 Left => _left;
        public static Vector2 Right => _right;


        // Properties
        public float LengthSquared => (X * X) + (Y * Y);
        public float Length => MathF.Sqrt(LengthSquared);


        // Constructor
        /// <summary>
        /// Creates a 2D Vector with the given values.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        public Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }
        /// <summary>
        /// Creates a 2D Vector with the same values.
        /// </summary>
        /// <param name="value">The x and y coordinates.</param>
        public Vector2(float value)
        {
            X = value;
            Y = value;
        }


        // Func
        public void Round()
        {
            X = MathF.Round(X);
            Y = MathF.Round(Y);
        }
        public void Ceiling()
        {
            X = MathF.Ceiling(X);
            Y = MathF.Ceiling(Y);
        }
        public void Floor()
        {
            X = MathF.Floor(X);
            Y = MathF.Floor(Y);
        }
        public bool Equals(Vector2 other)
        {
            return X == other.X && Y == other.Y;
        }


        // Overrides
        public override bool Equals(object? obj)
        {
            if (obj is Vector2)
            {
                return Equals((Vector2)obj);
            }

            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return X.GetHashCode() * 397 ^ Y.GetHashCode();
            }
        }
        public override string? ToString()
        {
            return "{ X: " + X + " Y: " + Y + " }";
        }


        // Static Func
        public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
        {
            return new Vector2(
                MathHelper.Clamp(value1.X, min.X, max.X),
                MathHelper.Clamp(value1.Y, min.Y, max.Y));
        }
        public static float Distance(Vector2 value1, Vector2 value2)
        {
            float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            return MathF.Sqrt(v1 * v1 + v2 * v2);
        }
        public static float DistanceSquared(Vector2 value1, Vector2 value2)
        {
            float v1 = value1.X - value2.X, v2 = value1.Y - value2.Y;
            return v1 * v1 + v2 * v2;
        }
        public static float Dot(Vector2 value1, Vector2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }
        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            return new Vector2(
                MathHelper.Lerp(value1.X, value2.X, amount),
                MathHelper.Lerp(value1.Y, value2.Y, amount));
        }

        public static Vector2 ComponentMin(Vector2 a, Vector2 b)
        {
            return new Vector2(
                    a.X < b.X ? a.X : b.X,
                    a.Y < b.Y ? a.Y : b.Y
                );
        }
        public static Vector2 ComponentMax(Vector2 a, Vector2 b)
        {
            return new Vector2(
                    a.X > b.X ? a.X : b.X,
                    a.Y > b.Y ? a.Y : b.Y
                );
        }


        // Operators
        public static Vector2 operator -(Vector2 value)
        {
            value.X = -value.X;
            value.Y = -value.Y;
            return value;
        }
        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            value1.X += value2.X;
            value1.Y += value2.Y;
            return value1;
        }
        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            value1.X -= value2.X;
            value1.Y -= value2.Y;
            return value1;
        }
        public static Vector2 operator *(Vector2 value1, Vector2 value2)
        {
            value1.X *= value2.X;
            value1.Y *= value2.Y;
            return value1;
        }
        public static Vector2 operator *(Vector2 value, float scaleFactor)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }
        public static Vector2 operator *(float scaleFactor, Vector2 value)
        {
            value.X *= scaleFactor;
            value.Y *= scaleFactor;
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 value1, Vector2 value2)
        {
            value1.X /= value2.X;
            value1.Y /= value2.Y;
            return value1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 operator /(Vector2 value1, float divider)
        {
            float factor = 1 / divider;
            value1.X *= factor;
            value1.Y *= factor;
            return value1;
        }

        public static bool operator ==(Vector2 value1, Vector2 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }
        public static bool operator !=(Vector2 value1, Vector2 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }


        // Cast Operators
        public static implicit operator System.Numerics.Vector2(Vector2 value)
        {
            return new System.Numerics.Vector2(value.X, value.Y);
        }
        public static implicit operator Vector2i(Vector2 value)
        {
            return new Vector2i((int)value.X, (int)value.Y);
        }
        public static implicit operator Vector2d(Vector2 value)
        {
            return new Vector2d(value.X, value.Y);
        }

        public static implicit operator Vector2(System.Numerics.Vector2 value)
        {
            return new Vector2(value.X, value.Y);
        }
        public static implicit operator Vector2(Vector2i value)
        {
            return new Vector2(value.X, value.Y);
        }
        public static implicit operator Vector2(Vector2d value)
        {
            return new Vector2((float)value.X, (float)value.Y);
        }
    }
}
