using Newtonsoft.Json;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Clockwork.Mathematics
{
    /// <summary>
    /// 2D Representation of a box.
    /// Copied and reformatted from the OpenTK Github.
    /// </summary>
    public struct Box2
    {
        // Static Values
        [JsonIgnore]
        public static readonly Box2 Empty = new Box2(0,0,0,0);


        // Values
        [JsonProperty] private Vector2 _min;
        [JsonProperty] private Vector2 _max;


        // Properties
        [JsonIgnore]
        public Vector2 Min
        {
            get => _min;
            set
            {
                _max = Vector2i.ComponentMax(_max, value);
                _min = value;
            }
        }
        [JsonIgnore]
        public Vector2 Max
        {
            get => _max;
            set
            {
                _min = Vector2i.ComponentMin(_min, value);
                _max = value;
            }
        }


        [JsonIgnore]
        public Vector2 Size => _max - _min;

        [JsonIgnore]
        public Vector2 Center => _min + ((_max - _min) * 0.5f);



        // Constructors
        public Box2(Vector2 min, Vector2 max)
        {
            _min = min; 
            _max = max;
        }
        public Box2(float minX, float minY, float maxX, float maxY) : this(new Vector2(minX, minY), new Vector2(maxX, maxY)) { }


        // Func
        public bool Contains(Vector2 point)
        {
            return _min.X <= point.X && point.X <= _max.X &&
                   _min.Y <= point.Y && point.Y <= _max.Y;
        }
        public bool ContainsExclusive(Vector2 point)
        {
            return _min.X < point.X && point.X < _max.X &&
                   _min.Y < point.Y && point.Y < _max.Y;
        }

        public bool Contains(Box2 other)
        {
            return _max.X >= other._min.X && _min.X <= other._max.X &&
                   _max.Y >= other._min.Y && _min.Y <= other._max.Y;
        }

        public static Box2 Intersect(Box2 a, Box2 b)
        {
            Vector2 min = Vector2.ComponentMax(a.Min, b.Min);
            Vector2 max = Vector2.ComponentMin(a.Max, b.Max);

            if (max.X >= min.X && max.Y >= min.Y)
            {
                return new Box2(min, max);
            }
            else
            {
                return Empty;
            }
        }

        public float DistanceToNearestEdge(Vector2 point)
        {
            Vector2 dist = new Vector2(
                Math.Max(0f, Math.Max(_min.X - point.X, point.X - _max.X)),
                Math.Max(0f, Math.Max(_min.Y - point.Y, point.Y - _max.Y)));
            return dist.Length;
        }

        public void Translate(Vector2 distance)
        {
            _min += distance;
            _max += distance;
        }
        public Box2 Translated(Vector2 distance)
        {
            // create a local copy of this box
            Box2 box = this;
            box.Translate(distance);
            return box;
        }

        public void Scale(Vector2 scale, Vector2 anchor)
        {
            _min = anchor + ((_min - anchor) * scale);
            _max = anchor + ((_max - anchor) * scale);
        }
        public Box2 Scaled(Vector2 scale, Vector2 anchor)
        {
            // create a local copy of this box
            Box2 box = this;
            box.Scale(scale, anchor);
            return box;
        }

        public void Inflate(Vector2 point)
        {
            _min = Vector2.ComponentMin(_min, point);
            _max = Vector2.ComponentMax(_max, point);
        }
        public Box2 Inflated(Vector2 point)
        {
            // create a local copy of this box
            Box2 box = this;
            box.Inflate(point);
            return box;
        }

        public bool Equals(Box2 other)
        {
            return _min.Equals(other._min) &&
                   _max.Equals(other._max);
        }

        // Overrides
        public override bool Equals(object? obj)
        {
            return obj is Box2i && Equals((Box2i)obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(_min, _max);
        }


        // Operators
        public static bool operator ==(Box2 left, Box2 right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Box2 left, Box2 right)
        {
            return !(left == right);
        }


        // Casting Operators
        public static explicit operator System.Drawing.Rectangle(Box2 box)
        {
            return new System.Drawing.Rectangle((int)box.Min.X, (int)box.Min.Y, (int)box.Size.X, (int)box.Size.Y);
        }

    }
}
