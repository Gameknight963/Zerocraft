using mszcubemod.Textures;
using UnityEngine;

namespace mszcubemod.World
{
    public class Block
    {
        public static readonly Vector3 DefaultCubeSize = new(1f, 1f, 1f);

        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public Vector3 Size { get; set; } = new Vector3(0.5f, 0.5f, 0.5f);

        public IBlockTexture Texture { get; private set; }

        public Block(string id, string name, IBlockTexture texture, Vector3? size = null)
        {
            Size = size ?? DefaultCubeSize;
            Id = id;
            Name = name;
            Texture = texture;
        }
    }
}
