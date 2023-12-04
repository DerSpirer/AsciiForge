using System.Text;
using AsciiEditor.Windows;
using AsciiEditor.Windows.FileBrowser;

namespace AsciiEditor;

public static class Editor
{
    public const int screenWidth = 120;
    public const int screenHeight = 30;
    private static readonly List<Window> _windows = new List<Window>();
    private static string _projectPath;
    public static string projectPath => _projectPath;

    public static async Task Commence(string projectPath)
    {
        Console.SetWindowSize(screenWidth, screenHeight);
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;
        _projectPath = projectPath;
        
        _windows.Add(new FileBrowser());
        while (_windows.Count > 0)
        {
            // Draw
            Console.Clear();
            await _windows.Last().Draw();
            // Update
            ConsoleKey key = Console.ReadKey(true).Key;
            await _windows.Last().Update(key);
        }
    }
}