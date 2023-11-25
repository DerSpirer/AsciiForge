namespace AsciiForge.Resources
{
    internal class MissingResourceException : Exception
    {
        public MissingResourceException() { }
        public MissingResourceException(string message) : base(message) { }
    }
}
