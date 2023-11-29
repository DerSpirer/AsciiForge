using System.Text.Json.Serialization;

namespace AsciiForge.Engine.Resources
{
    public class InstanceResource : Resource
    {
        private readonly string _species;
        public string species { get { return _species; } }
        private readonly ComponentResource[] _components;
        public ComponentResource[] components { get { return _components; } }

        [JsonConstructor]
        public InstanceResource(string species, ComponentResource[] components)
        {
            _species = species;
            _components = components;

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

            if (!ResourceManager.entities.ContainsKey(_species))
            {
                error = $"Referencing non-existing species '{_species}' in room resource";
                return (isValid, error);
            }

            isValid = true;
            return (isValid, error);
        }
    }
}
