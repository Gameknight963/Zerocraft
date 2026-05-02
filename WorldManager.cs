using UnityEngine;

namespace mszcubemod
{
    class WorldManager
    {
        public static WorldManager Instance { get; private set; } = null!;

        readonly List<Block> blocks;
        readonly Dictionary<Vector3, PlacedBlock> placedBlocks = new();

        public IReadOnlyDictionary<Vector3, PlacedBlock> PlacedBlocks => placedBlocks;

        public WorldManager(List<Block> blocks)
        {
            if (Instance != null) throw new InvalidOperationException("WorldManager already exists.");
            Instance = this;
            this.blocks = blocks;
        }

        public void PlaceBlock(string blockId, Vector3 position)
        {
            if (blocks.FirstOrDefault(b => b.Id == blockId) is not Block block) return;
            GameObject obj = Core.CreateCube(position, block.Texture, block.Size);
            placedBlocks[position] = new PlacedBlock(blockId, position, obj);
        }

        public void DeleteBlock(Vector3 position)
        {
            if (!placedBlocks.Remove(position, out PlacedBlock? placed)) return;
            GameObject.Destroy(placed.Object);
        }
    }
}