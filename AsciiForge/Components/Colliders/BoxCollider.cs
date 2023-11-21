using AsciiForge.Engine;
using System.Text.Json.Serialization;

namespace AsciiForge.Components.Colliders
{
    public class BoxCollider : Component, ICollider
    {
        public Vector2 size { get; set; } = Vector2.zero;
        public Vector2 offset { get; set; } = Vector2.zero;
        [JsonIgnore]
        public float left { get { return transform.position.x - size.x / 2 + offset.x; } }
        [JsonIgnore]
        public float right { get { return transform.position.x + size.x / 2 + offset.x; } }
        [JsonIgnore]
        public float top { get { return transform.position.y - size.y / 2 + offset.y; } }
        [JsonIgnore]
        public float bottom { get { return transform.position.y + size.y / 2 + offset.y; } }

        public bool PointMeeting(ICollider other, Vector2 position)
        {
            switch (other)
            {
                case BoxCollider b:
                    return !(position.x - size.x / 2 + offset.x > b.right ||
                             position.x + size.x / 2 + offset.x < b.left ||
                             position.y + size.y / 2 + offset.y < b.top ||
                             position.y - size.y / 2 + offset.y > b.bottom);
                case null:
                    Logger.Error($"Unabled to collide with null");
                    return false;
                default:
                    Logger.Error($"Unable to collide other({other.GetType().FullName}) with this({GetType().FullName})");
                    return false;
            }
        }
    }
}
