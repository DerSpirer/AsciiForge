namespace AsciiEditor.Windows.Sprites;

public class SpriteEditor : Window
{
    private readonly string _path;
    private SpriteResource _sprite;
    
    public SpriteEditor(string path)
    {
        _path = path;
    }

    public override async Task Start()
    {
        _sprite = await SpriteResource.Read(_path) ?? throw new Exception("Failed to read sprite resource");
    }
    public override async Task Update(ConsoleKey key)
    {
    }
    public override async Task Draw()
    {
    }
}