using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace mszcubemod
{
    public interface IBlockTexture
    {
        Texture2D Top { get; }
        Texture2D Bottom { get; }
        Texture2D Front { get; }
        Texture2D Back { get; }
        Texture2D Left { get; }
        Texture2D Right { get; }
    }
}
