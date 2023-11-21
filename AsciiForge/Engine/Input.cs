using System.Runtime.InteropServices;

namespace AsciiForge.Engine
{
    public static class Input
    {
        [DllImport("user32.dll")]
        private static extern int GetKeyboardState(byte[] state);
        [DllImport("user32.dll")]
        private static extern short GetKeyState(int key);

        private static byte[] _prevState = new byte[256];
        private static byte[] _currState = new byte[256];

        internal static void Read()
        {
            for (int i = 0; i < _currState.Length; i++)
            {
                _prevState[i] = _currState[i];
            }
            GetKeyState((int)Key.Shift);
            GetKeyboardState(_currState);
        }

        public static bool IsKeyDown(Key key) => (_currState[(int)key] & 0x80) != 0;
        public static bool IsKeyPressed(Key key) => (_currState[(int)key] & 0x80) != 0 && (_prevState[(int)key] & 0x80) == 0;
        public static bool IsKeyReleased(Key key) => (_currState[(int)key] & 0x80) == 0 && (_prevState[(int)key] & 0x80) != 0;

        public enum Key
        {
            Backspace = 0x08,
            Tab = 0x09,
            Enter = 0x0D,
            Shift = 0x10,
            Ctrl = 0x11,
            Alt = 0x12,
            CapsLock = 0x14,
            Escape = 0x1B,
            Space = 0x20,
            PageUp = 0x21,
            PageDown = 0x22,
            End = 0x23,
            Home = 0x24,
            LeftArrow = 0x25,
            UpArrow = 0x26,
            RightArrow = 0x27,
            DownArrow = 0x28,
            PrintScreen = 0x2C,
            Insert = 0x2D,
            Delete = 0x2E,
            _0 = 0x30,
            _1 = 0x31,
            _2 = 0x32,
            _3 = 0x33,
            _4 = 0x34,
            _5 = 0x35,
            _6 = 0x36,
            _7 = 0x37,
            _8 = 0x38,
            _9 = 0x39,
            A = 0x41,
            B = 0x42,
            C = 0x43,
            D = 0x44,
            E = 0x45,
            F = 0x46,
            G = 0x47,
            H = 0x48,
            I = 0x49,
            J = 0x4A,
            K = 0x4B,
            L = 0x4C,
            M = 0x4D,
            N = 0x4E,
            O = 0x4F,
            P = 0x50,
            Q = 0x51,
            R = 0x52,
            S = 0x53,
            T = 0x54,
            U = 0x55,
            V = 0x56,
            W = 0x57,
            X = 0x58,
            Y = 0x59,
            Z = 0x5A,
            LeftWindows = 0x5B,
            RightWindows = 0x5C,
            Numpad0 = 0x60,
            Numpad1 = 0x61,
            Numpad2 = 0x62,
            Numpad3 = 0x63,
            Numpad4 = 0x64,
            Numpad5 = 0x65,
            Numpad6 = 0x66,
            Numpad7 = 0x67,
            Numpad8 = 0x68,
            Numpad9 = 0x69,
            NumpadMultiply = 0x6A,
            NumpadAdd = 0x6B,
            NumpadSeperator = 0x6C,
            NumpadSubtract = 0x6D,
            NumpadDecimal = 0x6E,
            NumpadDivide = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,
            Numlock = 0x90,
            LeftShift = 0xA0,
            RightShift = 0xA1,
            LeftCtrl = 0xA2,
            RightCtrl = 0xA3,
            LeftAlt = 0xA4,
            RightAlt = 0xA5,
        }
    }
}
