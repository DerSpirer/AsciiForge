namespace AsciiForge.Engine.Resources
{
    internal class ResourceFormatException : Exception
    {
        public ResourceFormatException() : base() { }
        public ResourceFormatException(string message) : base(message) { }
    }
}
