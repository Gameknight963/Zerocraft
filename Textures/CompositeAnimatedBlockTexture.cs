using UnityEngine;

namespace mszcubemod.Textures
{
    public class CompositeAnimatedBlockTexture : IAnimatedBlockTexture
    {
        public Texture2D Up => _up.Texture;
        public Texture2D Down => _down.Texture;
        public Texture2D North => _north.Texture;
        public Texture2D South => _south.Texture;
        public Texture2D East => _east.Texture;
        public Texture2D West => _west.Texture;

        public event Action? Updated;

        private readonly AnimatedTexture _up;
        private readonly AnimatedTexture _down;
        private readonly AnimatedTexture _north;
        private readonly AnimatedTexture _south;
        private readonly AnimatedTexture _east;
        private readonly AnimatedTexture _west;

        public CompositeAnimatedBlockTexture(
            AnimatedTexture up,
            AnimatedTexture down,
            AnimatedTexture north,
            AnimatedTexture south,
            AnimatedTexture east,
            AnimatedTexture west)
        {
            _up = up;
            _down = down;
            _north = north;
            _south = south;
            _east = east;
            _west = west;
            _up.Updated += () => Updated?.Invoke();
            _down.Updated += () => Updated?.Invoke();
            _north.Updated += () => Updated?.Invoke();
            _south.Updated += () => Updated?.Invoke();
            _east.Updated += () => Updated?.Invoke();
            _west.Updated += () => Updated?.Invoke();
            _up.Start();
            _down.Start();
            _north.Start();
            _south.Start();
            _east.Start();
            _west.Start();
        }
    }
}
