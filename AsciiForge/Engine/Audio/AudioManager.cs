using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AsciiForge.Engine.Audio
{
    public static class AudioManager
    {
        // https://markheath.net/post/fire-and-forget-audio-playback-with <3
        private static readonly IWavePlayer _outputDevice;
        private static readonly MixingSampleProvider _mixer;

        internal const int sampleRate = 44100;
        internal const int channelCount = 2;

        static AudioManager()
        {
            _outputDevice = new WaveOutEvent();
            _mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount))
            {
                ReadFully = true
            };
            _outputDevice.Init(_mixer);
            _outputDevice.Play();
        }

        public static void Play(Sound sound)
        {
            if (sound.IsDone())
            {
                sound.Reset();
            }
            if (_mixer.MixerInputs.Contains(sound.sampleProvider))
            {
                return;
            }
            sound.soundProvider.OnDone += (sender, e) => Stop(sound);
            _mixer.AddMixerInput(sound.sampleProvider);
        }
        public static void Pause(Sound sound)
        {
            if (!_mixer.MixerInputs.Contains(sound.sampleProvider))
            {
                return;
            }
            _mixer.RemoveMixerInput(sound.sampleProvider);
        }
        public static void Stop(Sound sound)
        {
            if (!_mixer.MixerInputs.Contains(sound.sampleProvider))
            {
                return;
            }
            _mixer.RemoveMixerInput(sound.sampleProvider);
            sound.Reset();
        }

        internal static void Dispose()
        {
            _outputDevice.Dispose();
        }
    }
}
