using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace AsciiForge.Engine.Audio
{
    internal class ChannelsAdapterSampleProvider : ISampleProvider
    {
        private readonly ISampleProvider _provider;
        public ChannelsAdapterSampleProvider(ISampleProvider provider)
        {
            if (provider.WaveFormat.Channels == AudioManager.channelCount)
            {
                _provider = provider;
                return;
            }
            if (provider.WaveFormat.Channels == 1 && AudioManager.channelCount == 2)
            {
                _provider = new MonoToStereoSampleProvider(provider);
                return;
            }
            throw new NotImplementedException("Not yet implemented this channel count conversion");
        }

        public WaveFormat WaveFormat
        {
            get
            {
                return _provider.WaveFormat;
            }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            return _provider.Read(buffer, offset, count);
        }
    }
}
