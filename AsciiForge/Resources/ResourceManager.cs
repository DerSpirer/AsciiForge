using AsciiForge.Engine;
using AsciiForge.Helpers;
using System.Text.Json;

namespace AsciiForge.Resources
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
                if (!IsResource(path))
                {
                    Logger.Error($"Unknown resource type: {path}");
                    throw new Exception($"Unknown resource type: {path}");
                }

                (ResourceType type, string suffix) = _typesSuffixes.ToList().Find(s => path.EndsWith(s.Item2));
                this.path = path;
                this.type = type;
                int nameStart = path.LastIndexOf('\\');
                nameStart = nameStart < 0 ? 0 : nameStart + 1;
                this.name = path[nameStart..^suffix.Length];
            }

            private static readonly (ResourceType, string)[] _typesSuffixes = new (ResourceType, string)[] { (ResourceType.Sprite, ".sprite.json"), (ResourceType.Sound, ".sound.json"), (ResourceType.Entity, ".entity.json"), (ResourceType.Room, ".room.json") };
            public static bool IsResource(string path) => _typesSuffixes.Any(s => path.EndsWith(s.Item2));
        }

        internal static async Task Load()
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            ResourceFile[] resources = await FindResources(directory);
            if (resources.Length <= 0)
            {
                return;
            }
            await LoadResources(resources.Where(r => r.type == ResourceFile.ResourceType.Sprite).ToArray());
            await LoadResources(resources.Where(r => r.type == ResourceFile.ResourceType.Sound).ToArray());
            await LoadResources(resources.Where(r => r.type == ResourceFile.ResourceType.Entity).ToArray());
            await LoadResources(resources.Where(r => r.type == ResourceFile.ResourceType.Room).ToArray());
            await LoadRoomOrder(directory);
        }
        private static async Task<ResourceFile[]> FindResources(string directory)
        {
            List<ResourceFile> resourcePaths = new List<ResourceFile>();

            foreach (string dir in Directory.GetDirectories(directory))
            {
                resourcePaths.AddRange(await FindResources(dir));
            }
            resourcePaths.AddRange(Directory.GetFiles(directory).Where(f => ResourceFile.IsResource(f)).Select(f => new ResourceFile(f)));

            return resourcePaths.ToArray();
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
                            options.Converters.Add(new JsonRectangularArrayConverter<char>());
                            options.Converters.Add(new JsonRectangularArrayConverter<ConsoleColor>());
                            sprites.Add(r.name, await JsonSerializer.DeserializeAsync<SpriteResource>(fileStream, options) ?? throw new Exception("Failed to deserialize sprite resource file"));
                            break;
                        case ResourceFile.ResourceType.Sound:
                            if (sounds.ContainsKey(r.name))
                            {
                                continue;
                            }
                            sounds.Add(r.name, await JsonSerializer.DeserializeAsync<SoundResource>(fileStream, options) ?? throw new Exception("Failed to deserialize sound resource file"));
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
        private static async Task LoadRoomOrder(string directory)
        {
            try
            {
                const string fileName = "roomsList.json";
                string path = Path.Combine(directory, fileName);
                using FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
                string[]? order = await JsonSerializer.DeserializeAsync<string[]>(fileStream);
                if (order == null)
                {
                    Logger.Critical("Failed to read parse roomsOrder.json");
                    return;
                }
                if (order.Length == 0)
                {
                    Logger.Critical("roomOrder.json contains empty array");
                }

                foreach (string room in order)
                {
                    if (_rooms.ContainsKey(room))
                    {
                        rooms.Add(_rooms[room]);
                    }
                    else
                    {
                        Logger.Warning($@"'{room}' in roomsOrder.json references a non-existing room resource");
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                Logger.Critical("File roomsOrder.json not found", exception);
            }
            catch (JsonException exception)
            {
                Logger.Critical("Failed to parse roomsOrder.json", exception);
            }
            catch (Exception exception)
            {
                Logger.Critical("Failed to load roomsOrder.json", exception);
            }
        }
    }
}
