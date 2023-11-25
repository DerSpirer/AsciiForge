using AsciiForge.Engine;
using AsciiForge.Resources;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using AsciiForge.Helpers;
using System.Text.Json.Serialization;

namespace AsciiForge.Components
{
    public class Sound : Component
    {
        private WaveOutEvent _device;

        private string _sound;
        public string sound
        {
            get
            {
                return _sound;
            }
            set
            {
                if (string.IsNullOrEmpty(value) || !ResourceManager.sounds.ContainsKey(value))
                {
                    throw new MissingResourceException($"Trying to set a sound to non-existing sound resource: {sound}");
                }
                if (_device == null || playbackState == PlaybackState.Stopped)
                {
                    _sound = value;
                }
            }
        }
        private float _pitch = 1;
        public float pitch
        {
            get
            {
                return _pitch;
            }
            set
            {
                if (_device == null || playbackState == PlaybackState.Stopped)
                {
                    _pitch = value;
                }
            }
        }
        
        /// <summary>
        /// The maximum volume local sounds will play at, or the constant volume global sounds will play at
        /// </summary>
        public float maxVolume
        {
            get
            {
                return _maxVolume;
            }
            set
            {
                if (value != _maxVolume)
                {
                    _maxVolume = value;
                    UpdateVolume();
                }
            }
        }
        private float _maxVolume = 1;
        /// <summary>
        /// Local sounds will play at maximum volume until they get further away from this radius
        /// </summary>
        public float maxVolumeRadius
        {
            get
            {
                return _maxVolumeRadius;
            }
            set
            {
                if (value != _maxVolumeRadius)
                {
                    _maxVolumeRadius = value;
                    UpdateVolume();
                }
            }
        }
        private float _maxVolumeRadius = 0;
        /// <summary>
        /// The minimal volume local sounds will play at
        /// </summary>
        public float minVolume
        {
            get
            {
                return _minVolume;
            }
            set
            {
                if (value != _minVolume)
                {
                    _minVolume = value;
                    UpdateVolume();
                }
            }
        }
        private float _minVolume = 0;
        /// <summary>
        /// The radius at which local sounds will play at minimum volume
        /// </summary>
        public float minVolumeRadius
        {
            get
            {
                return _minVolumeRadius;
            }
            set
            {
                if (value != _minVolumeRadius)
                {
                    _minVolumeRadius = value;
                    UpdateVolume();
                }
            }
        }
        private float _minVolumeRadius = 60;
        /// <summary>
        /// Local sounds will play more quietly as they get further away from the camera, as opposed to global sounds will always play at max volume
        /// </summary>
        public bool isLocal
        {
            get
            {
                return _isLocal;
            }
            set
            {
                if (value != _isLocal)
                {
                    _isLocal = value;
                    UpdateVolume();
                }
            }
        }
        private bool _isLocal = false;

        [JsonIgnore]
        public PlaybackState playbackState { get { return _device.PlaybackState; } }

        private void Awake()
        {
            if (string.IsNullOrEmpty(sound) || !ResourceManager.sounds.ContainsKey(sound))
            {
                throw new MissingResourceException($"Trying to play a non-existing sound: {sound}");
            }
            _device = new WaveOutEvent();
            UpdateVolume();
        }
        private void Update(float deltaTime)
        {
            if (isLocal)
            {
                UpdateVolume();
            }
        }
        private void Destroy()
        {
            _device.Stop();
            _device.Dispose();
        }

        public void Play()
        {
            if (_device != null)
            {
                switch (playbackState)
                {
                    case PlaybackState.Playing:
                        Logger.Info($"Trying to play a sound that is already playing: {_sound}");
                        break;
                    case PlaybackState.Paused:
                        _device.Play();
                        break;
                    case PlaybackState.Stopped:
                        try
                        {
                            // NOAM TODO Remove AudioResource, it should just be an audio file, no need for extra properties
                            using MediaFoundationReader reader = new MediaFoundationReader(ResourceManager.sounds[sound].audioFile);
                            ISampleProvider provider = new SmbPitchShiftingSampleProvider(reader.ToSampleProvider())
                            {
                                PitchFactor = _pitch
                            };

                            _device.Init(provider);
                            _device.Play();
                        }
                        catch (Exception exception)
                        {
                            Logger.Error($"Failed to play sound: {sound}", exception);
                        }
                        break;
                }
            }
        }
        public void Pause()
        {
            if (_device != null)
            {
                _device.Pause();
            }
        }
        public void Stop()
        {
            if (_device != null)
            {
                _device.Stop();
            }
        }

        private void UpdateVolume()
        {
            if (_device != null)
            {
                if (_isLocal)
                {
                    float distance = Math.Clamp((transform.position - Game.world.camera.transform.position).length, maxVolumeRadius, minVolumeRadius);
                    float percentage = (distance - maxVolumeRadius) / (minVolumeRadius - maxVolumeRadius);
                    percentage = 1 - percentage;
                    _device.Volume = MathExt.Lerp(minVolume, maxVolume, percentage);
                }
                else
                {
                    _device.Volume = _maxVolume;
                }
            }
        }
    }
}
