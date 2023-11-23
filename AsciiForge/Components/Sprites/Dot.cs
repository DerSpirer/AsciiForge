using AsciiForge.Resources;
using System.Drawing;

namespace AsciiForge.Components.Sprites
{
    public class Dot : Drawable
    {
        private void Start()
        {
            texture = new TextureResource(new char?[,] { { 'o' } }, new Color[,] { { Color.White } }, new Color[,] { { Color.Black } });
        }
    }
}
