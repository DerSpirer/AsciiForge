namespace AsciiForge.Engine.Resources
{
    internal class MissingResourceException : Exception
    {
        public MissingResourceException() { }
        public MissingResourceException(string message) : base(message) { }
    }
}
