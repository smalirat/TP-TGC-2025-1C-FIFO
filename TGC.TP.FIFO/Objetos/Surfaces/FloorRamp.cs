using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos.Surfaces;

public abstract class FloorRamp : IGameObject
{
    private const float Height = 1.25f;

    private readonly BoxPrimitive model;
    private readonly XnaMatrix world;
    private readonly float width;
    private readonly float length;
    private readonly StaticHandle boundingVolume;

    public bool Visible { get; set; } = true;
    public bool VisibleForShadowMap { get; set; } = true;
    public Dictionary<CubeMapFace, bool> VisibleForEnvironmentMap { get; } = new();
    public BoundingBox BoundingBox => PhysicsManager.GetBoundingBox(boundingVolume);

    public FloorRamp(XnaVector3 position, XnaQuaternion rotation, float width, float length)
    {
        this.width = width;
        this.length = length;

        model = ModelManager.CreateBox(Height, width, length);
        world = XnaMatrix.CreateFromQuaternion(rotation) * XnaMatrix.CreateTranslation(position);
        boundingVolume = PhysicsManager.AddStaticBox(width, Height, length, position, rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition, int shadowMapSize, XnaMatrix targetLightView, XnaMatrix targetLightProjection, RenderTarget2D shadowMapRenderTarget)
    {
        var effect = EffectManager.ShadowPCFShader;

        effect.CurrentTechnique = effect.Techniques["Default"];

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(world)));

        effect.Parameters["ShadowMapSize"]?.SetValue(XnaVector2.One * shadowMapSize);
        effect.Parameters["LightPosition"]?.SetValue(lightPosition);
        effect.Parameters["LightViewProjection"]?.SetValue(targetLightView * targetLightProjection);

        var tilingScale = 6f;
        effect.Parameters["Tiling"]?.SetValue(new XnaVector2(length / width * tilingScale, tilingScale));

        effect.Parameters["BaseTexture"]?.SetValue(TextureManager.DirtTexture);
        effect.Parameters["ShadowMap"]?.SetValue(shadowMapRenderTarget);

        effect.Parameters["Bias"]?.SetValue(0.0033000011f);

        model.Draw(effect);
    }

    public void DrawIntoShadowMap(XnaMatrix targetLightView, XnaMatrix targetLightProjection)
    {
        var effect = EffectManager.ShadowMapShader;

        effect.CurrentTechnique = effect.Techniques["Default"];

        effect.Parameters["View"]?.SetValue(targetLightView);
        effect.Parameters["Projection"]?.SetValue(targetLightProjection);
        effect.Parameters["World"]?.SetValue(world);

        model.Draw(effect);
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
    }

    public void NotifyCollition(ICollisionable playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
    }

    public void Reset()
    {
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return true;
    }
}