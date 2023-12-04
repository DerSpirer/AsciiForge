namespace AsciiEditor.Windows.FileBrowser;

public class Folder : BrowserItem
{
    public readonly List<Folder> folders;
    public readonly List<File> files;
    public bool collapsed;

    public Folder(string path)
        : base(path)
    {
        folders = Directory.GetDirectories(path).Select(d => new Folder(d)).ToList();
        files = Directory.GetFiles(path).Select(f => new File(f)).ToList();
        collapsed = true;
    }

    public override void Draw()
    {
        Console.Write(new string(' ', 2 * indent) + (collapsed ? "> " : "v ") + name);
    }
    public void Indent(int offset)
    {
        indent = offset;
        foreach (Folder folder in folders)
        {
            folder.Indent(offset + 1);
        }
        foreach (File file in files)
        {
            file.indent = offset + 1;
        }
    }

    public BrowserItem[] Flatten()
    {
        List<BrowserItem> items = new List<BrowserItem>();
        items.Add(this);
        folders.ForEach(f =>
        {
            if (f.collapsed)
            {
                items.Add(f);
            }
            else
            {
                items.AddRange(f.Flatten());
            }
        });
        files.ForEach(f => items.Add(f));
        return items.ToArray();
    }
    public BrowserItem[] GetView(int rowOffset)
    {
        return Flatten().Skip(rowOffset).Take(Editor.screenHeight).ToArray();
    }
}