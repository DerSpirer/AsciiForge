using System.Drawing;
using AsciiForge.Engine.IO;
using AsciiForge.Engine.Resources;

namespace AsciiForge.Components.Drawables.Shapes
{
    public class Box : Drawable
    {
        public int boxWidth { get; private set; } = 2;
        public int boxHeight { get; private set; } = 2;
        private Color _stroke = Color.White;
        public Color stroke
        {
            get
            {
                return _stroke;
            }
            set
            {
                if (value != _stroke)
                {
                    _stroke = value;
                    CreateBox();
                }
            }
        }
        private Color _fill = Color.Transparent;
        public Color fill
        {
            get
            {
                return _fill;
            }
            set
            {
                if (value != _fill)
                {
                    _fill = value;
                    CreateBox();
                }
            }
        }
        
        private void Start()
        {
            if (boxWidth < 2)
            {
                Logger.Warning($"Trying to create a box of width smaller than 2: {boxWidth}");
                boxWidth = 2;
            }
            if (boxHeight < 2)
            {
                Logger.Warning($"Trying to create a box of height smaller than 2: {boxHeight}");
                boxHeight = 2;
            }
            CreateBox();
        }

        public void SetSize(int width, int height)
        {
            if (width < 2)
            {
                Logger.Warning($"Trying to create a box of width smaller than 2: {width}");
                width = 2;
            }
            if (height < 2)
            {
                Logger.Warning($"Trying to create a box of height smaller than 2: {height}");
                height = 2;
            }
            if (boxWidth == width && boxHeight == height)
            {
                return;
            }
            boxWidth = width;
            boxHeight = height;
            CreateBox();
        }
        private void CreateBox()
        {
            TextureResource t = new TextureResource(boxWidth, boxHeight, _fill, _fill);

            t.text[0, 0] = '+';
            t.text[0, boxWidth - 1] = '+';
            t.text[boxHeight - 1, boxWidth - 1] = '+';
            t.text[boxHeight - 1, 0] = '+';
            t.fg[0, 0] = _stroke;
            t.fg[0, boxWidth - 1] = _stroke;
            t.fg[boxHeight - 1, boxWidth - 1] = _stroke;
            t.fg[boxHeight - 1, 0] = _stroke;
            for (int i = 1; i < boxWidth - 1; i++)
            {
                t.text[0, i] = '—';
                t.text[boxHeight - 1, i] = '—';
                t.fg[0, i] = _stroke;
                t.fg[boxHeight - 1, i] = _stroke;
            }
            for (int i = 1; i < boxHeight - 1; i++)
            {
                t.text[i, 0] = '|';
                t.text[i, boxWidth - 1] = '|';
                t.fg[i, 0] = _stroke;
                t.fg[i, boxWidth - 1] = _stroke;
            }

            texture = t;
        }
    }
}
