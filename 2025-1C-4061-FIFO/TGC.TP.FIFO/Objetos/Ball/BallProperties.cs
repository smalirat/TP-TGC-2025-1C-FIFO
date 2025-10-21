using Microsoft.Xna.Framework.Graphics;
using TGC.TP.FIFO.Texturas;

namespace TGC.TP.FIFO.Objetos.Ball;

public class BallProperties
{
    public BallType BallType { get; init; }
    public float ImpulseForce { get; init; }
    public float JumpForce { get; init; }
    public float Friction { get; init; }
    public float SpringFrequency { get; init; }
    public float DampingRatio { get; init; }
    public float Mass { get; init; }
    public float MaximumRecoveryVelocity { get; init; }
    public float Radius { get; init; }
    public XnaVector3 AmbientColor { get; init; }
    public XnaVector3 DiffuseColor { get; init; }
    public XnaVector3 SpecularColor { get; init; }
    public float Shininess { get; init; }
    public float KAmbient { get; init; }
    public float KDiffuse { get; init; }
    public float KSpecular { get; init; }

    public Texture2D GetBaseTexture()
    {
        if (BallType.Goma == BallType)
        {
            return TextureManager.RubberBallTexture;
        }
        else if (BallType.Metal == BallType)
        {
            return TextureManager.MetalBallTexture;
        }
        
        return TextureManager.RockyBallTexture;
    }

    public Texture2D GetNormalMapTexture()
    {
        if (BallType.Goma == BallType)
        {
            return TextureManager.RubberBallNormalMapTexture;
        }
        else if (BallType.Metal == BallType)
        {
            return TextureManager.MetalBallNormalMapTexture;
        }

        return TextureManager.RockyBallNormalMapTexture;
    }
}