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

namespace TGC.TP.FIFO.Objetos;

public class Checkpoint : IGameObject
{
    private readonly StaticHandle boundingVolume;
    private readonly XnaVector3 position;
    private readonly float scale;
    private XnaQuaternion rotation;

    public XnaVector3 Position => PhysicsManager.GetPosition(boundingVolume);
    public bool Checked { get; private set; } = false;
    public bool Visible { get; set; } = true;
    public bool VisibleForShadowMap { get; set; } = true;
    public Dictionary<CubeMapFace, bool> VisibleForEnvironmentMap { get; } = new();
    public BoundingBox BoundingBox => PhysicsManager.GetBoundingBox(boundingVolume);

    public Checkpoint(XnaVector3 position, float scale = 1f)
    {
        this.scale = scale;
        this.position = position;

        rotation = XnaQuaternion.Identity;
        boundingVolume = PhysicsManager.AddStaticBox(2f, 10f, 2f, position + new Vector3(0f, 5f, 0f), rotation, this);
    }

    public void Draw(XnaMatrix view, XnaMatrix projection, XnaVector3 lightPosition, XnaVector3 eyePosition, int shadowMapSize, XnaMatrix targetLightView, XnaMatrix targetLightProjection, RenderTarget2D shadowMapRenderTarget)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(scale, scale, scale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var model = ModelManager.FlagModel;
        var baseTransforms = new XnaMatrix[model.Bones.Count];
        model.CopyAbsoluteBoneTransformsTo(baseTransforms);

        var localTransform = scaleMatrix * rotationMatrix * translationMatrix;

        var finalColor = Checked ? Color.LimeGreen : Color.Blue;
        var baseColor = finalColor.ToVector3();

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

            effect.Parameters["AmbientColor"]?.SetValue(baseColor * new Vector3(0.5f, 0.5f, 0.5f));
            effect.Parameters["DiffuseColor"]?.SetValue(baseColor * new Vector3(0.9f, 1.0f, 0.9f));
            effect.Parameters["SpecularColor"]?.SetValue(new Vector3(1.0f, 1.0f, 1.0f));
            effect.Parameters["KAmbient"]?.SetValue(0.6f);
            effect.Parameters["KDiffuse"]?.SetValue(1.0f);
            effect.Parameters["KSpecular"]?.SetValue(1.0f);
            effect.Parameters["Shininess"]?.SetValue(6.0f);

            effect.Parameters["LightPosition"]?.SetValue(lightPosition);
            effect.Parameters["EyePosition"]?.SetValue(eyePosition);

            mesh.Draw();
        }
    }

    public void DrawIntoShadowMap(XnaMatrix targetLightView, XnaMatrix targetLightProjection)
    {
        var translationMatrix = XnaMatrix.CreateTranslation(position);
        var scaleMatrix = XnaMatrix.CreateScale(scale, scale, scale);
        var rotationMatrix = XnaMatrix.CreateFromQuaternion(rotation);

        var model = ModelManager.FlagModel;
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
        if (!Checked && !GameState.Lost)
        {
            Checked = true;
            AudioManager.PlayCheckpointSound();
            GameState.CheckpointChecked();
        }
    }

    public void Reset()
    {
        Checked = false;
    }

    public bool CanPlayerBallJumpOnIt()
    {
        return false;
    }

    public XnaVector3 GetPlayerBallRespawnPosition()
    {
        return Position + new XnaVector3(0f, 10f, 0f);
    }
}