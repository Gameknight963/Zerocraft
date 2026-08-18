using UnityEngine;

namespace mszcubemod
{
    public class BlockData
    {
        public string? Name { get; set; }
        public Vector3? Size { get; set; }
        public float? AnimationSpeed { get; set; }
        public float[]? FrameDurations { get; set; }
    }
}
