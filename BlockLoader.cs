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

            foreach (string filePath in Directory.GetFiles(Core.ModResources))
            {
                string ext = Path.GetExtension(filePath).ToLower();
                if (ext != ".png" && ext != ".jpg") continue;
                string name = Path.GetFileNameWithoutExtension(filePath);
                blocks.Add(new Block(name, name, filePath));
            }

            foreach (string dir in Directory.GetDirectories(Core.ModResources))
            {
                string texturePath = Directory.GetFiles(dir, "*.png")
                    .Concat(Directory.GetFiles(dir, "*.jpg"))
                    .FirstOrDefault();
                if (texturePath == null) continue;

                string dirName = Path.GetFileName(dir);
                string jsonPath = Path.Combine(dir, "blockdata.json");

                if (File.Exists(jsonPath))
                {
                    Block data = JsonConvert.DeserializeObject<Block>(File.ReadAllText(jsonPath));
                    if (string.IsNullOrEmpty(data.Name)) throw new FormatException($"{jsonPath}: Block name is malformed");
                    if (data.Size == default) throw new FormatException($"{jsonPath}: Block size is malformed");
                    Block block = new Block(dirName, data.Name, texturePath, data.Size);
                    blocks.Add(block);
                }
            }

            return blocks;
        }
    }
}
