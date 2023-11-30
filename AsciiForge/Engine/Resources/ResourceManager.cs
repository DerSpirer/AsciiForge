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

        public struct ResourceFile
        {
            public readonly string path;
            public readonly ResourceType type;
            public readonly string name;

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
                if (!typesSuffixes.Any(t => path.EndsWith(t.Item2)))
                {
                    Logger.Error($"Unknown resource type: {path}");
                    throw new Exception($"Unknown resource type: {path}");
                }

                (ResourceType type, string suffix) = typesSuffixes.ToList().Find(s => path.EndsWith(s.Item2));
                this.path = path;
                this.type = type;
                int nameStart = path.LastIndexOf('\\');
                nameStart = nameStart < 0 ? 0 : nameStart + 1;
                name = path[nameStart..^suffix.Length];
            }

            internal static readonly (ResourceType, string)[] typesSuffixes = new (ResourceType, string)[] { (ResourceType.Sprite, ".sprite.json"), (ResourceType.Entity, ".entity.json"), (ResourceType.Room, ".room.json"), (ResourceType.Sound, ".sound.mp3"), (ResourceType.Sound, ".sound.wav"), };
        }

        internal static async Task Load()
        {
            string directory = Path.Combine(Directory.GetCurrentDirectory(), "Assets");
            await LoadResources(Directory.EnumerateFiles(directory, "*.sprite.json", SearchOption.AllDirectories).Select(f => new ResourceFile(f)).ToArray());
            await LoadResources(Directory.EnumerateFiles(directory, "*.sound.*", SearchOption.AllDirectories).Where(f => ResourceFile.typesSuffixes.Any(s => f.EndsWith(s.Item2))).Select(f => new ResourceFile(f)).ToArray());
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
                    switch (r.type)
                    {
                        case ResourceFile.ResourceType.Sprite:
                            if (sprites.ContainsKey(r.name))
                            {
                                continue;
                            }
                            sprites.Add(r.name, await SpriteResource.Read(r));
                            break;
                        case ResourceFile.ResourceType.Sound:
                            if (sounds.ContainsKey(r.name))
                            {
                                continue;
                            }
                            sounds.Add(r.name, await SoundResource.Read(r));
                            break;
                        case ResourceFile.ResourceType.Entity:
                            if (entities.ContainsKey(r.name))
                            {
                                continue;
                            }
                            entities.Add(r.name, await EntityResource.Read(r));
                            break;
                        case ResourceFile.ResourceType.Room:
                            if (_rooms.ContainsKey(r.name))
                            {
                                continue;
                            }
                            _rooms.Add(r.name, await RoomResource.Read(r));
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
