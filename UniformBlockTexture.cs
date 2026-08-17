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
        public Texture2D Top => _texture;
        public Texture2D Bottom => _texture;
        public Texture2D Front => _texture;
        public Texture2D Back => _texture;
        public Texture2D Left => _texture;
        public Texture2D Right => _texture;

        public static UniformBlockTexture CreateFromFile(string texturePath)
        {
            if (!File.Exists(texturePath)) throw new FileNotFoundException($"{texturePath}: No such file");
            return new UniformBlockTexture(TextureLoader.Load(texturePath));
        }
    }
}
