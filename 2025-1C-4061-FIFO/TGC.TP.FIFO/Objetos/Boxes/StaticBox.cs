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
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos.Boxes;

public class StaticBox : IGameObject
{
    private readonly BoxPrimitive model;
    private readonly XnaMatrix world;
    private readonly StaticHandle boundingVolume;

    public bool Visible { get; set; } = true;
    public bool VisibleForShadowMap { get; set; } = true;
    public Dictionary<CubeMapFace, bool> VisibleForEnvironmentMap { get; } = new();

    public BoundingBox BoundingBox => PhysicsManager.GetBoundingBox(boundingVolume);

    public StaticBox(XnaVector3 position, XnaQuaternion? rotation = null, float sideLength = 10f)
    {
        rotation ??= XnaQuaternion.Identity;

        boundingVolume = PhysicsManager.AddStaticBox(sideLength, sideLength, sideLength, position, rotation.Value, this);
        model = ModelManager.CreateBox(sideLength, sideLength, sideLength);
        world = XnaMatrix.CreateFromQuaternion(rotation.Value) * XnaMatrix.CreateTranslation(position);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition, int shadowMapSize, XnaMatrix targetLightView, XnaMatrix targetLightProjection, RenderTarget2D shadowMapRenderTarget)
    {
        var effect = EffectManager.BlinnPhongTextureShader;

        effect.CurrentTechnique = effect.Techniques["Default"];

        effect.Parameters["View"]?.SetValue(view);
        effect.Parameters["Projection"]?.SetValue(projection);
        effect.Parameters["World"]?.SetValue(world);
        effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(world)));

        effect.Parameters["LightPosition"]?.SetValue(lightPosition);
        effect.Parameters["EyePosition"]?.SetValue(eyePosition);

        effect.Parameters["AmbientColor"]?.SetValue(new XnaVector3(0.4f, 0.25f, 0.15f));
        effect.Parameters["DiffuseColor"]?.SetValue(new XnaVector3(0.85f, 0.55f, 0.3f));
        effect.Parameters["SpecularColor"]?.SetValue(new XnaVector3(0.4f, 0.3f, 0.2f));
        effect.Parameters["KAmbient"]?.SetValue(0.6f);
        effect.Parameters["KDiffuse"]?.SetValue(1.0f);
        effect.Parameters["KSpecular"]?.SetValue(0.3f);
        effect.Parameters["Shininess"]?.SetValue(16.0f);

        var tilingScale = 1f;
        effect.Parameters["Tiling"]?.SetValue(new XnaVector2(tilingScale, tilingScale));

        effect.Parameters["BaseTexture"]?.SetValue(TextureManager.WoodBox1Texture);
        effect.Parameters["NormalMapTexture"]?.SetValue(TextureManager.WoodBox1NormalMapTexture);

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
        AudioManager.PlayWallHitSound(GameState.BallType, contactSpeed);
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return true;
    }

    public void Reset()
    {
    }
}