using System.Drawing;
using System.Text.Json.Serialization;

namespace AsciiForge.Resources
{
    public class TextureResource : Resource
    {
        private readonly char?[,] _text;
        public char?[,] text { get { return _text; } }
        private readonly Color[,] _fg;
        public Color[,] fg { get { return _fg; } }
        private readonly Color[,] _bg;
        public Color[,] bg { get { return _bg; } }
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
                        (flipped._text[i, j], flipped._text[i, flipped.height - 1 - j]) = (flipped._text[i, flipped.height - 1 - j], flipped._text[i, j]);
                        (flipped._fg[i, j], flipped._fg[i, flipped.height - 1 - j]) = (flipped._fg[i, flipped.height - 1 - j], flipped._fg[i, j]);
                        (flipped._bg[i, j], flipped._bg[i, flipped.height - 1 - j]) = (flipped._bg[i, flipped.height - 1 - j], flipped._bg[i, j]);
                    }
                    for (int j = 0; j < flipped.width; j++)
                    {
                        if (flipped._text[i, j] != null)
                        {
                            (char, char) pair = flipHorizontalChars.Find(c => flipped._text[i, j] == c.Item1 || flipped._text[i, j] == c.Item2);
                            if (pair.Item1 != 0 && pair.Item2 != 0)
                            {
                                flipped._text[i, j] = flipped._text[i, j] == pair.Item1 ? pair.Item2 : pair.Item1;
                            }
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
                        (flipped._text[j, i], flipped._text[j, flipped.width - 1 - i]) = (flipped._text[j, flipped.width - 1 - i], flipped._text[j, i]);
                        (flipped._fg[j, i], flipped._fg[j, flipped.width - 1 - i]) = (flipped._fg[j, flipped.width - 1 - i], flipped._fg[j, i]);
                        (flipped._bg[j, i], flipped._bg[j, flipped.width - 1 - i]) = (flipped._bg[j, flipped.width - 1 - i], flipped._bg[j, i]);
                    }
                    for (int j = 0; j < flipped.height; j++)
                    {
                        if (flipped._text[j, i] != null)
                        {
                            (char, char) pair = flipVerticalChars.Find(c => flipped._text[j, i] == c.Item1 || flipped._text[j, i] == c.Item2);
                            if (pair.Item1 != 0 && pair.Item2 != 0)
                            {
                                flipped._text[j, i] = flipped._text[j, i] == pair.Item1 ? pair.Item2 : pair.Item1;
                            }
                        }
                    }
                }
                return flipped;
            }
        }

        [JsonConstructor]
        public TextureResource(char?[,] text, Color[,] fg, Color[,] bg)
        {
            _text = text;
            _fg = fg;
            _bg = bg;

            (bool isValid, string error) = IsValid();
            if (!isValid)
            {
                throw new ResourceFormatException(error);
            }
        }
        public TextureResource(TextureResource texture)
            : this((char?[,])texture._text.Clone(),
                  (Color[,])texture._fg.Clone(),
                  (Color[,])texture._bg.Clone())
        {
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
            if ((_fg?.GetLength(1) ?? 0) != width || _fg!.GetLength(0) != height)
            {
                error = "Texture with foreground colors of different width or height";
                return (isValid, error);
            }
            if ((_bg?.GetLength(1) ?? 0) != width || _bg!.GetLength(0) != height)
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
