using AsciiForge.Engine;
using AsciiForge.Engine.IO;
using System.Drawing;
using System.Text.Json.Serialization;
using AsciiForge.Components.Drawables;

namespace AsciiForge.Components
{
    public class Camera : Component
    {
        public Color bgColor { get; set; } = Color.Black;
        public Vector2 bgOffset { get; set; } = Vector2.zero;
        public BgMode bgMode { get; set; } = BgMode.Single;
        public Mode mode { get; set; } = Mode.Static;
        public Transform? target { get; set; } = null;
        public Vector3 targetOffset { get; set; } = new Vector3(0, 0, -10);

        [JsonIgnore]
        public Sprite sprite
        {
            get
            {
                return entity.FindComponent<Sprite>()!;
            }
        }

        public enum Mode
        {
            Static,
            FollowImmediate,
        }

        internal void DrawBg(Canvas canvas)
        {
            if (sprite.texture == null)
            {
                return;
            }
            switch (bgMode)
            {
                case BgMode.Single:
                    canvas.Draw(sprite.texture, new Vector3(bgOffset.x, bgOffset.y, transform.position.z + 1));
                    break;
                case BgMode.Multiply:
                    int offsetX = (int)Math.Round(transform.position.x % sprite.width);
                    int offsetY = (int)Math.Round(transform.position.y % sprite.height);
                    for (int i = 0; i < Math.Ceiling(((float)Screen.width) / sprite.width) + 1; i++)
                    {
                        for (int j = 0; j < Math.Ceiling((float)Screen.height / sprite.height) + 1; j++)
                        {
                            canvas.DrawGui(sprite.texture, new Vector3(i * sprite.width - offsetX, j * sprite.height - offsetY, transform.position.z + 1));
                        }
                    }
                    break;
            }
        }

        private void Update(float deltaTime)
        {
            switch (mode)
            {
                case Mode.FollowImmediate:
                    if (target != null)
                    {
                        transform.position = target.position + targetOffset;
                    }
                    break;
            }
        }

        public enum BgMode
        {
            Single,
            Multiply,
        }
    }
}
