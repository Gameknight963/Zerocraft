using UnityEngine;

namespace mszcubemod
{
    public class UniformBlockTexture : IBlockTexture
    {
        private readonly Texture2D _texture;

        public UniformBlockTexture(Texture2D texture)
        {
            _texture = texture;
        }

        public Texture2D Up => _texture;
        public Texture2D Down => _texture;
        public Texture2D North => _texture;
        public Texture2D South => _texture;
        public Texture2D East => _texture;
        public Texture2D West => _texture;
    }
}
