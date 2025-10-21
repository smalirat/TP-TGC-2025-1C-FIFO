using System.Collections.Generic;

namespace TGC.TP.FIFO.Objetos.Ball;

public static class BallPresets
{
    public static readonly Dictionary<BallType, BallProperties> Presets = new()
    {
        [BallType.Piedra] = new BallProperties
        {
            BallType = BallType.Piedra,
            ImpulseForce = 60f,
            JumpForce =  7500f,
            Friction = 0.4f,
            DampingRatio = 0.2f,
            MaximumRecoveryVelocity = 6f,
            SpringFrequency = 20f,
            Mass = 6f,
            Radius = 3f,
            AmbientColor = new XnaVector3(0.4f, 0.4f, 0.4f),
            DiffuseColor = new XnaVector3(0.75f, 0.75f, 0.75f),
            SpecularColor = new XnaVector3(1.0f, 1.0f, 1.0f),
            KAmbient = 0.6f,
            KDiffuse = 1.0f,
            KSpecular = 1.0f,
            Shininess = 128.0f
        },

        [BallType.Metal] = new BallProperties
        {
            BallType = BallType.Metal,
            ImpulseForce = 30f,
            JumpForce = 2000f,
            Friction = 0.4f,
            DampingRatio = 0.2f,
            MaximumRecoveryVelocity = 6f,
            SpringFrequency = 20f,
            Mass = 2f,
            Radius = 2f,
            AmbientColor = new XnaVector3(0.4f, 0.4f, 0.4f),
            DiffuseColor = new XnaVector3(0.75f, 0.75f, 0.75f),
            SpecularColor = new XnaVector3(1.0f, 1.0f, 1.0f),
            KAmbient = 0.6f,
            KDiffuse = 1.0f,
            KSpecular = 1.0f,
            Shininess = 128.0f
        },

        [BallType.Goma] = new BallProperties
        {
            BallType = BallType.Goma,
            ImpulseForce = 30f,
            JumpForce = 2100f,
            Friction = 0.8f,
            DampingRatio = 0.2f,
            MaximumRecoveryVelocity = 6f,
            SpringFrequency = 20f,
            Mass = 1.25f,
            Radius = 2.3f ,
            AmbientColor = new XnaVector3(0.5f, 0.5f, 0.5f),
            DiffuseColor = new XnaVector3(1.0f, 1.0f, 1.0f),
            SpecularColor = new XnaVector3(1.0f, 1.0f, 1.0f),
            KAmbient = 0.6f,
            KDiffuse = 1.2f,
            KSpecular = 1.0f,
            Shininess = 64.0f
        }
    };
}