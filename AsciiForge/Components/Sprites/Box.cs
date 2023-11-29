using AsciiForge.Engine.IO;
using AsciiForge.Engine.Resources;
using System.Drawing;

namespace AsciiForge.Components.Sprites
{
    public class Box : Drawable
    {
        public int boxWidth { get; private set; }
        public int boxHeight { get; private set; }

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
            char[,] text = new char[boxHeight,boxWidth];
            Color[,] fg = new Color[boxHeight,boxWidth];
            Color[,] bg = new Color[boxHeight,boxWidth];
            for (int i = 0; i < boxHeight; i++)
            {
                for (int j = 0; j < boxWidth; j++)
                {
                    text[i,j] = ' ';
                    fg[i,j] = Color.Transparent;
                    bg[i,j] = Color.Transparent;
                }
            }

            text[0, 0] = '+';
            text[0, boxWidth - 1] = '+';
            text[boxHeight - 1, boxWidth - 1] = '+';
            text[boxHeight - 1, 0] = '+';
            fg[0, 0] = Color.White;
            fg[0, boxWidth - 1] = Color.White;
            fg[boxHeight - 1, boxWidth - 1] = Color.White;
            fg[boxHeight - 1, 0] = Color.White;
            for (int i = 1; i < boxWidth - 1; i++)
            {
                text[0, i] = '—';
                text[boxHeight - 1, i] = '—';
                fg[0, i] = Color.White;
                fg[boxHeight - 1, i] = Color.White;
            }
            for (int i = 1; i < boxHeight - 1; i++)
            {
                text[i, 0] = '|';
                text[i, boxWidth - 1] = '|';
                fg[i, 0] = Color.White;
                fg[i, boxWidth - 1] = Color.White;
            }

            texture = new TextureResource(text, fg, bg);
        }
    }
}
