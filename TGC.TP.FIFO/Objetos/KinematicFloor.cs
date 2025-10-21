using BepuPhysics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
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

namespace TGC.TP.FIFO.Objetos;

public class KinematicFloor : IGameObject
{
    private const float Depth = 1.25f;
    private const float Width = 15f;
    private const float Mass = 1f;
    private const float Friction = 0.2f;

    private readonly BodyHandle boundingVolume;
    private readonly BoxPrimitive model;
    private readonly XnaVector3 movementDirection;

    private XnaMatrix world;
    private float tiempo;

    public bool Visible { get; set; } = true;
    public bool VisibleForShadowMap { get; set; } = true;
    public Dictionary<CubeMapFace, bool> VisibleForEnvironmentMap { get; } = new();
    public BoundingBox BoundingBox => PhysicsManager.GetBoundingBox(boundingVolume);

    public KinematicFloor(XnaVector3 initialPosition, XnaVector3 movementDirection)
    {
        this.movementDirection = movementDirection;

        model = ModelManager.CreateBox(Depth, Width, Width);
        boundingVolume = PhysicsManager.AddKinematicBox(Width, Depth, Width, Mass, Friction, initialPosition, XnaQuaternion.Identity, this);
        world = XnaMatrix.CreateTranslation(initialPosition);
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

        effect.Parameters["BaseTexture"]?.SetValue(TextureManager.WoodBox2Texture);
        effect.Parameters["NormalMapTexture"]?.SetValue(TextureManager.WoodBox2NormalMapTexture);

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
        PhysicsManager.Awake(boundingVolume);

        var previousPosition = PhysicsManager.GetPosition(boundingVolume);

        // Avanzar el tiempo
        tiempo += deltaTime * 1f;

        // Movimiento oscilante sobre la dirección indicada
        float offset = MathF.Sin(tiempo) * 0.2f;
        XnaVector3 desplazamiento = movementDirection * offset;

        XnaVector3 nuevaPosicion = new XnaVector3(
            previousPosition.X + desplazamiento.X,
            previousPosition.Y + desplazamiento.Y,
            previousPosition.Z + desplazamiento.Z
        );

        PhysicsManager.SetPosition(boundingVolume, new BepuVector3(nuevaPosicion.X, nuevaPosicion.Y, nuevaPosicion.Z));

        // Actualizar la matriz mundo
        world = XnaMatrix.CreateTranslation(nuevaPosicion);
    }

    public void NotifyCollition(ICollisionable playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        AudioManager.PlayWallHitSound(GameState.BallType, contactSpeed);
    }

    public void Reset()
    {
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return true;
    }
}