using AsciiForge.Components;
using System.Text.Json.Serialization;

namespace AsciiForge.Engine.Ecs
{
    public abstract class Component
    {
        public bool isEnabled { get; set; } = true;

        internal Entity _entity;
        [JsonIgnore]
        public Entity entity
        {
            get
            {
                return _entity;
            }
        }
        [JsonIgnore]
        public Transform transform
        {
            get
            {
                return entity.transform;
            }
        }
    }
}
