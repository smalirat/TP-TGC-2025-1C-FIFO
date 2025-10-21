using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using TGC.TP.FIFO.Fuentes;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Optimizaciones;

namespace TGC.TP.FIFO.HUDS;

public class HUD
{
    private const int MINIMAP_SIZE = 150;
    private const float MINIMAP_MIN_X = -75f;
    private const float MINIMAP_MAX_X = 75f;
    private const float MINIMAP_MIN_Z = -75f;
    private const float MINIMAP_MAX_Z = 900f;

    private Texture2D _progressBarTexture;
    private XnaVector2 _timerPosition;
    private XnaVector2 _progressBarPosition;
    private XnaVector2 _progressBarSize = new XnaVector2(350, 40);
    private XnaVector2 _minimapPosition;

    private static readonly Color BarBackground = new Color(0, 0, 0, 128);
    private static readonly Color BarFill = Color.Green;
    private static readonly Color MinimapBackground = Color.Black * 0.7f;

    private readonly List<Checkpoint> checkpoints;
    private readonly PlayerBall playerBall;
    private readonly PerformanceMetrics performanceMetrics;

    public HUD(List<Checkpoint> checkpoints, PlayerBall playerBall, PerformanceMetrics performanceMetrics)
    {
        this.checkpoints = checkpoints;
        this.playerBall = playerBall;
        this.performanceMetrics = performanceMetrics;

        _timerPosition = new XnaVector2(GameGlobals.GraphicsDevice.Viewport.Width / 2f, 50f);
        _progressBarPosition = new XnaVector2(GameGlobals.GraphicsDevice.Viewport.Width - _progressBarSize.X + 80, GameGlobals.GraphicsDevice.Viewport.Height - _progressBarSize.Y + 80);
        _minimapPosition = new XnaVector2(20, GameGlobals.GraphicsDevice.Viewport.Height - MINIMAP_SIZE + 80);
    }

    public void LoadContent(ContentManager content)
    {
        _progressBarTexture = new Texture2D(GameGlobals.GraphicsDevice, 1, 1);
        _progressBarTexture.SetData([Color.White]);
    }

    public void Draw()
    {
        var originalDepthStencilState = GameGlobals.GraphicsDevice.DepthStencilState;
        var originalBlendState = GameGlobals.GraphicsDevice.BlendState;

        GameGlobals.GraphicsDevice.DepthStencilState = DepthStencilState.None;
        GameGlobals.GraphicsDevice.BlendState = BlendState.AlphaBlend;

        DrawTimer();
        DrawProgressBar();
        DrawMinimap();
        DrawEndGameMessage();
        DrawFrustumCullingCount();
        DrawFps();

        GameGlobals.GraphicsDevice.DepthStencilState = originalDepthStencilState;
        GameGlobals.GraphicsDevice.BlendState = originalBlendState;
    }

    private void DrawFrustumCullingCount()
    {
        if (!GameState.DebugMode)
        {
            return;
        }

        GameGlobals.SpriteBatch.Begin();

        var font = FontsManager.LucidaConsole14;

        string[] texts =
        {
            $"* Pantalla: {CullingManager.VisibleGameObjectCount} draw calls de {CullingManager.GameObjectCount} potenciales",
            $"* Shadow map: {CullingManager.VisibleGameObjectShadowableCount} draw calls de {CullingManager.PotentialGameObjectShadowableCount} potenciales",
            $"* Environment map:",
            $"  - Potenciales draw calls por cara del cubo: {CullingManager.PotentialGameObjectReflectablePerFaceCount}",
            $"  - Potenciales draw calls en todo el cubo: {CullingManager.PotentialGameObjectReflectableCount}",
            $"    Draw calls en todo el cubo: {CullingManager.VisibleGameObjectReflectableCount}",
            $"* Total: {CullingManager.RealDrawCalls} draw calls de {CullingManager.PotentialDrawCalls} potenciales",
        };

        const int marginX = 20;
        const int marginY = 80;
        const int paddingX = 10;
        const int paddingY = 10;
        const int lineSpacing = 2;

        float maxWidth = 0f;
        float totalTextHeight = 0f;

        foreach (var line in texts)
        {
            var size = font.MeasureString(line);
            maxWidth = MathF.Max(maxWidth, size.X);
            totalTextHeight += size.Y + lineSpacing;
        }
        totalTextHeight -= lineSpacing;

        int rectWidth = (int)maxWidth + paddingX * 2;
        int rectHeight = (int)totalTextHeight + paddingY * 2;

        int rectX = GameGlobals.GraphicsDevice.Viewport.Width - rectWidth - marginX;
        int rectY = marginY;

        var backgroundRect = new Rectangle(rectX, rectY, rectWidth, rectHeight);
        GameGlobals.SpriteBatch.Draw(_progressBarTexture, backgroundRect, BarBackground);

        var penY = (float)rectY + paddingY;
        foreach (var line in texts)
        {
            GameGlobals.SpriteBatch.DrawString(
                font,
                line,
                new Vector2(rectX + paddingX, penY),
                Color.Yellow);

            penY += font.MeasureString(line).Y + lineSpacing;
        }

        GameGlobals.SpriteBatch.End();
    }

    private void DrawFps()
    {
        if (!GameState.DebugMode)
        {
            return;
        }

        GameGlobals.SpriteBatch.Begin();

        var font = FontsManager.LucidaConsole20;
        var fpsText = $"{performanceMetrics.FPS} FPS";
        var textSize = font.MeasureString(fpsText);

        const int marginX = 20;
        const int marginY = 20;
        const int paddingX = 10;
        const int paddingY = 10;

        int rectWidth = (int)textSize.X + paddingX * 2;
        int rectHeight = (int)textSize.Y + paddingY * 2;

        int rectX = GameGlobals.GraphicsDevice.Viewport.Width - rectWidth - marginX;
        int rectY = marginY;

        var textPos = new XnaVector2(rectX + paddingX, rectY + paddingY);

        GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle(rectX, rectY, rectWidth, rectHeight), BarBackground);
        GameGlobals.SpriteBatch.DrawString(font, fpsText, textPos, Color.Yellow);
        GameGlobals.SpriteBatch.End();
    }

    private void DrawTimer()
    {
        string time = GameState.Cronometer.Elapsed.ToString("mm\\:ss\\.ff");
        var font = FontsManager.LucidaConsole40;
        XnaVector2 size = font.MeasureString(time);
        XnaVector2 pos = new XnaVector2(_timerPosition.X - size.X / 2f, _timerPosition.Y);

        Color timerColor = Color.White;

        if (GameState.Won)
        {
            timerColor = Color.LimeGreen;
        }
        else if (GameState.Lost)
        {
            timerColor = Color.Red;
        }
        else
        {
            double elapsed = GameState.Cronometer.Elapsed.TotalSeconds;
            double total = GameState.TotalSecondsBeforeLosing;
            double threshold = GameState.TotalSecondsBeforeAboutToLose;

            if (total - elapsed <= threshold)
            {
                bool flash = (int)(elapsed * 2) % 2 == 0;
                timerColor = flash ? Color.Red : Color.White;
            }
        }

        int paddingX = 10;
        int paddingY = 10;
        int rectX = (int)(pos.X - paddingX);
        int rectY = (int)(pos.Y - paddingY);
        int rectWidth = (int)(size.X + 2 * paddingX);
        int rectHeight = (int)(size.Y + 2 * paddingY);

        GameGlobals.SpriteBatch.Begin();
        GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle(rectX, rectY, rectWidth, rectHeight), BarBackground);
        GameGlobals.SpriteBatch.DrawString(font, time, pos, timerColor);
        GameGlobals.SpriteBatch.End();
    }

    private void DrawProgressBar()
    {
        float progress = (float)GameState.TotalCheckpointsChecked / GameState.TotalCheckpoints;
        Color fillColor = GameState.Lost ? Color.Red : BarFill;
        var font = FontsManager.LucidaConsole20;
        string resultText;
        Color resultColor;

        resultText = $"CHECKPOINTS {GameState.TotalCheckpointsChecked}/{GameState.TotalCheckpoints}";
        resultColor = Color.White;

        XnaVector2 textSize = font.MeasureString(resultText);
        XnaVector2 textPosition = new XnaVector2(_progressBarPosition.X + _progressBarSize.X / 2f - textSize.X / 2f, _progressBarPosition.Y - textSize.Y - 5);

        GameGlobals.SpriteBatch.Begin();
        GameGlobals.SpriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, BarBackground, 0f, XnaVector2.Zero, _progressBarSize, SpriteEffects.None, 0f);
        GameGlobals.SpriteBatch.Draw(_progressBarTexture, _progressBarPosition, null, fillColor, 0f, XnaVector2.Zero, new XnaVector2(_progressBarSize.X * progress, _progressBarSize.Y), SpriteEffects.None, 0f);
        GameGlobals.SpriteBatch.DrawString(font, resultText, textPosition, resultColor);
        GameGlobals.SpriteBatch.End();
    }

    private void DrawMinimap()
    {
        GameGlobals.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle((int)_minimapPosition.X, (int)_minimapPosition.Y, MINIMAP_SIZE, MINIMAP_SIZE), MinimapBackground);

        foreach (var checkpoint in checkpoints)
        {
            XnaVector2 pos = GetMinimapPosition(checkpoint.Position);
            Color color = checkpoint.Checked ? Color.LightGreen : Color.Yellow;
            GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle((int)(pos.X - 4), (int)(pos.Y - 4), 8, 8), color);
        }

        XnaVector2 ballPos = GetMinimapPosition(playerBall?.Position ?? XnaVector3.Zero);

        GameGlobals.SpriteBatch.Draw(_progressBarTexture, new Rectangle((int)(ballPos.X - 3), (int)(ballPos.Y - 3), 6, 6), Color.Red);
        GameGlobals.SpriteBatch.End();
    }

    private void DrawEndGameMessage()
    {
        if (!GameState.Won && !GameState.Lost)
        {
            return;
        }

        string message = GameState.Won ? "GANASTE" : "PERDISTE";
        var font = FontsManager.LucidaConsole60;

        var textSize = font.MeasureString(message);
        var screenCenter = new XnaVector2(GameGlobals.GraphicsDevice.Viewport.Width / 2f, GameGlobals.GraphicsDevice.Viewport.Height / 2f);
        var textOrigin = textSize / 2f;

        var paddingX = 30;
        var paddingY = 20;

        var backgroundRect = new Rectangle(
            (int)(screenCenter.X - textSize.X / 2f) - paddingX,
            (int)(screenCenter.Y - textSize.Y / 2f) - paddingY,
            (int)textSize.X + 2 * paddingX,
            (int)textSize.Y + 2 * paddingY
        );

        Color color = GameState.Won ? Color.LimeGreen : Color.Red;

        GameGlobals.SpriteBatch.Begin();
        GameGlobals.SpriteBatch.Draw(_progressBarTexture, backgroundRect, MinimapBackground);
        GameGlobals.SpriteBatch.DrawString(font, message, screenCenter, color, 0f, textOrigin, 1f, SpriteEffects.None, 0f);
        GameGlobals.SpriteBatch.End();
    }

    private Vector2 GetMinimapPosition(Vector3 worldPosition)
    {
        float normX = 1f - (worldPosition.X - MINIMAP_MIN_X) / (MINIMAP_MAX_X - MINIMAP_MIN_X);
        float normZ = (worldPosition.Z - MINIMAP_MIN_Z) / (MINIMAP_MAX_Z - MINIMAP_MIN_Z);

        normX = MathHelper.Clamp(normX, 0f, 1f);
        normZ = MathHelper.Clamp(normZ, 0f, 1f);

        float posX = normX * MINIMAP_SIZE;
        float posZ = (1f - normZ) * MINIMAP_SIZE;

        return _minimapPosition + new Vector2(posX, posZ);
    }
}