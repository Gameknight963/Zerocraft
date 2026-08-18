using UnityEngine;

namespace mszcubemod.Textures
{
    public class UniformAnimatedBlockTexture : IAnimatedBlockTexture
    {
        public UniformAnimatedBlockTexture(AnimatedTexture texture)
        {
            _animatedTexture = texture;
            texture.Updated += () => Updated?.Invoke();
            texture.Start();
        }

        private readonly AnimatedTexture _animatedTexture;

        public event Action? Updated;

        public Texture2D Up => _animatedTexture.Texture;
        public Texture2D Down => _animatedTexture.Texture;
        public Texture2D North => _animatedTexture.Texture;
        public Texture2D South => _animatedTexture.Texture;
        public Texture2D East => _animatedTexture.Texture;
        public Texture2D West => _animatedTexture.Texture;
    }
}
