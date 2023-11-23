using AsciiForge.Resources;
using System.Drawing;

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
        private readonly Color[,] _fg;
        public Color[,] fg
        {
            get
            {
                return _fg;
            }
        }
        private readonly Color[,] _bg;
        public Color[,] bg
        {
            get
            {
                return _bg;
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
            _fg = new Color[_height,_width];
            _bg = new Color[_height,_width];
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
            _fg = (Color[,])canvas._fg.Clone();
            _bg = (Color[,])canvas._bg.Clone();
        }

        public void Clear()
        {
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    _text[i,j] = ' ';
                    _fg[i,j] = Color.White;
                    _bg[i,j] = Color.Black;
                }
            }
        }
        public void Draw(char? chr, Color fg, Color bg, float x, float y, BlendMode blendMode = BlendMode.Alpha)
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
            _fg[yy, xx] = BlendColors(fg, _fg[yy, xx], blendMode);
            _bg[yy, xx] = BlendColors(bg, _bg[yy, xx], blendMode);
        }
        public void Draw(string text, Color[] fg, Color[] bg, float x, float y, BlendMode blendMode = BlendMode.Alpha)
        {
            for (int i = 0; i < text.Length; i++)
            {
                Draw(text[i], fg[i], bg[i], x + i, y, blendMode);
            }
        }
        public void Draw(TextureResource texture, float x, float y, BlendMode blendMode = BlendMode.Alpha)
        {
            for (int i = 0; i < texture.height; i++)
            {
                for (int j = 0; j < texture.width; j++)
                {
                    Draw(texture.text[i, j], texture.fg[i, j], texture.bg[i, j], x + j, y + i, blendMode);
                }
            }
        }

        private Color BlendColors(Color src, Color dst, BlendMode blendMode)
        {
            int resultAlpha = dst.A;
            int resultRed = dst.R;
            int resultGreen = dst.G;
            int resultBlue = dst.B;

            switch (blendMode)
            {
                case BlendMode.Alpha:
                    float srcAlpha = src.A / 255.0f;
                    float dstAlpha = dst.A / 255.0f;
                    
                    resultAlpha = (byte)Math.Round((srcAlpha + dstAlpha * (1 - srcAlpha)) * 255.0f);
                    resultRed = (byte)Math.Round((src.R * srcAlpha + dst.R * (1 - srcAlpha)));
                    resultGreen = (byte)Math.Round((src.G * srcAlpha + dst.G * (1 - srcAlpha)));
                    resultBlue = (byte)Math.Round((src.B * srcAlpha + dst.B * (1 - srcAlpha)));
                    break;
                case BlendMode.Additive:
                    resultAlpha = Math.Min(src.A + dst.A, 255);
                    resultRed = Math.Min(src.R + dst.R, 255);
                    resultGreen = Math.Min(src.G + dst.G, 255);
                    resultBlue = Math.Min(src.B + dst.B, 255);
                    break;
            }

            return Color.FromArgb(resultAlpha, resultRed, resultGreen, resultBlue);
        }
        public enum BlendMode
        {
            Alpha,
            Additive,
        }
    }
}
