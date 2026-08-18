namespace mszcubemod
{
    public interface IAnimatedBlockTexture : IBlockTexture
    {
        public event Action? Updated;
    }
}
