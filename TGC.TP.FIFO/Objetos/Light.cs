using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;

namespace TGC.TP.FIFO.Objetos;

public class Light
{
    private const float Size = 20f;

    private readonly BoxPrimitive model;
    private XnaMatrix world;
    
    public Light()
    {
        model = ModelManager.CreateBox(Size, Size, Size);
        world = XnaMatrix.CreateTranslation(EffectManager.LightPosition);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection)
    {
        if (!GameState.DebugMode)
        {
            return;
        }

        var effect = EffectManager.BasicColorShader;

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["DiffuseColor"]?.SetValue(Color.Yellow.ToVector3());

        model.Draw(effect);
    }
}