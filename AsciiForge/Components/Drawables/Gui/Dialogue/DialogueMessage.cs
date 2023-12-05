namespace AsciiForge.Components.Drawables.Gui.Dialogue;

public class DialogueMessage : TextBox
{
    private string _fullText = string.Empty;
    public string fullText
    {
        get
        {
            return _fullText;
        }
        set
        {
            if (_fullText != value)
            {
                _fullText = value;
                Reset();
            }
        }
    }
    public bool displayInstantly { get; set; } = false;
    public float incDisplayDelay { get; set; } = 0.1f;
    
    private int _displayCount;
    private float _incDelayTimer;
    
    protected void Start()
    {
        this.isGui = true;
        Reset();
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
            _displayCount = Math.Min(_displayCount + 1, fullText.Length);
            text = fullText[.._displayCount];
            if (_displayCount < fullText.Length)
            {
                _incDelayTimer = incDisplayDelay;
            }
        }
    }

    public void Reset()
    {
        if (displayInstantly)
        {
            _displayCount = fullText.Length;
            text = fullText;
        }
        else
        {
            _displayCount = 0;
            text = string.Empty;
            _incDelayTimer = incDisplayDelay;
        }
    }
}