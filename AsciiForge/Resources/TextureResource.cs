using System.Text.Json.Serialization;

namespace AsciiForge.Resources
{
    public class TextureResource : Resource
    {
        private readonly char[,] _text;
        public char[,] text { get { return _text; } }
        private readonly ConsoleColor[,] _foregroundColors;
        public ConsoleColor[,] foregroundColors { get { return _foregroundColors; } }
        private readonly ConsoleColor[,] _backgroundColors;
        public ConsoleColor[,] backgroundColors { get { return _backgroundColors; } }
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
                        (flipped._foregroundColors[i, j], flipped._foregroundColors[i, flipped.height - 1 - j]) = (flipped._foregroundColors[i, flipped.height - 1 - j], flipped._foregroundColors[i, j]);
                        (flipped._backgroundColors[i, j], flipped._backgroundColors[i, flipped.height - 1 - j]) = (flipped._backgroundColors[i, flipped.height - 1 - j], flipped._backgroundColors[i, j]);
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
                        (flipped._foregroundColors[j, i], flipped._foregroundColors[j, flipped.width - 1 - i]) = (flipped._foregroundColors[j, flipped.width - 1 - i], flipped._foregroundColors[j, i]);
                        (flipped._backgroundColors[j, i], flipped._backgroundColors[j, flipped.width - 1 - i]) = (flipped._backgroundColors[j, flipped.width - 1 - i], flipped._backgroundColors[j, i]);
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
        public TextureResource(char[,] text, ConsoleColor[,] foregroundColors, ConsoleColor[,] backgroundColors)
        {
            _text = text;
            _foregroundColors = foregroundColors;
            _backgroundColors = backgroundColors;

            (bool isValid, string error) = IsValid();
            if (!isValid)
            {
                throw new ResourceFormatException(error);
            }
        }
        public TextureResource(TextureResource texture)
            : this((char[,])texture._text.Clone(),
                  (ConsoleColor[,])texture._foregroundColors.Clone(),
                  (ConsoleColor[,])texture._backgroundColors.Clone())
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
            if ((foregroundColors?.GetLength(1) ?? 0) != width || foregroundColors!.GetLength(0) != height)
            {
                error = "Texture with foreground colors of different width or height";
                return (isValid, error);
            }
            if ((backgroundColors?.GetLength(1) ?? 0) != width || backgroundColors!.GetLength(0) != height)
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
