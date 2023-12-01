using System.Text;
using AsciiEditor;

Console.WriteLine("Hello, World!");

if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
{
    throw new Exception("Usage: AsciiEditor.exe <PROJECT_PATH>");
}
await Editor.Commence(args[0]);