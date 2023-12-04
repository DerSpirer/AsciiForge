namespace AsciiEditor.Windows.FileBrowser;

public class FileBrowser : Window
{
    private readonly Folder _root;
    private int _selectedIndex;
    private int _rowOffset;

    public FileBrowser()
    {
        _root = new Folder(Editor.projectPath);
        _selectedIndex = 0;
        _rowOffset = 0;
    }

    public override async Task Start()
    {
    }
    public override async Task Update(ConsoleKey key)
    {
        BrowserItem[] allItems = _root.Flatten();
        BrowserItem[] view = _root.GetView(_rowOffset);
        switch (key)
        {
            case ConsoleKey.DownArrow:
                if (_selectedIndex + 1 >= view.Length)
                {
                    // TODO Read this again when sober
                    if (_rowOffset < allItems.Length - view.Length)
                    {
                        _rowOffset++;
                    }
                }
                else
                {
                    _selectedIndex++;
                }
                break;
            case ConsoleKey.UpArrow:
                if (_selectedIndex - 1 < 0)
                {
                    if (_rowOffset > 0)
                    {
                        _rowOffset--;
                    }
                }
                else
                {
                    _selectedIndex--;
                }
                break;
            case ConsoleKey.Enter:
                if (view[_selectedIndex] is Folder)
                {
                    Folder folder = (Folder)view[_selectedIndex];
                    folder.collapsed = !folder.collapsed;
                }
                break;
        }
    }

    public override async Task Draw()
    {
        BrowserItem[] view = _root.GetView(_rowOffset);
        if (view.Length <= 0)
        {
            return;
        }

        for (int i = 0; i < _selectedIndex && i < view.Length - 1; i++)
        {
            view[i].Draw();
            Console.WriteLine();
        }
        Console.BackgroundColor = ConsoleColor.White;
        Console.ForegroundColor = ConsoleColor.Black;
        view[_selectedIndex].Draw();
        Console.WriteLine();
        Console.BackgroundColor = ConsoleColor.Black;
        Console.ForegroundColor = ConsoleColor.White;
        for (int i = _selectedIndex + 1; i < view.Length - 1; i++)
        {
            view[i].Draw();
            Console.WriteLine();
        }
        view.Last().Draw();
        Console.SetCursorPosition(0, 0);
    }
}