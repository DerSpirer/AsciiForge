using AsciiForge.Engine;

namespace AsciiForge.Components
{
    public class Transform : Component
    {
        public Vector3 position { get; set; } = Vector3.zero;
        public Vector3 rotation { get; set; } = Vector3.zero; // Don't know what to really do with it now. Ain't that important.
        public Vector3 scale { get; set; } = Vector3.zero; // Don't know what to really do with it now. Ain't that important.
    }
}
