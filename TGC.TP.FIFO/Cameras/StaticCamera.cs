using Microsoft.Xna.Framework;

namespace TGC.TP.FIFO.Cameras;

public class StaticCamera
{
    private const float DefaultFieldOfViewDegrees = MathHelper.PiOver4;
    private const float DefaultNearPlaneDistance = 0.1f;
    private const float DefaultFarPlaneDistance = 2000;

    public XnaVector3 FrontDirection { get; set; }
    public XnaMatrix Projection { get; set; }
    public XnaVector3 Position { get; set; }
    public XnaVector3 UpDirection { get; set; }
    public XnaMatrix View { get; set; }

    public StaticCamera(float aspectRatio, XnaVector3 position, XnaVector3 frontDirection, XnaVector3 upDirection)
    {
        Position = position;
        FrontDirection = frontDirection;
        UpDirection = upDirection;
        
        BuildView();
        BuildProjection(aspectRatio, DefaultNearPlaneDistance, DefaultFarPlaneDistance, DefaultFieldOfViewDegrees);
    }

    public void BuildView()
    {
        View = XnaMatrix.CreateLookAt(Position, Position + FrontDirection, UpDirection);
    }

    public void BuildProjection(float aspectRatio, float nearPlaneDistance, float farPlaneDistance, float fieldOfViewDegrees)
    {
        Projection = XnaMatrix.CreatePerspectiveFieldOfView(fieldOfViewDegrees, aspectRatio, nearPlaneDistance, farPlaneDistance);
    }

    public BoundingFrustum GetFrustum()
    {
        return new BoundingFrustum(View * Projection);
    }
}