namespace AsciiEditor.Windows;

public abstract class Window
{
    public abstract Task Update(ConsoleKey key);
    public abstract Task Draw();
}