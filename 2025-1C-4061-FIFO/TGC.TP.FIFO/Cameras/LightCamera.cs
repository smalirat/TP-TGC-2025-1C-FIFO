using Microsoft.Xna.Framework;
using TGC.TP.FIFO.Efectos;

namespace TGC.TP.FIFO.Cameras;

public class LightCamera
{
    private const float nearPlane = 80f;
    private const float farPlane = 400f;

    public Matrix View => GetView();

    public Matrix Projection => Matrix.CreatePerspectiveFieldOfView(
        fieldOfView: MathHelper.ToRadians(55f),
        aspectRatio: 1f,
        nearPlaneDistance: nearPlane,
        farPlaneDistance: farPlane);

    public Matrix GetView()
    {
        var FrontDirection = Vector3.Normalize(EffectManager.FloorCenterScenarioPosition - EffectManager.LightPosition);
        var RightDirection = Vector3.Normalize(Vector3.Cross(Vector3.Up, FrontDirection));
        var UpDirection = Vector3.Cross(FrontDirection, RightDirection);
        return Matrix.CreateLookAt(EffectManager.LightPosition, EffectManager.LightPosition + FrontDirection, UpDirection);
    }

    public BoundingFrustum GetFrustum()
    {
        return new BoundingFrustum(View * Projection);
    }
}