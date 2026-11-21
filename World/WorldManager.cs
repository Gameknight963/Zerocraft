using MelonLoader;
using mszcubemod.Rendering;
using UnityEngine;

namespace mszcubemod.World
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
            GameObject obj = CubeFactory.CreateCube(position, block.Texture, block.Size, block.Transparency);
            placedBlocks[position] = new PlacedBlock(blockId, position, obj);
        }

        public void DeleteBlock(Vector3 position)
        {
            if (!placedBlocks.TryGetValue(position, out PlacedBlock placed))
            {
                MelonLogger.Warning($"Attempted to delete non-existant block at {position}!");
            }
            placedBlocks.Remove(position);
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