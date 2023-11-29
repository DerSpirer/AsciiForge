using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;
using AsciiForge.Engine.Ecs;

namespace AsciiForge.Engine.IO
{
    public static class Screen
    {
        // Colors
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool GetConsoleMode(IntPtr handle, out int mode);
        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetStdHandle(int handle);

        // Delete window menu
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
            Console.Clear();
            
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
            foreach (PrintCommand cmd in printCommands)
            {
                Console.SetCursorPosition(cmd.x, cmd.y);
                Console.Write($"\x1b[48;2;{cmd.bg.R};{cmd.bg.G};{cmd.bg.B}m\x1b[38;2;{cmd.fg.R};{cmd.fg.G};{cmd.fg.B}m{cmd.text}");
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
            public Color fg;
            public Color bg;
            public PrintCommand(int x, int y, string text, Color fg, Color bg)
            {
                this.x = x;
                this.y = y;
                this.text = text;
                this.fg = fg;
                this.bg = bg;
            }
        }
        private static List<PrintCommand> GetPrintCommands()
        {
            List<PrintCommand> commands = new List<PrintCommand>();
            
            char[] text = canvas.text.Cast<char>().ToArray();
            Color[] fg = canvas.fg.Cast<Color>().ToArray();
            Color[] bg = canvas.bg.Cast<Color>().ToArray();
            string textString = new string(text);

            if (_prevCanvas == null || _prevCanvas.width != canvas.width || _prevCanvas.height != canvas.height)
            {
                // Canvas changed size - simply print all canvas
                int commandIndex = 0;
                for (int i = 0; i < _width * _height; i++)
                {
                    if (fg[i] != fg[commandIndex] || bg[i] != bg[commandIndex])
                    {
                        commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..i], fg[commandIndex], bg[commandIndex]));
                        commandIndex = i;
                    }
                }
                commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..], fg[commandIndex], bg[commandIndex]));
            }
            else
            {
                char[] prevText = _prevCanvas.text.Cast<char>().ToArray();
                Color[] prevFg = _prevCanvas.fg.Cast<Color>().ToArray();
                Color[] prevBg = _prevCanvas.bg.Cast<Color>().ToArray();

                int commandIndex = 0;
                for (int i = 0; i < _width * _height; i++)
                {
                    if (text[i] == prevText[i] && fg[i] == prevFg[i] && bg[i] == prevBg[i])
                    {
                        if (i > commandIndex)
                        {
                            commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..i], fg[commandIndex], bg[commandIndex]));
                        }
                        commandIndex = i + 1;
                    }
                    else if (fg[i] != fg[commandIndex] || bg[i] != bg[commandIndex])
                    {
                        commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..i], fg[commandIndex], bg[commandIndex]));
                        commandIndex = i;
                    }
                }
                if (commandIndex < _width * _height)
                {
                    commands.Add(new PrintCommand(commandIndex % _width, commandIndex / _width, textString[commandIndex..], fg[commandIndex], bg[commandIndex]));
                }
            }

            return commands;
        }
    }
}
