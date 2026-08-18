using UnityEngine;

namespace mszcubemod.Textures
{
    public interface IBlockTexture
    {
        Texture2D Up { get; }
        Texture2D Down { get; }
        Texture2D North { get; }
        Texture2D South { get; }
        Texture2D East { get; }
        Texture2D West { get; }
    }
}
