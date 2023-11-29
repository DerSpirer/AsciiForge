using AsciiForge.Engine.Resources;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AsciiForge.Engine.Audio
{
    public class Sound
    {
        internal readonly SoundResourceSampleProvider soundProvider;
        private readonly ChannelsAdapterSampleProvider _channelsAdapterProvider;
        private readonly SmbPitchShiftingSampleProvider _pitchProvider;
        private readonly VolumeSampleProvider _volumeProvider;
        internal ISampleProvider sampleProvider { get { return _volumeProvider; } }
        public float pitch
        {
            get
            {
                return _pitchProvider.PitchFactor;
            }
            set
            {
                if (pitch != value)
                {
                    _pitchProvider.PitchFactor = value;
                }
            }
        }
        public float volume
        {
            get
            {
                return _volumeProvider.Volume;
            }
            set
            {
                if (volume != value)
                {
                    _volumeProvider.Volume = value;
                }
            }
        }
        public bool isLooping
        {
            get
            {
                return soundProvider.isLooping;
            }
            set
            {
                soundProvider.isLooping = value;
            }
        }
        public event EventHandler? OnEnd;
        public event EventHandler? OnDone;

        public Sound(string sound, float pitch = 1, float volume = 1, bool isLooping = false)
        {
            if (string.IsNullOrEmpty(sound) || !ResourceManager.sounds.ContainsKey(sound))
            {
                throw new MissingResourceException($"Trying to initialize a sound with a non-existing sound resource: {sound}");
            }
            soundProvider = new SoundResourceSampleProvider(ResourceManager.sounds[sound]) { isLooping = isLooping };
            _channelsAdapterProvider = new ChannelsAdapterSampleProvider(soundProvider);
            _pitchProvider = new SmbPitchShiftingSampleProvider(_channelsAdapterProvider) { PitchFactor = pitch };
            _volumeProvider = new VolumeSampleProvider(_pitchProvider) { Volume = volume };

            soundProvider.OnEnd += (sender, e) => OnEnd?.Invoke(this, e);
            soundProvider.OnDone += (sender, e) => OnDone?.Invoke(this, e);
        }
        public void Reset()
        {
            soundProvider.Reset();
        }
        public bool IsDone() => soundProvider.IsDone();
    }
}
