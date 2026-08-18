using UnityEngine;

namespace mszcubemod.Textures
{
    public class CompositeBlockTexture : IBlockTexture
    {
        public Texture2D Up { get; }
        public Texture2D Down { get; }
        public Texture2D North { get; }
        public Texture2D South { get; }
        public Texture2D East { get; }
        public Texture2D West { get; }

        public CompositeBlockTexture(
            Texture2D up,
            Texture2D down,
            Texture2D north,
            Texture2D south,
            Texture2D east,
            Texture2D west)
        {
            Up = up;
            Down = down;
            North = north;
            South = south;
            East = east;
            West = west;
        }
    }
}
