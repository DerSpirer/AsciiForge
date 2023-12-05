namespace AsciiForge.Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class RequireComponentAttribute : Attribute
    {
        public Type componentType;

        public RequireComponentAttribute(Type componentType)
        {
            this.componentType = componentType;
        }
    }
}
