using System.Text.Json.Serialization;

namespace AsciiForge.Engine
{
    public class Vector3
    {
        private System.Numerics.Vector3 _vector;

        public float x { get { return _vector.X; } set { _vector.X = value; } }
        public float y { get { return _vector.Y; } set { _vector.Y = value; } }
        public float z { get { return _vector.Z; } set { _vector.Z = value; } }

        /// <summary>
        /// Shorthand for (0, 0, 0)
        /// </summary>
        public static Vector3 zero { get { return new Vector3(0, 0, 0); } }
        /// <summary>
        /// Shorthand for (1, 1, 1)
        /// </summary>
        public static Vector3 one { get { return new Vector3(1, 1, 1); } }
        [JsonIgnore]
        public float length
        {
            get
            {
                return (float)Math.Sqrt(x * x + y * y + z * z);
            }
            set
            {
                Normalize();
                x *= value;
                y *= value;
                z *= value;
            }
        }
        [JsonIgnore]
        public Vector3 normalized
        {
            get
            {
                float len = length;
                if (len == 0)
                {
                    return zero;
                }
                return new Vector3(x / len, y / len, z / len);
            }
        }

        public Vector3(float x, float y, float z)
        {
            _vector = new System.Numerics.Vector3(x, y, z);
        }

        public override string ToString()
        {
            return $"({x:0.###},{y:0.###},{z:0.###})";
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
            z /= len;
        }
    }
}
