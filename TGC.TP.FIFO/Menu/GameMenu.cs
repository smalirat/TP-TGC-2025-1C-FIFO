﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Fuentes;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Objetos;
using TGC.TP.FIFO.Objetos.Ball;
using TGC.TP.FIFO.Objetos.Boxes;
using TGC.TP.FIFO.Objetos.Surfaces;

namespace TGC.TP.FIFO.Menu;

public class GameMenu
{
    private readonly TargetCamera Camera;

    private readonly Floor Piso;
    private readonly Wall ParedIzquierda;
    private readonly Wall ParedFondo;
    private readonly Checkpoint DummyCheckpoint;
    private readonly PlayerBall DummyPlayerBall;

    private float currentRotation = 0f;

    public List<StaticBox> StaticBoxes = new List<StaticBox>();

    private KeyboardState previousKeyboardState;

    private MenuState mainCurrentMenuState = MenuState.MainMenu;
    private MenuState subOptionMenuState = MenuState.NoSubOptions;

    private Dictionary<Tuple<MenuState, MenuState>, MenuEntry[]> menuEntries;

    private int selectedOptionIndex = 0;

    private static XnaVector3 MenuPosition = new XnaVector3(40000f, 40000f, 40000f);

    private static XnaVector3 BallPosition = MenuPosition + new XnaVector3(0f, -5f, 10f);

    private static XnaVector3 LightPosition = BallPosition + new XnaVector3(-4.2f, 2.984f, 15.79f);

    private Texture2D BackgroundMenuTexture;

    public GameMenu(Action exitGameAction, Action newGameAction, Action resetBallAction)
    {
        BackgroundMenuTexture = new Texture2D(GameGlobals.GraphicsDevice, 1, 1);
        BackgroundMenuTexture.SetData([Color.White]);

        Camera = new TargetCamera(initialTargetPosition: MenuPosition, initialRotation: XnaQuaternion.Identity);

        DummyPlayerBall = new PlayerBall(BallPosition, null, isDummy: true);

        DummyCheckpoint = new Checkpoint(MenuPosition + new XnaVector3(10f, -5f, -10f), scale: 0.5f);

        Piso = new Floor(MenuPosition + new XnaVector3(50f, -10f, -50f), width: 150f, length: 150f);
        ParedFondo = new Wall(MenuPosition + new XnaVector3(50f, 65f, -75f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, MathF.PI / 2f), 150f, 150f);
        ParedIzquierda = new Wall(MenuPosition + new XnaVector3(-25f, 65f, -50f), XnaQuaternion.CreateFromAxisAngle(XnaVector3.Forward, MathF.PI / 2f), 150f, 150f);

        StaticBoxes.Add(new StaticBox(position: MenuPosition + new XnaVector3(-9f, -3.5f, 13f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 0f), 2f));
        StaticBoxes.Add(new StaticBox(position: MenuPosition + new XnaVector3(20f, -4f, -5f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, 60f), 10f));
        StaticBoxes.Add(new StaticBox(position: MenuPosition + new XnaVector3(0f, -4f, -25f), rotation: XnaQuaternion.CreateFromAxisAngle(XnaVector3.Up, -60f), 10f));

        menuEntries = new Dictionary<Tuple<MenuState, MenuState>, MenuEntry[]>
        {
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.NoSubOptions),
                new MenuEntry[]
                {
                    new TextMenuEntry("Nuevo juego", newGameAction, FontsManager.LucidaConsole20),
                    new TextMenuEntry("Opciones de juego", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.BallSubOptions), FontsManager.LucidaConsole20),
                    new TextMenuEntry("Opciones de sonido", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.SoundSubOptions), FontsManager.LucidaConsole20),
                    new TextMenuEntry("Salir", exitGameAction, FontsManager.LucidaConsole20)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.BallSubOptions),
                new MenuEntry[]
                {
                    new SelectMenuEntry<BallType>(
                        "Tipo de bola",
                        new List<BallType> { BallType.Goma, BallType.Metal, BallType.Piedra },
                        (BallType selected) => {
                            GameState.BallType = selected;
                            resetBallAction.Invoke();
                            DummyPlayerBall.Reset();
                        },
                        FontsManager.LucidaConsole20
                    ),
                    new TextMenuEntry("Volver", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), FontsManager.LucidaConsole20)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.MainMenu, MenuState.SoundSubOptions),
                new MenuEntry[]
                {
                    GetMasterVolumeMenuEntry(),
                    GetBackgroundMusicVolumeMenuEntry(),
                    GetSoundEffectsVolumeMenuEntry(),
                    new TextMenuEntry("Volver", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), FontsManager.LucidaConsole20),
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.OptionsMenu, MenuState.NoSubOptions),
                new MenuEntry[]
                {
                    new TextMenuEntry("Seguir jugando", GameState.Resume, FontsManager.LucidaConsole20),
                    new TextMenuEntry("Opciones de sonido", SetCurrentMenuStateAction(MenuState.OptionsMenu, MenuState.SoundSubOptions), FontsManager.LucidaConsole20),
                    new TextMenuEntry("Volver al menu principal", SetCurrentMenuStateAction(MenuState.MainMenu, MenuState.NoSubOptions), FontsManager.LucidaConsole20),
                    new TextMenuEntry("Salir", exitGameAction, FontsManager.LucidaConsole20)
                }
            },
            {
                new Tuple<MenuState, MenuState>(MenuState.OptionsMenu, MenuState.SoundSubOptions),
                new MenuEntry[]
                {
                    GetMasterVolumeMenuEntry(),
                    GetBackgroundMusicVolumeMenuEntry(),
                    GetSoundEffectsVolumeMenuEntry(),
                    new TextMenuEntry("Volver", SetCurrentMenuStateAction(MenuState.OptionsMenu, MenuState.NoSubOptions), FontsManager.LucidaConsole20)
                }
            }
        };
    }

    public void ChangeToOptionsMenu()
    {
        this.mainCurrentMenuState = MenuState.OptionsMenu;
    }

    public void Update(KeyboardState currentState, float deltaTime, TargetCamera camera)
    {
        currentRotation += GetRotationIncrement() * deltaTime;

        DummyCheckpoint.Update(currentState, deltaTime, camera);
        DummyPlayerBall.UpdatePositionAndRotation(BallPosition, XnaQuaternion.CreateFromAxisAngle(XnaVector3.Right, currentRotation));

        if (WasKeyPressed(Keys.S, currentState) || WasKeyPressed(Keys.Down, currentState))
        {
            selectedOptionIndex = (selectedOptionIndex + 1) % menuEntries[GetCurrentMenuState()].Length;
        }
        else if (WasKeyPressed(Keys.W, currentState) || WasKeyPressed(Keys.Up, currentState))
        {
            selectedOptionIndex = (selectedOptionIndex - 1 + menuEntries[GetCurrentMenuState()].Length) % menuEntries[GetCurrentMenuState()].Length;
        }
        else if (WasKeyPressed(Keys.Enter, currentState))
        {
            menuEntries[GetCurrentMenuState()][selectedOptionIndex].Action.Invoke(Keys.Enter);
        }
        else if (WasKeyPressed(Keys.A, currentState) || WasKeyPressed(Keys.Left, currentState))
        {
            menuEntries[GetCurrentMenuState()][selectedOptionIndex].Action.Invoke(Keys.A);
        }
        else if (WasKeyPressed(Keys.D, currentState) || WasKeyPressed(Keys.Right, currentState))
        {
            menuEntries[GetCurrentMenuState()][selectedOptionIndex].Action.Invoke(Keys.D);
        }

        previousKeyboardState = currentState;
    }

    public void Draw(GameTime gameTime, GraphicsDevice graphicsDevice)
    {
        var eyePosition = this.Camera.Position;

        Piso.Draw(this.Camera.View, this.Camera.Projection, LightPosition, eyePosition, 0, Matrix.Identity, Matrix.Identity, null);
        ParedFondo.Draw(this.Camera.View, this.Camera.Projection, LightPosition, eyePosition, 0, Matrix.Identity, Matrix.Identity, null);
        ParedIzquierda.Draw(this.Camera.View, this.Camera.Projection, LightPosition, eyePosition, 0, Matrix.Identity, Matrix.Identity, null);
        DummyCheckpoint.Draw(this.Camera.View, this.Camera.Projection, LightPosition, eyePosition, 0, Matrix.Identity, Matrix.Identity, null);
        DummyPlayerBall.Draw(this.Camera.View, this.Camera.Projection, LightPosition, eyePosition, 0, Matrix.Identity, Matrix.Identity, null);

        foreach (StaticBox staticBox in StaticBoxes)
        {
            staticBox.Draw(this.Camera.View, this.Camera.Projection, LightPosition, eyePosition, 0, Matrix.Identity, Matrix.Identity, null);
        }

        var originalDepthStencilState = graphicsDevice.DepthStencilState;
        var originalBlendState = graphicsDevice.BlendState;

        graphicsDevice.DepthStencilState = DepthStencilState.None;
        graphicsDevice.BlendState = BlendState.AlphaBlend;

        var menuEntries = this.menuEntries[GetCurrentMenuState()];

        var startY = graphicsDevice.Viewport.Height * 0.3f;
        var spacing = FontsManager.LucidaConsole20.LineSpacing * 2.5f;
        var centerX = graphicsDevice.Viewport.Width / 2f;

        for (int i = 0; i < menuEntries.Length; i++)
        {
            var menuEntry = menuEntries[i];
            var textSize = menuEntry.Size;
            var position = new Vector2(centerX - textSize.X / 2f, startY + i * spacing);
            var textColor = (i == selectedOptionIndex) ? Color.Yellow : Color.White;

            int padding = 10;
            var backgroundRect = new Rectangle(
                (int)(position.X - padding),
                (int)(position.Y - padding),
                (int)(textSize.X + padding * 2),
                (int)(textSize.Y + padding * 2)
            );
            GameGlobals.SpriteBatch.Begin();
            GameGlobals.SpriteBatch.Draw(this.BackgroundMenuTexture, backgroundRect, Color.Black * 0.6f);
            GameGlobals.SpriteBatch.End();

            menuEntry.Draw(GameGlobals.SpriteBatch, textColor, position);
        }

        graphicsDevice.DepthStencilState = originalDepthStencilState;
        graphicsDevice.BlendState = originalBlendState;
    }

    private float GetRotationIncrement()
    {
        if (GameState.BallType == BallType.Metal)
        {
            return 4f;
        }

        if (GameState.BallType == BallType.Piedra)
        {
            return 0.5f;
        }

        return 1f;
    }

    private bool WasKeyPressed(Keys key, KeyboardState currentState)
    {
        return currentState.IsKeyDown(key) && !previousKeyboardState.IsKeyDown(key);
    }

    private Tuple<MenuState, MenuState> GetCurrentMenuState()
    {
        return new Tuple<MenuState, MenuState>(mainCurrentMenuState, subOptionMenuState);
    }

    private Action SetCurrentMenuStateAction(MenuState mainCurrentMenuState, MenuState subOptionMenuState)
    {
        return () =>
        {
            this.mainCurrentMenuState = mainCurrentMenuState;
            this.subOptionMenuState = subOptionMenuState;
            this.selectedOptionIndex = 0;
        };
    }

    private BarMenuEntry GetMasterVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen general",
            () =>
            {
                return GameState.MasterVolume;
            },
            (int newVolume) =>
            {
                GameState.MasterVolume = newVolume;
                MediaPlayer.Volume = GameState.MasterVolume / 100f * GameState.BackgroundMusicVolume / 100f;
                SoundEffect.MasterVolume = GameState.MasterVolume / 100f * GameState.SoundEffectsVolume / 100f;
            },
            FontsManager.LucidaConsole20);
    }

    private BarMenuEntry GetBackgroundMusicVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen musica de fondo",
            () =>
            {
                return GameState.BackgroundMusicVolume;
            },
            (int newVolume) =>
            {
                GameState.BackgroundMusicVolume = newVolume;
                MediaPlayer.Volume = GameState.MasterVolume / 100f * GameState.BackgroundMusicVolume / 100f;
            },
            FontsManager.LucidaConsole20);
    }

    private BarMenuEntry GetSoundEffectsVolumeMenuEntry()
    {
        return new BarMenuEntry("Volumen efectos de sonido",
            () =>
            {
                return GameState.SoundEffectsVolume;
            },
            (int newVolume) =>
            {
                GameState.SoundEffectsVolume = newVolume;
                SoundEffect.MasterVolume = GameState.MasterVolume / 100f * GameState.SoundEffectsVolume / 100f;
            },
            FontsManager.LucidaConsole20);
    }
}