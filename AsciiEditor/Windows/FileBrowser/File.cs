namespace AsciiEditor.Windows.FileBrowser;

public class File : BrowserItem
{
    public ResourceType type;

    public File(string path)
        : base(path)
    {
        if (_typesSuffixes.Any(s => path.EndsWith(s.Item1)))
        {
            type = _typesSuffixes.First(s => path.EndsWith(s.Item1)).Item2;
        }
        else
        {
            type = ResourceType.Undefined;
        }
    }
    
    public override void Draw()
    {
        Console.Write(new string(' ', 2 * indent) + "+ " + name);
    }

    private static readonly (string, ResourceType)[] _typesSuffixes = new (string, ResourceType)[]
    {
        (".sprite.json", ResourceType.Sprite),
        (".sound.wav", ResourceType.Sound),
        (".sound.ogg", ResourceType.Sound),
        (".entity.json", ResourceType.Entity),
        (".room.json", ResourceType.Room),
    };
    public enum ResourceType
    {
        Undefined,
        Sprite,
        Sound,
        Entity,
        Room
    }
}