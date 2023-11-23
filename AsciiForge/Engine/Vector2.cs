using System.Text.Json.Serialization;

namespace AsciiForge.Engine
{
    public class Vector2
    {
        private System.Numerics.Vector2 _vector;

        public float x { get { return _vector.X; } set { _vector.X = value; } }
        public float y { get { return _vector.Y; } set { _vector.Y = value; } }

        /// <summary>
        /// Shorthand for (0, 0)
        /// </summary>
        public static Vector2 zero { get { return new Vector2(0, 0); } }
        /// <summary>
        /// Shorthand for (1, 1)
        /// </summary>
        public static Vector2 one { get { return new Vector2(1, 1); } }
        [JsonIgnore]
        public float length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y);
            }
            set
            {
                Normalize();
                x *= value;
                y *= value;
            }
        }
        [JsonIgnore]
        public Vector2 normalized
        {
            get
            {
                float len = length;
                if (len == 0)
                {
                    return zero;
                }
                return new Vector2(x / len, y / len);
            }
        }

        [JsonConstructor]
        public Vector2(float x, float y)
        {
            _vector = new System.Numerics.Vector2(x, y);
        }

        public override string ToString()
        {
            return $"({x:0.###},{y:0.###})";
        }
        public void Normalize()
        {
            float len = length;
            if (len == 0)
            {
                return;
            }
            x /= len;
            y /= len;
        }

        public static Vector2 operator +(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x + v2.x, v1.y + v2.y);
        }
        public static Vector2 operator -(Vector2 v1, Vector2 v2)
        {
            return new Vector2(v1.x - v2.x, v1.y - v2.y);
        }
    }
}
