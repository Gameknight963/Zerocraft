namespace mszcubemod.Textures
{
    public interface IAnimatedBlockTexture : IBlockTexture
    {
        public event Action? Updated;
    }
}
