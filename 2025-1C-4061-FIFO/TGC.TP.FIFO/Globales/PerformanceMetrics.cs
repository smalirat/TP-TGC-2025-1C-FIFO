namespace TGC.TP.FIFO.Globales;

public class PerformanceMetrics
{
    public int FPS = 0;
    private int currentFPS = 0;
    private double FPSTimer = 0;

    public void Update(double elapsedSeconds)
    {
        currentFPS++;
        FPSTimer += elapsedSeconds;

        if (FPSTimer >= 1.0)
        {
            FPS = currentFPS;
            currentFPS = 0;
            FPSTimer -= 1.0;
        }
    }
}