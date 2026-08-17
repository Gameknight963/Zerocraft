using UnityEngine;

namespace mszcubemod
{
    public class CompositeBlockTexture : IBlockTexture
    {
        public Texture2D Top { get; }
        public Texture2D Bottom { get; }
        public Texture2D Front { get; }
        public Texture2D Back { get; }
        public Texture2D Left { get; }
        public Texture2D Right { get; }

        public CompositeBlockTexture(
            Texture2D top,
            Texture2D bottom,
            Texture2D front,
            Texture2D back,
            Texture2D left,
            Texture2D right)
        {
            Top = top;
            Bottom = bottom;
            Front = front;
            Back = back;
            Left = left;
            Right = right;
        }
    }
}
