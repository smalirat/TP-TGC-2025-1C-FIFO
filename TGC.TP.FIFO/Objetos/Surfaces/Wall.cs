using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TGC.TP.FIFO.Audio;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Efectos;
using TGC.TP.FIFO.Fisica;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Modelos;
using TGC.TP.FIFO.Modelos.Primitivas;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos.Surfaces;

public class Wall : IGameObject
{
    private const float Depth = 1.25f;

    private readonly StaticHandle boundingVolume;
    private readonly BoxPrimitive model;
    private readonly XnaMatrix world;

    public bool Visible { get; set; } = true;
    public bool VisibleForShadowMap { get; set; } = true;
    public Dictionary<CubeMapFace, bool> VisibleForEnvironmentMap { get; } = new();

    public BoundingBox BoundingBox => PhysicsManager.GetBoundingBox(boundingVolume);

    public Wall(XnaVector3 position, XnaQuaternion rotation, float width, float height)
    {
        model = ModelManager.CreateBox(Depth, width, height);
        boundingVolume = PhysicsManager.AddStaticBox(width, Depth, height, position, rotation, this);
        world = XnaMatrix.CreateFromQuaternion(PhysicsManager.GetOrientation(boundingVolume)) * XnaMatrix.CreateTranslation(PhysicsManager.GetPosition(boundingVolume));
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

        var tilingScale = 8f;
        effect.Parameters["Tiling"]?.SetValue(new XnaVector2(tilingScale, tilingScale));

        effect.Parameters["BaseTexture"]?.SetValue(TextureManager.StonesTexture);
        effect.Parameters["ShadowMap"]?.SetValue(shadowMapRenderTarget);

        effect.Parameters["Bias"]?.SetValue(0.006799997f);

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
        if (!(playerBall as PlayerBall).IsDummy)
        {
            AudioManager.PlayWallHitSound(GameState.BallType, contactSpeed);
        }
    }

    public void Reset()
    {
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return false;
    }
}