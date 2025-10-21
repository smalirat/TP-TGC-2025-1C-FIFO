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
using TGC.TP.FIFO.Modelos;

namespace TGC.TP.FIFO.Objetos.PowerUps.Jump;

public abstract class JumpPowerUp : IGameObject
{
    private const float xScale = 1f / 0.78f;
    private const float yScale = 5f / 3.72f;
    private const float zScale = 3f / 5.66f;

    private readonly XnaVector3 modelOffset = new(0f, 2f, 0f);
    private readonly StaticHandle boundingVolume;

    private Color color;
    private XnaQuaternion rotation;
    private XnaVector3 position;

    public float JumpMultiplier { get; private set; }
    public bool Visible { get; set; } = true;
    public bool VisibleForShadowMap { get; set; } = true;
    public Dictionary<CubeMapFace, bool> VisibleForEnvironmentMap { get; } = new();
    public BoundingBox BoundingBox => PhysicsManager.GetBoundingBox(boundingVolume);

    public JumpPowerUp(XnaVector3 position, float jumpMultiplier, Color color)
    {
        this.position = position + modelOffset;
        this.color = color;

        rotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, -MathF.PI / 2f);
        boundingVolume = PhysicsManager.AddStaticBox(4.4f, 4.4f, 3.5f, position, rotation, this);
        JumpMultiplier = jumpMultiplier;
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition, int shadowMapSize, XnaMatrix targetLightView, XnaMatrix targetLightProjection, RenderTarget2D shadowMapRenderTarget)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(xScale, yScale, zScale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var model = ModelManager.ArrowModel;
        var baseTransforms = new XnaMatrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        var baseColor = color.ToVector3();

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = EffectManager.BlinnPhongBasicColorShader;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            var effect = EffectManager.BlinnPhongBasicColorShader;
            effect.CurrentTechnique = effect.Techniques["Default"];

            effect.Parameters["View"]?.SetValue(view);
            effect.Parameters["Projection"]?.SetValue(projection);
            effect.Parameters["World"]?.SetValue(meshTransform * localTransform);
            effect.Parameters["WorldViewProjection"]?.SetValue(meshTransform * localTransform * view * projection);
            effect.Parameters["InverseTransposeWorld"]?.SetValue(XnaMatrix.Transpose(XnaMatrix.Invert(meshTransform * localTransform)));

            effect.Parameters["AmbientColor"]?.SetValue(baseColor * new Vector3(0.5f, 0.5f, 0.3f));
            effect.Parameters["DiffuseColor"]?.SetValue(baseColor * new Vector3(1.0f, 1.0f, 0.7f));
            effect.Parameters["SpecularColor"]?.SetValue(new Vector3(1.0f, 1.0f, 1.0f));
            effect.Parameters["KAmbient"]?.SetValue(0.6f);
            effect.Parameters["KDiffuse"]?.SetValue(1.0f);
            effect.Parameters["KSpecular"]?.SetValue(1.0f);
            effect.Parameters["Shininess"]?.SetValue(96.0f);

            effect.Parameters["LightPosition"]?.SetValue(lightPosition);
            effect.Parameters["EyePosition"]?.SetValue(eyePosition);

            mesh.Draw();
        }
    }

    public void DrawIntoShadowMap(XnaMatrix targetLightView, XnaMatrix targetLightProjection)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(xScale, yScale, zScale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var model = ModelManager.ArrowModel;
        var baseTransforms = new XnaMatrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        foreach (var mesh in model.Meshes)
        {
            foreach (var meshPart in mesh.MeshParts)
            {
                meshPart.Effect = EffectManager.ShadowMapShader;
            }

            var meshTransform = baseTransforms[mesh.ParentBone.Index];

            var effect = EffectManager.ShadowMapShader;
            effect.CurrentTechnique = effect.Techniques["Default"];

            effect.Parameters["View"]?.SetValue(targetLightView);
            effect.Parameters["Projection"]?.SetValue(targetLightProjection);
            effect.Parameters["World"]?.SetValue(meshTransform * localTransform);

            mesh.Draw();
        }
    }

    public void Update(KeyboardState keyboardState, float deltaTime, TargetCamera camera)
    {
        var incrementalRotation = XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, deltaTime * 0.5f);
        rotation = XnaQuaternion.Normalize(incrementalRotation * rotation);
    }

    public void NotifyCollition(ICollisionable playerBall, XnaVector3? contactNormal, float contactSpeed)
    {
        AudioManager.PlayJumpPowerUpSound();
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return false;
    }

    public void Reset()
    {
    }
}