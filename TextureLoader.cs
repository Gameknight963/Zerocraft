using UnityEngine;

namespace mszcubemod
{
    public static class TextureLoader
    {
        public static Texture2D Load(byte[] texture)
        {
            Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);

            if (!ImageConversion.LoadImage(tex, texture))
                throw new InvalidDataException("Failed to load texture.");

            tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
            tex.filterMode = FilterMode.Point;

            return tex;
        }

        public static Texture2D Load(Stream stream)
        {
            using MemoryStream memoryStream = new();
            stream.CopyTo(memoryStream);
            return Load(memoryStream.ToArray());
        }

        public static Texture2D Load(string texturePath)
        {
            using Stream stream = File.OpenRead(texturePath);
            return Load(stream);
        }
    }
}
