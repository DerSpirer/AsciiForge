using AsciiForge.Engine;
using AsciiForge.Resources;
using System.Text.Json.Serialization;
using static AsciiForge.Engine.Canvas;

namespace AsciiForge.Components.Sprites
{
    public abstract class Drawable : Component
    {
        private TextureResource? _texture;
        [JsonIgnore]
        public TextureResource? texture
        {
            get
            {
                return _texture;
            }
            protected set
            {
                if (value != _texture)
                {
                    if (value != null)
                    {
                        _texture = value;
                        width = value.width;
                        height = value.height;
                    }
                    else
                    {
                        _texture = null;
                        width = 0;
                        height = 0;
                    }
                    anchor = _anchor;
                }
            }
        }
        [JsonIgnore]
        public int width { get; private set; }
        [JsonIgnore]
        public int height { get; private set; }
        public bool isVisible { get; set; } = true;
        public BlendMode blendMode { get; set; } = BlendMode.Alpha;
        private Vector2 _offset;
        public Vector2 offset
        {
            get
            {
                return _offset;
            }
            set
            {
                _offset = value;
                _anchor = Anchor.Undefined;
            }
        }
        private Anchor _anchor;
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Anchor anchor
        {
            get
            {
                return _anchor;
            }
            set
            {
                switch (value)
                {
                    case Anchor.Undefined:
                        break;
                    case Anchor.TopLeft:
                        offset = new Vector2(0, 0);
                        break;
                    case Anchor.TopCenter:
                        offset = new Vector2(-(width / 2), 0);
                        break;
                    case Anchor.TopRight:
                        offset = new Vector2(-width, 0);
                        break;
                    case Anchor.MiddleLeft:
                        offset = new Vector2(0, -(height / 2));
                        break;
                    case Anchor.MiddleCenter:
                        offset = new Vector2(-(width / 2), -(height / 2));
                        break;
                    case Anchor.MiddleRight:
                        offset = new Vector2(-width, -(height / 2));
                        break;
                    case Anchor.BottomLeft:
                        offset = new Vector2(0, -height);
                        break;
                    case Anchor.BottomCenter:
                        offset = new Vector2(-(width / 2), -height);
                        break;
                    case Anchor.BottomRight:
                        offset = new Vector2(-width, -height);
                        break;
                    default:
                        Logger.Critical("Unknown texture anchor type");
                        throw new Exception("Unknown texture anchor type");
                }
                _anchor = value;
            }
        }
        public bool flipHorizontal { get; set; }
        public bool flipVertical { get; set; }

        protected void Draw(Canvas canvas)
        {
            if (_texture != null && isVisible)
            {
                TextureResource drawTexture = _texture;
                if (flipHorizontal)
                {
                    drawTexture = drawTexture.flippedHorizontal;
                }
                if (flipVertical)
                {
                    drawTexture = drawTexture.flippedVertical;
                }
                canvas.Draw(drawTexture, transform.position + new Vector3(offset));
            }
        }

        public enum Anchor
        {
            Undefined,
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }
    }
}
