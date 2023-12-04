using AsciiForge.Engine;

namespace AsciiForge.Components.Drawables.Gui;

public class GuiElement : Component
{
    public float zIndex { get; set; } = 0;

    public Vector3 position
    {
        get
        {
            return transform.position;
        }
        set
        {
            if (transform.position != value)
            {
                value.z = zIndex;
                transform.position = value;
            }
        }
    }
}