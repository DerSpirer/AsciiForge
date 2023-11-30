using System.Drawing;
using System.Text.Json;
using System.Text.Json.Serialization;
using AsciiForge.Helpers.JsonConverters;

namespace AsciiForge.Engine.Resources
{
    public class SpriteResource : Resource
    {
        private readonly bool _isPlaying;
        public bool isPlaying { get { return _isPlaying; } }
        private readonly float _clipLength;
        public float clipLength { get { return _clipLength; } }
        private readonly int _startFrame;
        public int startFrame { get { return _startFrame; } }
        private readonly TextureResource[] _textures;
        public TextureResource[] textures { get { return _textures; } }
        [JsonIgnore]
        public int width
        {
            get
            {
                return textures[0].width;
            }
        }
        [JsonIgnore]
        public int height
        {
            get
            {
                return textures[0].height;
            }
        }

        [JsonConstructor]
        private SpriteResource(bool isPlaying, float clipLength, int startFrame, TextureResource[] textures)
        {
            _isPlaying = isPlaying;
            _clipLength = clipLength;
            _startFrame = startFrame;
            _textures = textures;

            (bool isValid, string error) = IsValid();
            if (!isValid)
            {
                throw new ResourceFormatException(error);
            }
        }

        protected override (bool, string) IsValid()
        {
            bool isValid = false;
            string error = string.Empty;

            if (_textures == null || _textures.Length <= 0)
            {
                error = "Failed to load sprite resource with no textures";
                return (isValid, error);
            }
            for (int i = 0; i < _textures.Length; i++)
            {
                if (_textures[i] == null)
                {
                    error = $"Failed to load sprite resource with a null texture at index {i}";
                    return (isValid, error);
                }
                if (i > 0 && (_textures[i].width != _textures[0].width || _textures[i].height != _textures[0].height))
                {
                    error = $"Failed to load sprite resource with a different sized texture at index {i}";
                    return (isValid, error);
                }
            }
            if (_clipLength <= 0)
            {
                error = $"Failed to load sprite resource with negative clip length";
                return (isValid, error);
            }
            if (_startFrame < 0 || _startFrame >= _textures.Length)
            {
                error = $"Failed to load sprite resource with start frame not between 0 and the amount of textures";
                return (isValid, error);
            }

            isValid = true;
            return (isValid, error);
        }

        public static async Task<SpriteResource> Read(ResourceManager.ResourceFile resourceFile)
        {
            await using FileStream fileStream = File.Open(resourceFile.path, FileMode.Open, FileAccess.Read);
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                AllowTrailingCommas = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
            };
            options.Converters.Add(new JsonColorConverter());
            options.Converters.Add(new JsonRectangularArrayConverter<char>());
            options.Converters.Add(new JsonRectangularArrayConverter<Color>());
            SpriteResource resource = await JsonSerializer.DeserializeAsync<SpriteResource>(fileStream, options) ?? throw new ResourceFormatException($"Failed to deserialize sprite resource at: {resourceFile.path}");
            return resource;
        }
    }
}
