using UnityEngine;

namespace mszcubemod
{
    class WorldManager
    {
        public static WorldManager Instance { get; } = new();

        readonly List<Block> _blocks = new();
        readonly Dictionary<Vector3, PlacedBlock> placedBlocks = new();

        public IReadOnlyList<Block> Blocks { get; }
        public IReadOnlyDictionary<Vector3, PlacedBlock> PlacedBlocks => placedBlocks;

        WorldManager()
        {
            Blocks = _blocks.AsReadOnly();
        }

        public void Initialize(List<Block> blocks)
        {
            _blocks.AddRange(blocks);
        }

        public void PlaceBlock(string blockId, Vector3 position)
        {
            if (Blocks.FirstOrDefault(b => b.Id == blockId) is not Block block) return;
            GameObject obj = Core.CreateCube(position, block.Texture, block.Size);
            placedBlocks[position] = new PlacedBlock(blockId, position, obj);
        }

        public void DeleteBlock(Vector3 position)
        {
            if (!placedBlocks.Remove(position, out PlacedBlock? placed)) return;
            GameObject.Destroy(placed.Object);
        }

        public void RegisterBlock(Block block)
        {
            if (_blocks.Any(b => b.Id == block.Id))
                throw new InvalidOperationException($"Block '{block.Id}' is already registered.");
            _blocks.Add(block);
        }
    }
}