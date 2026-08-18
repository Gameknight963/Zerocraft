using UnityEngine;

namespace mszcubemod.World
{
    public class BlockData
    {
        public string? Name { get; set; }
        public Vector3? Size { get; set; }
        public float? AnimationSpeed { get; set; }
        public float[]? FrameDurations { get; set; }
    }
}
