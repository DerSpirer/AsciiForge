﻿using System.Text.Json.Serialization;

namespace AsciiForge.Engine.Resources
{
    public class EntityResource : Resource
    {
        [JsonIgnore]
        public string species { get; internal set; }
        private readonly ComponentResource[] _components;
        public ComponentResource[] components { get { return _components; } }

        [JsonConstructor]
        public EntityResource(ComponentResource[] components)
        {
            species = string.Empty;
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

            isValid = true;
            return (isValid, error);
        }
    }
}