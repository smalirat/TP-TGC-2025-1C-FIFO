using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Globales;

public interface IDrawable
{
    void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition, int shadowMapSize, XnaMatrix targetLightView, XnaMatrix targetLightProjection, RenderTarget2D shadowMapRenderTarget);

    void DrawIntoShadowMap(XnaMatrix targetLightView, XnaMatrix targetLightProjection);
}
