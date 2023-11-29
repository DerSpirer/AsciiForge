using AsciiForge.Engine.Resources;
using NAudio.Wave;

namespace AsciiForge.Engine.Audio
{
    internal class SoundResourceSampleProvider : ISampleProvider
    {
        internal readonly SoundResource _sound;
        private int _position;

        public bool isLooping;
        public event EventHandler? OnEnd;
        public event EventHandler? OnDone;
        public WaveFormat WaveFormat { get { return _sound.waveFormat; } }

        public SoundResourceSampleProvider(SoundResource sound)
        {
            this._sound = sound;
        }

        public int Read(float[] buffer, int offset, int count)
        {
            if (isLooping)
            {
                for (int i = 0; i < count; i++)
                {
                    buffer[offset + i] = _sound.audioData[(_position + i) % _sound.audioData.Length];
                }
                _position += count;
                if (_position >= _sound.audioData.Length)
                {
                    _position %= _sound.audioData.Length;
                    OnEnd?.Invoke(this, EventArgs.Empty);
                }
            }
            else
            {
                int samplesLeft = _sound.audioData.Length - _position;
                if (samplesLeft > 0)
                {
                    int samplesToCopy = Math.Min(samplesLeft, count);
                    Array.Copy(_sound.audioData, _position, buffer, offset, samplesToCopy);
                    for (int i = samplesToCopy; i < count; i++)
                    {
                        buffer[offset + i] = 0;
                    }
                    _position += samplesToCopy;
                    if (_position >= _sound.audioData.Length)
                    {
                        OnEnd?.Invoke(this, EventArgs.Empty);
                        OnDone?.Invoke(this, EventArgs.Empty);
                    }
                }
            }
            return count;
        }

        public void Reset()
        {
            _position = 0;
        }
        public bool IsDone() => _position >= _sound.audioData.Length;
    }
}
