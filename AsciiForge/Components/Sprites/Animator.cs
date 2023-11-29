using AsciiForge.Engine.Ecs;
using AsciiForge.Engine.IO;
using AsciiForge.Engine.Resources;

namespace AsciiForge.Components.Sprites
{
    public class Animator : Component
    {
        private Sprite _sprite;
        private string[] _sprites;
        public string[] sprites
        {
            get
            {
                return _sprites;
            }
            set
            {
                if (value == null || value.Length == 0)
                {
                    Logger.Error("Trying to set animator's sprites array to an empty array");
                    throw new Exception("Trying to set animator's sprites array to an empty array");
                }
                for (int i = 0; i < value.Length; i++)
                {
                    if (!ResourceManager.sprites.ContainsKey(value[i]))
                    {
                        Logger.Error($"Trying to set animator's sprites array with reference to a sprite resource that does not exist: {value[i]}");
                        throw new Exception($"Trying to set animator's sprites array with reference to a sprite resource that does not exist: {value[i]}");
                    }
                    if (i > 0 && (ResourceManager.sprites[value[i]].width != ResourceManager.sprites[value[0]].width || ResourceManager.sprites[value[i]].height != ResourceManager.sprites[value[0]].height))
                    {
                        Logger.Error($"Trying to set animator's sprites array with reference to a differently sized sprite resource: {value[i]}");
                        throw new Exception($"Trying to set animator's sprites array with reference to a differently sized sprite resource: {value[i]}");
                    }
                }
                _sprites = value;
            }
        }
        private int _currSprite;
        public int currSprite
        {
            get
            {
                return _currSprite;
            }
            set
            {
                if (_currSprite != value)
                {
                    _sprite.sprite = sprites[value];
                    _currSprite = value;
                }
            }
        }

        private void Start()
        {
            _sprite = entity.FindComponent<Sprite>()!;
        }
        public void Next()
        {
            currSprite = (_currSprite + 1) % _sprites.Length;
        }
        public void Set(string sprite)
        {
            if (string.IsNullOrEmpty(sprite))
            {
                Logger.Warning("Trying to set current animator sprite to an invalid sprite");
                return;
            }
            if (!_sprites.Contains(sprite))
            {
                Logger.Warning("Trying to set current animator sprite to a sprite its array does not contain");
                return;
            }
            currSprite = _sprites.ToList().IndexOf(sprite);
        }
    }
}
