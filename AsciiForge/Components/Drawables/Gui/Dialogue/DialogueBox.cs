using AsciiForge.Components.Drawables.Primitives;
using AsciiForge.Engine;
using AsciiForge.Engine.IO;

namespace AsciiForge.Components.Drawables.Gui.Dialogue;

public class DialogueBox : GuiElement
{
    public int boxHeight { get; set; } = 10;
    public int margin { get; set; } = 2;
    public int padding { get; set; } = 2;
    public string text { get; set; } = string.Empty;

    protected bool displayInstantly = false;
    protected float incDisplayDelay = 0.2f;
    private int _displayCount;
    private float _incDelayTimer;
    private Box _box;
    private TextBox _textBox;

    protected void Start()
    {
        _box = entity.FindComponent<Box>()!;
        _box.isGui = true;
        _box.anchor = Drawable.Anchor.BottomCenter;
        _textBox = entity.FindComponent<TextBox>()!;
        _textBox.isGui = true;
        _textBox.anchor = Drawable.Anchor.BottomCenter;
        if (displayInstantly)
        {
            _displayCount = text.Length;
            _textBox.text = text;
        }
        else
        {
            _incDelayTimer = incDisplayDelay;
        }
        UpdateLayout();
    }

    private void Update(float deltaTime)
    {
        if (_incDelayTimer <= 0)
        {
            return;
        }
        _incDelayTimer = Math.Max(_incDelayTimer - deltaTime, 0);
        if (_incDelayTimer <= 0)
        {
            _displayCount = Math.Min(_displayCount + 1, text.Length);
            _textBox.text = text[.._displayCount];
            if (_displayCount < text.Length)
            {
                _incDelayTimer = incDisplayDelay;
            }
        }
    }
    
    private void UpdateLayout()
    {
        _box.SetSize(Screen.width - margin * 2, boxHeight);
        position = new Vector3(Screen.width / 2f, Screen.height - margin, 0);
        _textBox.SetSize(Screen.width - margin * 2, boxHeight, padding);
    }
}