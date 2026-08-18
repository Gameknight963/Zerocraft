namespace mszcubemod
{
    public readonly struct AnimatedBlockFaceTextures
    {
        public readonly AnimatedTexture Up;
        public readonly AnimatedTexture Down;
        public readonly AnimatedTexture North;
        public readonly AnimatedTexture South;
        public readonly AnimatedTexture East;
        public readonly AnimatedTexture West;

        public AnimatedBlockFaceTextures(
            AnimatedTexture up,
            AnimatedTexture down,
            AnimatedTexture north,
            AnimatedTexture south,
            AnimatedTexture east,
            AnimatedTexture west)
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
