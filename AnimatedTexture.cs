using MelonLoader;
using System.Collections;
using UnityEngine;

namespace mszcubemod
{
    public class AnimatedTexture
    {
        private readonly Texture2D[] _frames;
        private readonly float[] _durations;

        private int _currentFrame;

        public Texture2D Texture => _frames[_currentFrame];

        public event Action? Updated;

        public AnimatedTexture(Texture2D[] frames, float[] durations)
        {
            if (frames.Length < 2)
                throw new ArgumentException("Animation must contain at least two frames.");

            if (frames.Length != durations.Length)
                throw new ArgumentException(
                    "There must be one duration for every frame.");

            if (durations.Any(duration => duration <= 0))
                throw new ArgumentException(
                    "All animation frame durations must be greater than zero.");

            _frames = frames.ToArray();
            _durations = durations.ToArray();
        }

        internal void Start()
        {
            MelonCoroutines.Start(Animate());
        }

        private IEnumerator Animate()
        {
            while (true)
            {
                yield return new WaitForSeconds(_durations[_currentFrame]);
                _currentFrame = (_currentFrame + 1) % _frames.Length;
                Updated?.Invoke();
            }
        }
    }
}
