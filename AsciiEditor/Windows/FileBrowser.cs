namespace AsciiEditor.Windows;

public class FileBrowser : Window
{
    private readonly Folder _root;
    private BrowserItem _selectedItem;
    private int _rowOffset;

    public FileBrowser()
    {
        _root = new Folder(Editor.projectPath);
        _selectedItem = _root;
        _rowOffset = 0;
    }
    
    public override async Task Update(ConsoleKey key)
    {
        (int, BrowserItem)[] indentedItems = GetIndentedItems();
        int prevIndex = 0;
        switch (key)
        {
            case ConsoleKey.DownArrow:
                for (int i = 0; i < indentedItems.Length; i++)
                {
                    if (indentedItems[i].Item2 == _selectedItem)
                    {
                        prevIndex = i;
                        break;
                    }
                }
                _selectedItem = indentedItems[Math.Clamp(prevIndex + 1, 0, indentedItems.Length - 1)].Item2;
                break;
            case ConsoleKey.UpArrow:
                for (int i = 0; i < indentedItems.Length; i++)
                {
                    if (indentedItems[i].Item2 == _selectedItem)
                    {
                        prevIndex = i;
                        break;
                    }
                }
                _selectedItem = indentedItems[Math.Clamp(prevIndex - 1, 0, indentedItems.Length - 1)].Item2;
                break;
            case ConsoleKey.Enter:
                if (_selectedItem is Folder)
                {
                    Folder folder = (Folder)_selectedItem;
                    folder.collapsed = !folder.collapsed;
                }
                break;
        }
    }

    public override async Task Draw()
    {
        (int, BrowserItem)[] indentedItems = GetIndentedItems();
        foreach ((int, BrowserItem) indentedItem in indentedItems.Take(indentedItems.Length - 1))
        {
            DrawIndentedItem(indentedItem);
            Console.WriteLine();
        }
        DrawIndentedItem(indentedItems.Last());
        Console.SetCursorPosition(0, 0);
    }
    private void DrawIndentedItem((int, BrowserItem) indentedItem)
    {
        (int indent, BrowserItem item) = indentedItem;
        
        string prefix = "";
        for (int i = 0; i < indent; i++)
        {
            prefix += "  ";
        }

        if (item is File)
        {
            prefix += "+ ";
        }
        else if (item is Folder folder)
        {
            prefix += folder.collapsed ? "> " : "v ";
        }

        if (_selectedItem == item)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
        }
        Console.Write(prefix + item.name);
        if (_selectedItem == item)
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
    private (int, BrowserItem)[] GetIndentedItems() => _root.GetIndentedItems().Skip(_rowOffset).Take(Editor.screenHeight).ToArray();
    
    private abstract class BrowserItem
    {
        public readonly string path;
        public readonly string name;

        protected BrowserItem(string path)
        {
            this.path = path;
            int nameIndex = path.LastIndexOf("\\");
            this.name = nameIndex < 0 ? path : path[(nameIndex + 1)..];
        }
    }
    private class Folder : BrowserItem
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

        public List<(int, BrowserItem)> GetIndentedItems()
        {
            List<(int, BrowserItem)> indented = new List<(int, BrowserItem)>();
            indented.Add((0, this));
            if (!collapsed)
            {
                foreach (Folder folder in folders)
                {
                    indented.AddRange(folder.GetIndentedItems().Select(i => (i.Item1 + 1, i.Item2)));
                }
                foreach (File file in files)
                {
                    indented.Add((1, file));
                }
            }
            return indented;
        }
    }
    private class File : BrowserItem
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
}