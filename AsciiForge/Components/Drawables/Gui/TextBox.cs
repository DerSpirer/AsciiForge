using System.Drawing;
using AsciiForge.Engine.Resources;

namespace AsciiForge.Components.Drawables.Gui;

public abstract class TextBox : Drawable
{
    public int boxWidth { get; set; } = 30;
    public int boxHeight { get; set; } = 10;
    public int paddingHorizontal { get; set; } = 0;
    public int paddingVertical { get; set; } = 0;
    public Color bg { get; set; } = Color.Transparent;
    public Color fg { get; set; } = Color.White;
    private string _text = string.Empty;
    public string text
    {
        get
        {
            return _text;
        }
        protected set
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

    private void CreateBox()
    {
        TextureResource t = new TextureResource(boxWidth, boxHeight, Color.Transparent, bg);
        if (string.IsNullOrEmpty(text))
        {
            texture = t;
            return;
        }

        int x = paddingHorizontal;
        int y = paddingVertical;
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == ' ')
            {
                x++;
                continue;
            }
            if (text[i] == '\n' || x + GetWordLength(i) >= boxWidth - paddingHorizontal)
            {
                x = paddingHorizontal;
                y++;
                if (text[i] == '\n')
                {
                    continue;
                }
            }
            t.text[y, x] = text[i];
            t.fg[y, x] = fg;
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