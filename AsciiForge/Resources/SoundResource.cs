using System.Text.Json.Serialization;

namespace AsciiForge.Resources
{
    public class SoundResource : Resource
    {
        private readonly string _audioFile;
        public string audioFile { get { return _audioFile; } }

        [JsonConstructor]
        public SoundResource(string audioFile)
        {
            this._audioFile = Path.Combine(Directory.GetCurrentDirectory(), audioFile);

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

            if (!File.Exists(_audioFile))
            {
                error = $"Failed to load sound resource pointing to a non-existing sound file: {_audioFile}";
                return (isValid, error);
            }

            isValid = true;
            return (isValid, error);
        }
    }
}
