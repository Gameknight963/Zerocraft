using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace mszcubemod
{
    public static class BlockLoader
    {
        public static List<Block> LoadAll()
        {
            List<Block> blocks = new List<Block>();

            foreach (string file in Directory.GetFiles(Core.ModResources))
            {
                string ext = Path.GetExtension(file).ToLower();
                if (ext != ".png" && ext != ".jpg") continue;
                string name = Path.GetFileNameWithoutExtension(file);
                blocks.Add(new Block(name, name, file));
            }

            foreach (string dir in Directory.GetDirectories(Core.ModResources))
            {
                string texturePath = Directory.GetFiles(dir, "*.png")
                    .Concat(Directory.GetFiles(dir, "*.jpg"))
                    .FirstOrDefault();
                if (texturePath == null) continue;

                string name = Path.GetFileName(dir);
                Block block = new Block(texturePath);

                string jsonPath = Path.Combine(dir, "blockdata.json");
                if (File.Exists(jsonPath))
                {
                    Block data = JsonConvert.DeserializeObject<Block>(File.ReadAllText(jsonPath));
                    if (string.IsNullOrEmpty(data.Name)) throw new FormatException($"{jsonPath}: Block name is malformed");
                    if (data.Size == default) throw new FormatException($"{jsonPath}: Block size is malformed");
                    block.Name = data.Name;
                    block.Size = data.Size;
                }

                blocks.Add(block);
            }

            return blocks;
        }
    }
}
