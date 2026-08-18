using mszcubemod.Textures;
using Newtonsoft.Json;
using UnityEngine;

namespace mszcubemod.World
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
                Texture2D tex = TextureLoader.Load(filePath);
                if (tex.height > tex.width)
                {
                    AnimatedTexture anim = SliceAnimation(tex, null, null);
                    blocks.Add(new Block(id, id, new UniformAnimatedBlockTexture(anim)));
                }
                else
                {
                    blocks.Add(new Block(id, id, new UniformBlockTexture(tex)));
                }
            }
            foreach (string dir in Directory.GetDirectories(Core.ModResources))
            {
                string id = Path.GetFileName(dir);
                string jsonPath = Path.Combine(dir, "blockdata.json");
                BlockData? data = null;
                if (File.Exists(jsonPath))
                    data = JsonConvert.DeserializeObject<BlockData>(File.ReadAllText(jsonPath))
                        ?? throw new InvalidOperationException($"{jsonPath} is malformed or invalid");

                IBlockTexture texture = LoadCompositeTexture(dir, data);
                blocks.Add(new Block(id, data?.Name ?? id, texture, data?.Size));
            }
            return blocks;
        }

        private static IBlockTexture LoadCompositeTexture(string dir, BlockData? data)
        {
            Texture2D up = TextureLoader.Load(FindFace(dir, "up"));
            Texture2D down = TextureLoader.Load(FindFace(dir, "down"));
            Texture2D north = TextureLoader.Load(FindFace(dir, "north"));
            Texture2D south = TextureLoader.Load(FindFace(dir, "south"));
            Texture2D east = TextureLoader.Load(FindFace(dir, "east"));
            Texture2D west = TextureLoader.Load(FindFace(dir, "west"));

            bool anyAnimated = new[] { up, down, north, south, east, west }
                .Any(t => t.height > t.width);

            if (!anyAnimated)
                return new CompositeBlockTexture(up, down, north, south, east, west);

            return new CompositeAnimatedBlockTexture(
                SliceAnimation(up, data?.FrameDurations, data?.AnimationSpeed),
                SliceAnimation(down, data?.FrameDurations, data?.AnimationSpeed),
                SliceAnimation(north, data?.FrameDurations, data?.AnimationSpeed),
                SliceAnimation(south, data?.FrameDurations, data?.AnimationSpeed),
                SliceAnimation(east, data?.FrameDurations, data?.AnimationSpeed),
                SliceAnimation(west, data?.FrameDurations, data?.AnimationSpeed)
            );
        }

        private static string FindFace(string dir, string face)
        {
            string? path = Directory.GetFiles(dir, $"{face}.png")
                .Concat(Directory.GetFiles(dir, $"{face}.jpg"))
                .FirstOrDefault();
            return path ?? throw new FileNotFoundException($"Missing face texture '{face}' in {dir}");
        }

        private static AnimatedTexture SliceAnimation(Texture2D tex, float[]? frameDurations, float? animationSpeed)
        {
            int frameSize = tex.width;
            int frameCount = tex.height / frameSize;
            Texture2D[] frames = new Texture2D[frameCount];
            for (int i = 0; i < frameCount; i++)
            {
                Color[] pixels = tex.GetPixels(0, tex.height - (i + 1) * frameSize, frameSize, frameSize);
                Texture2D frame = new(frameSize, frameSize, tex.format, false);
                frame.SetPixels(pixels);
                frame.Apply();
                frames[i] = frame;
            }

            float[] durations = frameDurations ?? Enumerable.Repeat(animationSpeed ?? 0.1f, frameCount).ToArray();
            return new AnimatedTexture(frames, durations);
        }
    }
}
