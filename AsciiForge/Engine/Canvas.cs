using AsciiForge.Resources;

namespace AsciiForge.Engine
{
    public class Canvas
    {
        private readonly int _width;
        public int width
        {
            get
            {
                return _width;
            }
        }
        private readonly int _height;
        public int height
        {
            get
            {
                return _height;
            }
        }
        private readonly char[,] _text;
        public char[,] text
        {
            get
            {
                return _text;
            }
        }
        private readonly ConsoleColor[,] _foregroundColors;
        public ConsoleColor[,] foregroundColors
        {
            get
            {
                return _foregroundColors;
            }
        }
        private readonly ConsoleColor[,] _backgroundColors;
        public ConsoleColor[,] backgroundColors
        {
            get
            {
                return _backgroundColors;
            }
        }

        public Canvas(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                Logger.Error("Failed to create canvas with negative width or height");
                throw new Exception("Failed to create canvas with negative width or height");
            }
            _width = width;
            _height = height;
            _text = new char[_height,_width];
            _foregroundColors = new ConsoleColor[_height,_width];
            _backgroundColors = new ConsoleColor[_height,_width];
            Clear();
        }
        public Canvas(Canvas canvas)
        {
            if (canvas.width <= 0 || canvas.height <= 0)
            {
                Logger.Error("Failed to create canvas with negative width or height");
                throw new Exception("Failed to create canvas with negative width or height");
            }
            _width = canvas.width;
            _height = canvas.height;
            _text = (char[,])canvas._text.Clone();
            _foregroundColors = (ConsoleColor[,])canvas._foregroundColors.Clone();
            _backgroundColors = (ConsoleColor[,])canvas._backgroundColors.Clone();
        }

        public void Clear()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _text[i,j] = ' ';
                    _foregroundColors[i,j] = ConsoleColor.White;
                    _backgroundColors[i,j] = ConsoleColor.Black;
                }
            }
        }
        public void Draw(char? chr, ConsoleColor? foregroundColor, ConsoleColor? backgroundColor, float x, float y)
        {
            if (x < 0 || x >= _width || y < 0 || y >= _height)
            {
                return;
            }

            int xx = (int)Math.Round(x);
            int yy = (int)Math.Round(y);
            if (chr != null)
            {
                _text[yy, xx] = (char)chr;
            }
            if (foregroundColor != null)
            {
                _foregroundColors[yy, xx] = (ConsoleColor)foregroundColor;
            }
            if (backgroundColor != null)
            {
                _backgroundColors[yy, xx] = (ConsoleColor)backgroundColor;
            }
        }
        public void Draw(string text, ConsoleColor?[] foregroundColors, ConsoleColor?[] backgroundColors, float x, float y)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Draw(text[i], foregroundColors[i], backgroundColors[i], x + i, y);
            }
        }
        public void Draw(TextureResource texture, float x, float y)
        {
            for (int i = 0; i < texture.height; i++)
            {
                for (int j = 0; j < texture.width; j++)
                {
                    Draw(texture.text[i, j], texture.foregroundColors[i, j], texture.backgroundColors[i, j], x + j, y + i);
                }
            }
        }
    }
}
