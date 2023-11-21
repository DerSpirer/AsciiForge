using System.Runtime.InteropServices;
using System.Text;

namespace AsciiForge.Engine
{
    public static class Screen
    {
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        public static Canvas canvas { get; private set; }
        private static Canvas? _prevCanvas;
        private static int _width = 120;
        public static int width { get { return _width; } }
        private static int _height = 30;
        public static int height { get { return _height; } }

        internal static void Init()
        {
            IntPtr consoleHandle = GetConsoleWindow();
            IntPtr menuHandle = GetSystemMenu(consoleHandle, false);
            if (consoleHandle != IntPtr.Zero && menuHandle != IntPtr.Zero)
            {
                const int MF_BYCOMMAND = 0x00000000;
                //const int SC_CLOSE = 0xF060;
                //const int SC_MINIMIZE = 0xF020;
                const int SC_MAXIMIZE = 0xF030;
                const int SC_SIZE = 0xF000;
                //DeleteMenu(menuHandle, SC_CLOSE, MF_BYCOMMAND);
                //DeleteMenu(menuHandle, SC_MINIMIZE, MF_BYCOMMAND);
                DeleteMenu(menuHandle, SC_MAXIMIZE, MF_BYCOMMAND);
                DeleteMenu(menuHandle, SC_SIZE, MF_BYCOMMAND);
            }
            Console.OutputEncoding = Encoding.UTF8;
            Console.CursorVisible = false;
            Console.SetWindowSize(_width, _height);
            
            _prevCanvas = null;
            canvas = new Canvas(_width, _height);
        }
        internal static void Clear()
        {
            canvas.Clear();
        }
        internal static void Draw()
        {
            List<PrintCommand> printCommands = GetPrintCommands();
            foreach (PrintCommand printCommand in printCommands)
            {
                Console.ForegroundColor = printCommand.foregroundColor;
                Console.BackgroundColor = printCommand.backgroundColor;
                Console.SetCursorPosition(printCommand.x, printCommand.y);
                Console.Write(printCommand.text);
            }
            _prevCanvas = new Canvas(canvas);
        }
        public static void SetScreenSize(int width, int height)
        {
            if (width == _width && height == _height)
            {
                return;
            }
            _width = width;
            _height = height;
            Console.SetWindowSize(_width, _height);

            _prevCanvas = null;
            canvas = new Canvas(_width, _height);
        }

        private struct PrintCommand
        {
            public int x;
            public int y;
            public string text;
            public ConsoleColor foregroundColor;
            public ConsoleColor backgroundColor;
            public PrintCommand(int x, int y, string text, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
            {
                this.x = x;
                this.y = y;
                this.text = text;
                this.foregroundColor = foregroundColor;
                this.backgroundColor = backgroundColor;
            }
        }
        private static List<PrintCommand> GetPrintCommands()
        {
            List<PrintCommand> commands = new List<PrintCommand>();
            
            char[] text = canvas.text.Cast<char>().ToArray();
            ConsoleColor[] foregroundColors = canvas.foregroundColors.Cast<ConsoleColor>().ToArray();
            ConsoleColor[] backgroundColors = canvas.backgroundColors.Cast<ConsoleColor>().ToArray();
            string textString = new string(text);

            if (_prevCanvas == null || _prevCanvas.width != canvas.width || _prevCanvas.height != canvas.height)
            {
                // Canvas changed size - simply print all canvas
                int commandIndex = 0;
                for (int i = 0; i < _width * _height; i++)
                {
                    if (foregroundColors[i] != foregroundColors[commandIndex] || backgroundColors[i] != backgroundColors[commandIndex])
                    {
                        commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..i], foregroundColors[commandIndex], backgroundColors[commandIndex]));
                        commandIndex = i;
                    }
                }
                commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..], foregroundColors[commandIndex], backgroundColors[commandIndex]));
            }
            else
            {
                char[] prevText = _prevCanvas.text.Cast<char>().ToArray();
                ConsoleColor[] prevForegroundColors = _prevCanvas.foregroundColors.Cast<ConsoleColor>().ToArray();
                ConsoleColor[] prevBackgroundColors = _prevCanvas.backgroundColors.Cast<ConsoleColor>().ToArray();

                int commandIndex = 0;
                for (int i = 0; i < _width * _height; i++)
                {
                    if (text[i] == prevText[i] && foregroundColors[i] == prevForegroundColors[i] && backgroundColors[i] == prevBackgroundColors[i])
                    {
                        if (i > commandIndex)
                        {
                            commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..i], foregroundColors[commandIndex], backgroundColors[commandIndex]));
                        }
                        commandIndex = i + 1;
                    }
                    else if (foregroundColors[i] != foregroundColors[commandIndex] || backgroundColors[i] != backgroundColors[commandIndex])
                    {
                        commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..i], foregroundColors[commandIndex], backgroundColors[commandIndex]));
                        commandIndex = i;
                    }
                }
                if (commandIndex < _width * _height)
                {
                    commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..], foregroundColors[commandIndex], backgroundColors[commandIndex]));
                }
            }

            return commands;
        }
    }
}
