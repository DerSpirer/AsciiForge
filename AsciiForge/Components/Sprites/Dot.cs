using AsciiForge.Resources;

namespace AsciiForge.Components.Sprites
{
    public class Dot : Drawable
    {
        private void Start()
        {
            texture = new TextureResource(new char[,] { { 'o' } }, new ConsoleColor[,] { { ConsoleColor.White } }, new ConsoleColor[,] { { ConsoleColor.Black } });
        }
    }
}
