using AsciiForge.Engine.IO;
using AsciiForge.Engine.Resources;
using System.Text.Json.Serialization;

namespace AsciiForge.Components.Sprites
{
    public class Sprite : Drawable
    {
        private string? _sprite;
        public string? sprite
        {
            get
            {
                return _sprite;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _sprite = value;
                    isPlaying = false;
                    _clipLength = 0;
                    spriteIndex = 0;
                    return;
                }
                if (!ResourceManager.sprites.ContainsKey(value))
                {
                    Logger.Error($"Trying to set a sprite to a resource that does not exist: {value}");
                    throw new Exception($"Trying to set a sprite to a resource that does not exist: {value}");
                }
                _sprite = value;
                isPlaying = resource!.isPlaying;
                _clipLength = resource.clipLength;
                spriteIndex = resource.startFrame;
            }
        }
        [JsonIgnore]
        public SpriteResource? resource
        {
            get
            {
                if (string.IsNullOrEmpty(_sprite))
                {
                    return null;
                }
                return ResourceManager.sprites[_sprite];
            }
        }
        public bool isPlaying { get; set; }
        private float _clipLength;
        public float clipLength
        {
            get
            {
                return _clipLength;
            }
            set
            {
                if (resource == null)
                {
                    return;
                }
                if (value < 0)
                {
                    Logger.Error("Sprite clip length must not be negative");
                    throw new Exception("Sprite clip length must not be negative");
                }
                if (_clipLength != value)
                {
                    _clipLength = value;
                    spriteIndex = 0;
                }
            }
        }
        private float _clipTime;
        public float clipTime
        {
            get
            {
                return _clipTime;
            }
            set
            {
                if (resource == null)
                {
                    return;
                }
                if (value < 0 || value >= clipLength)
                {
                    Logger.Error("Sprite clip time must be between 0 and clip length");
                    throw new Exception("Sprite clip time must be between 0 and clip length");
                }
                _clipTime = value;
            }
        }
        [JsonIgnore]
        public int spriteIndex
        {
            get
            {
                return (int)(_clipTime / _clipLength * spriteLength);
            }
            set
            {
                if (resource == null)
                {
                    return;
                }
                if (spriteIndex < 0 || spriteIndex >= spriteLength)
                {
                    Logger.Error("Sprite index must be between 0 and sprite length");
                    throw new Exception("Sprite index must be between 0 and sprite length");
                }
                _clipTime = ((float)value) / spriteLength * _clipLength;
                if (resource != null)
                {
                    texture = resource.textures[value];
                }
            }
        }
        [JsonIgnore]
        public int spriteLength
        {
            get
            {
                if (resource != null)
                {
                    return resource.textures.Length;
                }
                return 0;
            }
        }
        [JsonIgnore]
        public TextureResource? currTexture
        {
            get
            {
                if (resource != null)
                {
                    return resource.textures[spriteIndex];
                }
                return null;
            }
        }

        private void Update(float deltaTime)
        {
            if (isPlaying)
            {
                int prevFrame = spriteIndex;
                _clipTime = (_clipTime + deltaTime) % _clipLength;
                int currFrame = spriteIndex;
                if (currFrame != prevFrame)
                {
                    texture = currTexture;
                }
            }
        }

        public void ResetClipLength()
        {
            clipLength = resource.clipLength;
        }
    }
}
