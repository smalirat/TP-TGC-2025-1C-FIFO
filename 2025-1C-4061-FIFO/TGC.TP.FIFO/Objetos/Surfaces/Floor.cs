namespace TGC.TP.FIFO.Objetos.Surfaces;

public class Floor : FloorRamp
{
    public Floor(XnaVector3 position, float width, float length) : base(position, XnaQuaternion.Identity, width, length)
    {
    }
}
