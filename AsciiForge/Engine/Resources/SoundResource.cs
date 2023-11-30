using NAudio.Wave;

namespace AsciiForge.Engine.Resources
{
    public class SoundResource : Resource
    {
        public float[] audioData { get; private set; }
        public WaveFormat waveFormat { get; private set; }

        private SoundResource(float[] audioData, WaveFormat waveFormat)
        {
            this.audioData = audioData;
            this.waveFormat = waveFormat;

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
        
        public static async Task<SoundResource> Read(ResourceManager.ResourceFile resourceFile)
        {
            await using AudioFileReader reader = new AudioFileReader(resourceFile.path);
            
            List<float> data = new List<float>();
            float[] readBuffer = new float[reader.WaveFormat.SampleRate * reader.WaveFormat.Channels];
            int samplesRead = reader.Read(readBuffer, 0, readBuffer.Length);
            while (samplesRead > 0)
            {
                data.AddRange(readBuffer.Take(samplesRead));
                samplesRead = reader.Read(readBuffer, 0, readBuffer.Length);
            }
            
            float[] audioData = data.ToArray();
            WaveFormat waveFormat = reader.WaveFormat;
            return new SoundResource(audioData, waveFormat);
        }
    }
}
