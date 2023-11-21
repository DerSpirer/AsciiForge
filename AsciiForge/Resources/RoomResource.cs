using System.Text.Json.Serialization;

namespace AsciiForge.Resources
{
    public class RoomResource : Resource
    {
        private readonly InstanceResource[] _instances;
        public InstanceResource[] instances { get { return _instances; } }

        [JsonConstructor]
        public RoomResource(InstanceResource[] instances)
        {
            this._instances = instances;

            (bool isValid, string error) = IsValid();
            if (!isValid)
            {
                throw new ResourceFormatException(error);
            }
        }

        protected override (bool, string) IsValid()
        {
            bool isValid = false;
            string error = string.Empty;

            isValid = true;
            return (isValid, error);
        }
    }
}
