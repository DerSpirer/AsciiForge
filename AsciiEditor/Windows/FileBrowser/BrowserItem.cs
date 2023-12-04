namespace AsciiEditor.Windows.FileBrowser;

public abstract class BrowserItem
{
    public readonly string path;
    public readonly string name;
    public int indent;

    protected BrowserItem(string path)
    {
        this.path = path;
        int nameIndex = path.LastIndexOf("\\");
        this.name = nameIndex < 0 ? path : path[(nameIndex + 1)..];
        this.indent = 0;
    }

    public abstract void Draw();
}