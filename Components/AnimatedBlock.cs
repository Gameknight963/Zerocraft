using MelonLoader;
using mszcubemod.Textures;
using UnityEngine;

namespace mszcubemod.Components
{
    [RegisterTypeInIl2Cpp]
    public class AnimatedBlock : MonoBehaviour
    {
        private (AnimatedTexture Animation, Material Material)[] _animatedFaces = Array.Empty<(AnimatedTexture, Material)>();

        internal void Initialize(AnimatedBlockFaceTextures animations, Material[] materials)
        {
            _animatedFaces = new[]
            {
                (animations.Up, materials[2]),
                (animations.Down, materials[3]),
                (animations.North, materials[4]),
                (animations.South, materials[5]),
                (animations.East, materials[0]),
                (animations.West, materials[1]),
            };
            foreach ((AnimatedTexture animation, Material material) in _animatedFaces)
            {
                Material mat = material;
                animation.Updated += () => mat.mainTexture = animation.Texture;
                animation.Start();
            }
        }

        private void Update()
        {
            foreach ((AnimatedTexture? animation, Material? material) in _animatedFaces)
                material.mainTexture = animation.Texture;
        }
    }
}
