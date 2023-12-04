using System.Drawing;
using AsciiForge.Engine.IO;
using AsciiForge.Engine.Resources;

namespace AsciiForge.Components.Drawables.Primitives;

public class TextBox : Drawable
{
    public int boxWidth { get; private set; } = Screen.width;
    public int boxHeight { get; private set; } = Screen.height;
    public int padding { get; private set; } = 0;

    private Color _bg = Color.Transparent;
    public Color bg
    {
        get
        {
            return _bg;
        }
        set
        {
            if (value != _bg)
            {
                _bg = value;
                CreateBox();
            }
        }
    }
    private Color _fg = Color.White;
    public Color fg
    {
        get
        {
            return _fg;
        }
        set
        {
            if (value != _fg)
            {
                _fg = value;
                CreateBox();
            }
        }
    }
    
    private string _text = string.Empty;
    public string text
    {
        get
        {
            return _text;
        }
        set
        {
            if (value != _text)
            {
                _text = value;
                CreateBox();
            }
        }
    }

    private void Start()
    {
        CreateBox();
    }
    
    public void SetSize(int width, int height, int padding)
    {
        if (boxWidth == width && boxHeight == height && this.padding == padding)
        {
            return;
        }
        boxWidth = width;
        boxHeight = height;
        this.padding = padding;
        CreateBox();
    }
    private void CreateBox()
    {
        TextureResource t = new TextureResource(boxWidth, boxHeight, Color.Transparent, _bg);
        if (string.IsNullOrEmpty(text))
        {
            texture = t;
            return;
        }
        
        int x = padding;
        int y = padding;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                x++;
                continue;
            }
            if (text[i] == '\n' || x + GetWordLength(i) >= boxWidth - padding)
            {
                x = padding;
                y++;
                if (text[i] == '\n')
                {
                    continue;
                }
            }
            t.text[y, x] = text[i];
            t.fg[y, x] = Color.White;
            x++;
        }

        texture = t;
    }
    private int GetWordLength(int index)
    {
        int len = 0;
        while (index + len < text.Length && char.IsLetterOrDigit(text[index + len]))
        {
            len++;
        }
        return len;
    }
}