using Newtonsoft.Json;
namespace mszcubemod
{
    public static class BlockLoader
    {
        public static List<Block> LoadAll()
        {
            List<Block> blocks = new();

            foreach (string filePath in Directory.GetFiles(Core.ModResources))
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext != ".png" && ext != ".jpg") continue;
                string id = Path.GetFileNameWithoutExtension(filePath);
                blocks.Add(new Block(id, id, new UniformBlockTexture(TextureLoader.Load(filePath))));
            }

            foreach (string dir in Directory.GetDirectories(Core.ModResources))
            {
                string id = Path.GetFileName(dir);
                IBlockTexture texture = LoadCompositeTexture(dir);

                string jsonPath = Path.Combine(dir, "blockdata.json");
                if (File.Exists(jsonPath))
                {
                    BlockData data = JsonConvert.DeserializeObject<BlockData>(File.ReadAllText(jsonPath))
                        ?? throw new InvalidOperationException($"{jsonPath} is malformed or invalid");
                    blocks.Add(new Block(id, data.Name ?? id, texture, data.Size));
                }
                else
                {
                    blocks.Add(new Block(id, id, texture));
                }
            }

            return blocks;
        }

        private static IBlockTexture LoadCompositeTexture(string dir)
        {
            return new CompositeBlockTexture(
                up: TextureLoader.Load(FindFace(dir, "up")),
                down: TextureLoader.Load(FindFace(dir, "down")),
                north: TextureLoader.Load(FindFace(dir, "north")),
                south: TextureLoader.Load(FindFace(dir, "south")),
                east: TextureLoader.Load(FindFace(dir, "east")),
                west: TextureLoader.Load(FindFace(dir, "west"))
            );
        }

        private static string FindFace(string dir, string face)
        {
            string? path = Directory.GetFiles(dir, $"{face}.png")
                .Concat(Directory.GetFiles(dir, $"{face}.jpg"))
                .FirstOrDefault();
            return path ?? throw new FileNotFoundException($"Missing face texture '{face}' in {dir}");
        }
    }
}
