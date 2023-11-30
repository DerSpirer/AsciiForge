using AsciiForge.Engine.IO;
using System.Drawing;
using System.Text.Json;
using AsciiForge.Helpers.JsonConverters;

namespace AsciiForge.Engine.Resources
{
    public static class ResourceManager
    {
        public static readonly Dictionary<string, SpriteResource> sprites = new Dictionary<string, SpriteResource>();
        public static readonly Dictionary<string, SoundResource> sounds = new Dictionary<string, SoundResource>();
        public static readonly Dictionary<string, EntityResource> entities = new Dictionary<string, EntityResource>();
        private static readonly Dictionary<string, RoomResource> _rooms = new Dictionary<string, RoomResource>();
        public static readonly List<RoomResource> rooms = new List<RoomResource>();

        private struct ResourceFile
        {
            public string path;
            public ResourceType type;
            public string name;

            public enum ResourceType
            {
                Sprite,
                Sound,
                Entity,
                Room,
            }

            public ResourceFile(string path)
            {
                if (string.IsNullOrEmpty(path))
                {
                    Logger.Error($"Invalid resource file path: {path}");
                    throw new Exception($"Invalid resource file path: {path}");
                }
                if (!_typesSuffixes.Any(t => path.EndsWith(t.Item2)))
                {
                    Logger.Error($"Unknown resource type: {path}");
                    throw new Exception($"Unknown resource type: {path}");
                }

                (ResourceType type, string suffix) = _typesSuffixes.ToList().Find(s => path.EndsWith(s.Item2));
                this.path = path;
                this.type = type;
                int nameStart = path.LastIndexOf('\\');
                nameStart = nameStart < 0 ? 0 : nameStart + 1;
                name = path[nameStart..^suffix.Length];
            }

            public static readonly (ResourceType, string)[] _typesSuffixes = new (ResourceType, string)[] { (ResourceType.Sprite, ".sprite.json"), (ResourceType.Entity, ".entity.json"), (ResourceType.Room, ".room.json"), (ResourceType.Sound, ".sound.mp3"), (ResourceType.Sound, ".sound.wav"), };
        }

        internal static async Task Load()
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            await LoadResources(Directory.EnumerateFiles(directory, "*.sprite.json", SearchOption.AllDirectories).Select(f => new ResourceFile(f)).ToArray());
            await LoadResources(Directory.EnumerateFiles(directory, "*.sound.*", SearchOption.AllDirectories).Where(f => ResourceFile._typesSuffixes.Any(s => f.EndsWith(s.Item2))).Select(f => new ResourceFile(f)).ToArray());
            await LoadResources(Directory.EnumerateFiles(directory, "*.entity.json", SearchOption.AllDirectories).Select(f => new ResourceFile(f)).ToArray());
            await LoadResources(Directory.EnumerateFiles(directory, "*.room.json", SearchOption.AllDirectories).Select(f => new ResourceFile(f)).ToArray());

            await GlobalDefinitions.Load();
            rooms.AddRange(GlobalDefinitions.OrderRooms(_rooms));
        }
        private static async Task LoadResources(ResourceFile[] resources)
        {
            foreach (ResourceFile r in resources)
            {
                try
                {
                    using FileStream fileStream = File.Open(r.path, FileMode.Open, FileAccess.Read);
                    JsonSerializerOptions options = new JsonSerializerOptions
                    {
                        AllowTrailingCommas = true,
                        ReadCommentHandling = JsonCommentHandling.Skip,
                    };
                    switch (r.type)
                    {
                        case ResourceFile.ResourceType.Sprite:
                            if (sprites.ContainsKey(r.name))
                            {
                                continue;
                            }
                            options.Converters.Add(new JsonColorConverter());
                            options.Converters.Add(new JsonRectangularArrayConverter<char>());
                            options.Converters.Add(new JsonRectangularArrayConverter<Color>());
                            sprites.Add(r.name, await JsonSerializer.DeserializeAsync<SpriteResource>(fileStream, options) ?? throw new Exception("Failed to deserialize sprite resource file"));
                            break;
                        case ResourceFile.ResourceType.Sound:
                            if (sounds.ContainsKey(r.name))
                            {
                                continue;
                            }
                            sounds.Add(r.name, await SoundResource.ReadFile(r.path));
                            break;
                        case ResourceFile.ResourceType.Entity:
                            if (entities.ContainsKey(r.name))
                            {
                                continue;
                            }
                            options.Converters.Add(new JsonComponentTypeConverter());
                            options.Converters.Add(new JsonComponentPropertyConverter());
                            EntityResource entity = await JsonSerializer.DeserializeAsync<EntityResource>(fileStream, options) ?? throw new Exception("Failed to deserialize entity resource file");
                            entity.species = r.name;
                            entities.Add(r.name, entity);
                            break;
                        case ResourceFile.ResourceType.Room:
                            if (_rooms.ContainsKey(r.name))
                            {
                                continue;
                            }
                            options.Converters.Add(new JsonComponentTypeConverter());
                            options.Converters.Add(new JsonComponentPropertyConverter());
                            _rooms.Add(r.name, await JsonSerializer.DeserializeAsync<RoomResource>(fileStream, options) ?? throw new Exception("Failed to deserialize room resource file"));
                            break;
                        default:
                            Logger.Critical($"Failed to load resource of unknown type: {r.type}");
                            break;
                    }
                }
                catch (ResourceFormatException exception)
                {
                    Logger.Error($"Unaccepted resource format at: {r.path}", exception);
                    continue;
                }
                catch (JsonException exception)
                {
                    Logger.Error($"Failed to parse JSON of resource file: {r.path}", exception);
                    continue;
                }
                catch (Exception exception)
                {
                    Logger.Error($"Failed to load resource file: {r.path}", exception);
                    continue;
                }
            }
        }
    }
}
