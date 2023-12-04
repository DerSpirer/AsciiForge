using System.Drawing;
using System.Text.Json.Serialization;

namespace AsciiForge.Engine.Resources
{
    public class TextureResource : Resource
    {
        public char[,] text { get; private set; }
        public Color[,] fg { get; private set; }
        public Color[,] bg { get; private set; }
        [JsonIgnore]
        public int width { get { return text.GetLength(1); } }
        [JsonIgnore]
        public int height { get { return text.GetLength(0); } }
        [JsonIgnore]
        public TextureResource flippedHorizontal
        {
            get
            {
                TextureResource flipped = new TextureResource(this);
                for (int i = 0; i < flipped.height; i++)
                {
                    for (int j = 0; j < flipped.width / 2; j++)
                    {
                        (flipped.text[i, j], flipped.text[i, flipped.height - 1 - j]) = (flipped.text[i, flipped.height - 1 - j], flipped.text[i, j]);
                        (flipped.fg[i, j], flipped.fg[i, flipped.height - 1 - j]) = (flipped.fg[i, flipped.height - 1 - j], flipped.fg[i, j]);
                        (flipped.bg[i, j], flipped.bg[i, flipped.height - 1 - j]) = (flipped.bg[i, flipped.height - 1 - j], flipped.bg[i, j]);
                    }
                    for (int j = 0; j < flipped.width; j++)
                    {
                        (char, char) pair = flipHorizontalChars.Find(c => flipped.text[i, j] == c.Item1 || flipped.text[i, j] == c.Item2);
                        if (pair.Item1 != 0 && pair.Item2 != 0)
                        {
                            flipped.text[i, j] = flipped.text[i, j] == pair.Item1 ? pair.Item2 : pair.Item1;
                        }
                    }
                }
                return flipped;
            }
        }
        [JsonIgnore]
        public TextureResource flippedVertical
        {
            get
            {
                TextureResource flipped = new TextureResource(this);
                for (int i = 0; i < flipped.width; i++)
                {
                    for (int j = 0; j < flipped.height / 2; j++)
                    {
                        (flipped.text[j, i], flipped.text[j, flipped.width - 1 - i]) = (flipped.text[j, flipped.width - 1 - i], flipped.text[j, i]);
                        (flipped.fg[j, i], flipped.fg[j, flipped.width - 1 - i]) = (flipped.fg[j, flipped.width - 1 - i], flipped.fg[j, i]);
                        (flipped.bg[j, i], flipped.bg[j, flipped.width - 1 - i]) = (flipped.bg[j, flipped.width - 1 - i], flipped.bg[j, i]);
                    }
                    for (int j = 0; j < flipped.height; j++)
                    {
                        (char, char) pair = flipVerticalChars.Find(c => flipped.text[j, i] == c.Item1 || flipped.text[j, i] == c.Item2);
                        if (pair.Item1 != 0 && pair.Item2 != 0)
                        {
                            flipped.text[j, i] = flipped.text[j, i] == pair.Item1 ? pair.Item2 : pair.Item1;
                        }
                    }
                }
                return flipped;
            }
        }

        [JsonConstructor]
        public TextureResource(char[,] text, Color[,] fg, Color[,] bg)
        {
            this.text = text;
            this.fg = fg;
            this.bg = bg;

            (bool isValid, string error) = IsValid();
            if (!isValid)
            {
                throw new ResourceFormatException(error);
            }
        }
        public TextureResource(TextureResource texture)
            : this((char[,])texture.text.Clone(),
                  (Color[,])texture.fg.Clone(),
                  (Color[,])texture.bg.Clone())
        {
        }
        public TextureResource(int width, int height, Color fgColor, Color bgColor)
        {
            text = new char[height, width];
            fg = new Color[height, width];
            bg = new Color[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    text[i,j] = ' ';
                    fg[i,j] = fgColor;
                    bg[i,j] = bgColor;
                }
            }
        }

        protected override (bool, string) IsValid()
        {
            bool isValid = false;
            string error = string.Empty;

            int width = text?.GetLength(1) ?? 0;
            int height = text?.GetLength(0) ?? 0;
            if (width <= 0 || height <= 0)
            {
                error = "Texture with invalid width or height";
                return (isValid, error);
            }
            if ((fg?.GetLength(1) ?? 0) != width || fg!.GetLength(0) != height)
            {
                error = "Texture with foreground colors of different width or height";
                return (isValid, error);
            }
            if ((bg?.GetLength(1) ?? 0) != width || bg!.GetLength(0) != height)
            {
                error = "Texture with background colors of different width or height";
                return (isValid, error);
            }

            isValid = true;
            return (isValid, error);
        }

        public static readonly List<(char, char)> flipHorizontalChars = new List<(char, char)>
        {
            ('>', '<'), ('/', '\\'), ('(', ')'), ('[', ']'), ('{', '}'), ('d', 'b'), ('q', 'p'), ('Z', 'S'),
        };
        public static readonly List<(char, char)> flipVerticalChars = new List<(char, char)>
        {
            ('/', '\\'), ('W', 'M'), ('p', 'b'), ('Z', 'S'),
        };
    }
}
