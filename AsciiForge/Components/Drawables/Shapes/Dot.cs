using System.Drawing;
using AsciiForge.Engine.Resources;

namespace AsciiForge.Components.Drawables.Shapes
{
    public class Dot : Drawable
    {
        private void Start()
        {
            texture = new TextureResource(new char[,] { { 'o' } }, new Color[,] { { Color.White } }, new Color[,] { { Color.Transparent } });
        }
    }
}
