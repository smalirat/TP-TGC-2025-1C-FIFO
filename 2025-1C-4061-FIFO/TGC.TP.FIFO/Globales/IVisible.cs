using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TGC.TP.FIFO.Globales;

public interface IVisible
{
    BoundingBox BoundingBox { get; }

    bool Visible { get; set; }

    bool VisibleForShadowMap { get; set; }
    Dictionary<CubeMapFace, bool> VisibleForEnvironmentMap { get; }
}