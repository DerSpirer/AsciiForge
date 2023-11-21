using System.Text.Json.Serialization;

namespace AsciiForge.Resources
{
    public class SoundResource : Resource
    {
        private readonly string _audioFile;
        public string audioFile { get { return _audioFile; } }
        private readonly float _pitch;
        public float pitch { get { return _pitch; } }
        private readonly float _volume;
        public float volume { get { return _volume; } }

        [JsonConstructor]
        public SoundResource(string audioFile, float pitch, float volume)
        {
            this._audioFile = audioFile;
            this._pitch = pitch;
            this._volume = Math.Clamp(volume, 0, 1);

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
                error = $"";
                return (isValid, error);
            }

            isValid = true;
            return (isValid, error);
        }
    }
}
