using System.Text;
using AsciiForge.Engine.IO;

namespace AsciiForge.Components.Drawables.Gui.Dialogue;

public class DialogueChoice : TextBox
{
    private readonly List<string> _choices = new List<string>();
    public List<string> choices
    {
        get
        {
            return _choices;
        }
        set
        {
            if (_choices != value)
            {
                if (value == null)
                {
                    throw new Exception("Choices array must not be null");
                }
                _choices.Clear();
                _choices.AddRange(value);
                BuildText();
            }
        }
    }
    private int _currIndex = 0;
    public int currIndex
    {
        get
        {
            return _currIndex;
        }
        set
        {
            if (_currIndex != value)
            {
                if (value < 0 || value >= _choices.Count)
                {
                    throw new Exception("Current index cannot be outside choices list range");
                }
                _currIndex = value;
                BuildText();
            }
        }
    }
    public string choice
    {
        get
        {
            return _choices[currIndex];
        }
        set
        {
            int index = _choices.IndexOf(choice);
            if (index < 0)
            {
                throw new Exception($"Choice {value} does not exist in choices list");
            }
            currIndex = index;
        }
    }
    public event EventHandler? onChoice;
    
    protected void Start()
    {
        this.isGui = true;
        BuildText();
    }
    private void Update(float deltaTime)
    {
        if (Input.IsKeyPressed(Input.Key.UpArrow))
        {
            _currIndex = Math.Max(_currIndex - 1, 0);
            BuildText();
        }
        if (Input.IsKeyPressed(Input.Key.DownArrow))
        {
            _currIndex = Math.Min(_currIndex + 1, _choices.Count - 1);
            BuildText();
        }
        if (Input.IsKeyReleased(Input.Key.Enter))
        {
            onChoice?.Invoke(this, EventArgs.Empty);
        }
    }
    
    private void BuildText()
    {
        if (_choices.Count == 0)
        {
            text = string.Empty;
            return;
        }
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < currIndex; i++)
        {
            builder.AppendLine(_choices[i]);
        }
        builder.AppendLine("> " + _choices[currIndex]);
        for (int i = currIndex + 1; i < _choices.Count; i++)
        {
            builder.AppendLine(_choices[i]);
        }
        text = builder.ToString().Trim();
    }
}