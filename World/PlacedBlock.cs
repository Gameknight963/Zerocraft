using UnityEngine;

namespace mszcubemod.World
{
    record PlacedBlock(string BlockId, Vector3 Position, GameObject Object);
}
