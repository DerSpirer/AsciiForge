namespace AsciiForge.Resources
{
    internal class ResourceMissingException : Exception
    {
        public ResourceMissingException() { }
        public ResourceMissingException(string message) : base(message) { }
    }
}
