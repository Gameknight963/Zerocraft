using Newtonsoft.Json;
using System;
using System.IO;
using UnityEngine;

namespace mszcubemod
{
    public class Block
    {
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";
        public Vector3 Size { get; set; } = new Vector3(0.5f, 0.5f, 0.5f);

        [JsonIgnore]
        public Texture2D Texture { get; private set; }

        [JsonIgnore]
        public string TexturePath { get; set; }

        public Block(string id, string name, string texturePath, Vector3 size)
        {
            ThrowIfTexturePathIsInvalid(texturePath);
            Texture = LoadTexture(texturePath);
            Id = id;
            Name = name;
            Size = size;
        }

        public Block(string id, string name, string texturePath)
        {
            ThrowIfTexturePathIsInvalid(texturePath);
            Texture = LoadTexture(texturePath);
            Id = id;
            Name = name;
            Size = Core.DefaultCubeSize;
        }

        /// <summary>
        /// Id and Name will be null when using this overload,
        /// be careful!
        /// </summary>
        public Block(string texturePath)
        {
            ThrowIfTexturePathIsInvalid(texturePath);
            Texture = LoadTexture(texturePath);
        }

        private Texture2D LoadTexture(string texturePath)
        {
            TexturePath = texturePath;
            Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            ImageConversion.LoadImage(tex, File.ReadAllBytes(texturePath));
            tex.hideFlags = HideFlags.DontUnloadUnusedAsset;
            return tex;
        }

        private void ThrowIfTexturePathIsInvalid(string texturePath)
        {
            if (!File.Exists(texturePath)) throw new FileNotFoundException($"{texturePath}: No such file");
            if (!texturePath.EndsWith(".jpg") && !texturePath.EndsWith(".png"))
                throw new InvalidOperationException($"{texturePath}: File must be either PNG or JPG");
        }
    }
}
