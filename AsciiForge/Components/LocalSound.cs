using AsciiForge.Engine;
using AsciiForge.Helpers;
using AsciiForge.Engine.Ecs;
using AsciiForge.Engine.Audio;
using AsciiForge.Engine.Resources;

namespace AsciiForge.Components
{
    public class LocalSound : Component
    {
        private Sound? _sound;
        public string? sound
        {
            get
            {
                if (_sound == null)
                {
                    return string.Empty;
                }
                return ResourceManager.sounds.Single(s => s.Value == _sound.soundProvider._sound).Key;
            }
            set
            {
                if (sound != value)
                {
                    if (!string.IsNullOrEmpty(value) && !ResourceManager.sounds.ContainsKey(value))
                    {
                        throw new MissingResourceException($"Trying to set sound of LocalSound component to non-existing sound resource: {value}");
                    }
                    if (_sound != null)
                    {
                        AudioManager.Stop(_sound);
                    }
                    if (!string.IsNullOrEmpty(value))
                    {
                        _sound = new Sound(value);
                        _sound.OnEnd += (sender, e) => OnEnd?.Invoke(this, e);
                        _sound.OnDone += (sender, e) => OnDone?.Invoke(this, e);
                        if (playOnStart)
                        {
                            AudioManager.Play(_sound);
                        }
                    }
                    else
                    {
                        _sound = null;
                    }
                }
            }
        }
        /// <summary>
        /// The maximum volume the sound will play at
        /// </summary>
        public float maxVolume { get; set; } = 1;
        /// <summary>
        /// The sound will play at maximum volume until it gets further away than this radius
        /// </summary>
        public float maxVolumeRadius { get; set; } = 0;
        /// <summary>
        /// The minimal volume the sound will play at
        /// </summary>
        public float minVolume { get; set; } = 0;
        /// <summary>
        /// The radius at which the sound start to play at minimal volume
        /// </summary>
        public float minVolumeRadius { get; set; } = 60;
        public bool playOnStart { get; set; } = true;
        public float? pitch
        {
            get
            {
                return _sound?.pitch;
            }
            set
            {
                if (_sound != null && value != _sound.pitch)
                {
                    if (value != null)
                    {
                        _sound.pitch = (float)value;
                    }
                }
            }
        }
        public float? volume
        {
            get
            {
                return _sound?.volume;
            }
            set
            {
                if (_sound != null && value != _sound.volume)
                {
                    if (value != null)
                    {
                        _sound.volume = Math.Clamp((float)value, 0, 1);
                    }
                }
            }
        }
        public bool? isLooping
        {
            get
            {
                return _sound?.isLooping;
            }
            set
            {
                if (_sound != null && value != _sound.isLooping)
                {
                    if (value != null)
                    {
                        _sound.isLooping = (bool)value;
                    }
                }
            }
        }
        public event EventHandler? OnEnd;
        public event EventHandler? OnDone;

        private void Awake()
        {
            if (_sound != null)
            {
                UpdateVolume();
            }
        }
        private void Update(float deltaTime)
        {
            if (_sound != null)
            {
                UpdateVolume();
            }
        }
        private void Destroy()
        {
            if (_sound != null)
            {
                AudioManager.Stop(_sound);
            }
        }
        private void UpdateVolume()
        {
            float distance = Math.Clamp((transform.position - Game.world.camera.transform.position).length, maxVolumeRadius, minVolumeRadius);
            float percentage = (distance - maxVolumeRadius) / (minVolumeRadius - maxVolumeRadius);
            percentage = 1 - percentage;
            _sound!.volume = MathExt.Lerp(minVolume, maxVolume, percentage);
        }

        public void Play()
        {
            if (_sound != null)
            {
                AudioManager.Play(_sound);
            }
        }
        public void Pause()
        {
            if (_sound != null)
            {
                AudioManager.Pause(_sound);
            }
        }
        public void Stop()
        {
            if (_sound != null)
            {
                AudioManager.Stop(_sound);
            }
        }
    }
}
