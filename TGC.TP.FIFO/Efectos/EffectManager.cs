using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TGC.TP.FIFO.Efectos;

public static class EffectManager
{
    public const string ContentFolderEffects = "Effects/";
    public static Effect SkyBoxShader { get; private set; }
    public static Effect BasicColorShader { get; private set; }
    public static Effect BlinnPhongBasicColorShader { get; private set; }
    public static Effect BlinnPhongTextureShader { get; private set; }
    public static Effect EnvironmentMapShader { get; private set; }
    public static Effect ShadowMapShader { get; private set; }
    public static Effect ShadowPCFShader { get; private set; }

    public static XnaVector3 FloorCenterScenarioPosition { get; set; } = new XnaVector3(0f, 10f, 0f);
    public static XnaVector3 LightPosition { get; set; } = new XnaVector3(0f, 69f, -133f);

    public static void Load(ContentManager content)
    {
        SkyBoxShader = content.Load<Effect>(ContentFolderEffects + "SkyBoxShader");
        BasicColorShader = content.Load<Effect>(ContentFolderEffects + "BasicColorShader");
        BlinnPhongBasicColorShader = content.Load<Effect>(ContentFolderEffects + "BlinnPhongBasicColorShader");
        BlinnPhongTextureShader = content.Load<Effect>(ContentFolderEffects + "BlinnPhongTextureShader");
        EnvironmentMapShader = content.Load<Effect>(ContentFolderEffects + "EnvironmentMapShader");
        ShadowMapShader = content.Load<Effect>(ContentFolderEffects + "ShadowMapShader");
        ShadowPCFShader = content.Load<Effect>(ContentFolderEffects + "ShadowPCFShader");
    }
}