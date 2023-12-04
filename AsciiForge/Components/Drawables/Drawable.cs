using System.Text.Json.Serialization;
using AsciiForge.Engine;
using AsciiForge.Engine.IO;
using AsciiForge.Engine.Resources;
using static AsciiForge.Engine.Canvas;

namespace AsciiForge.Components.Drawables
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
        private Vector2 _offset = Vector2.zero;
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
                        _offset = new Vector2(0, 0);
                        break;
                    case Anchor.TopCenter:
                        _offset = new Vector2(-(width / 2f), 0);
                        break;
                    case Anchor.TopRight:
                        _offset = new Vector2(-width, 0);
                        break;
                    case Anchor.MiddleLeft:
                        _offset = new Vector2(0, -(height / 2f));
                        break;
                    case Anchor.MiddleCenter:
                        _offset = new Vector2(-(width / 2f), -(height / 2f));
                        break;
                    case Anchor.MiddleRight:
                        _offset = new Vector2(-width, -(height / 2f));
                        break;
                    case Anchor.BottomLeft:
                        _offset = new Vector2(0, -height);
                        break;
                    case Anchor.BottomCenter:
                        _offset = new Vector2(-(width / 2f), -height);
                        break;
                    case Anchor.BottomRight:
                        _offset = new Vector2(-width, -height);
                        break;
                    default:
                        Logger.Critical("Unknown texture anchor type");
                        throw new Exception("Unknown texture anchor type");
                }
                _anchor = value;
            }
        }
        public bool isGui { get; set; } = false;
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
                if (isGui)
                {
                    canvas.DrawGui(drawTexture, transform.position + new Vector3(_offset));
                }
                else
                {
                    canvas.Draw(drawTexture, transform.position + new Vector3(_offset));
                }
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
