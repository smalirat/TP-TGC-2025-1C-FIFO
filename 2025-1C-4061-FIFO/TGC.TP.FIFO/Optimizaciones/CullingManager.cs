using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TGC.TP.FIFO.Cameras;
using TGC.TP.FIFO.Globales;
using TGC.TP.FIFO.Menu;
using TGC.TP.FIFO.Objetos;

namespace TGC.TP.FIFO.Optimizaciones;

public static class CullingManager
{
    public static int VisibleGameObjectCount { get; private set; } = 0;
    public static int GameObjectCount { get; private set; } = 0;
    public static int PotentialGameObjectShadowableCount { get; private set; } = 0;
    public static int VisibleGameObjectShadowableCount { get; private set; } = 0;
    public static int PotentialGameObjectReflectableCount { get; private set; } = 0;
    public static int PotentialGameObjectReflectablePerFaceCount { get; private set; } = 0;
    public static int VisibleGameObjectReflectableCount { get; private set; } = 0;

    public static int PotentialDrawCalls => GameObjectCount + PotentialGameObjectShadowableCount + PotentialGameObjectReflectableCount;
    public static int RealDrawCalls => VisibleGameObjectCount + VisibleGameObjectShadowableCount + VisibleGameObjectReflectableCount;

    public static void RefreshFrustumCulling(List<IGameObject> gameObjects, TargetCamera playerBallCamera, LightCamera lightCamera, StaticCamera environmentMapCamera)
    {
        GameObjectCount = gameObjects.Count + 2; // +2 por skybox y pelota
        VisibleGameObjectCount = 2;

        VisibleGameObjectShadowableCount = 0;

        PotentialGameObjectReflectableCount = GameState.EnvironmentMapEnabled() ? 6 : 0; // 6 por skybox (una por cada cara del cubemap)
        VisibleGameObjectReflectableCount = GameState.EnvironmentMapEnabled() ? 6 : 0;

        foreach (var gameObject in gameObjects)
        {
            CheckVisibleFromPlayerBallCamera(gameObject, playerBallCamera);
            CheckVisibleFromLightCamera(gameObject, playerBallCamera, lightCamera);
            CheckVisibleFromEnvironmentMapCamera(gameObject, environmentMapCamera);
        }

        PotentialGameObjectShadowableCount = 80; // Hay 80 objetos dentro del escenario que pueden ser visibles desde la luz
        PotentialGameObjectReflectablePerFaceCount = GameState.EnvironmentMapEnabled() ? PotentialGameObjectReflectableCount / 6 : 0;
    }

    public static void CheckVisibleFromPlayerBallCamera(IGameObject gameObject, TargetCamera playerBallCamera)
    {
        gameObject.Visible = false;

        if (playerBallCamera.GetFrustum().Contains(gameObject.BoundingBox) != ContainmentType.Disjoint)
        {
            gameObject.Visible = true;
            VisibleGameObjectCount++;
        }
    }

    public static void CheckVisibleFromLightCamera(IGameObject gameObject, TargetCamera playerBallCamera, LightCamera lightCamera)
    {
        gameObject.VisibleForShadowMap = false;

        // Si el objeto es visible desde la luz y la camara del jugador esta mirando a lo que mira la luz
        if (lightCamera.GetFrustum().Contains(playerBallCamera.GetFrustum()) != ContainmentType.Disjoint &&
            lightCamera.GetFrustum().Contains(gameObject.BoundingBox) != ContainmentType.Disjoint)
        {
            gameObject.VisibleForShadowMap = true;
            VisibleGameObjectShadowableCount++;
        }
    }

    private static void CheckVisibleFromEnvironmentMapCamera(IGameObject gameObject, StaticCamera environmentMapCamera)
    {
        for (var face = CubeMapFace.PositiveX; face <= CubeMapFace.NegativeZ; face++)
        {
            switch (face)
            {
                default:
                case CubeMapFace.PositiveX:
                    environmentMapCamera.FrontDirection = -Vector3.UnitX;
                    environmentMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeX:
                    environmentMapCamera.FrontDirection = Vector3.UnitX;
                    environmentMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.PositiveY:
                    environmentMapCamera.FrontDirection = Vector3.Down;
                    environmentMapCamera.UpDirection = Vector3.UnitZ;
                    break;

                case CubeMapFace.NegativeY:
                    environmentMapCamera.FrontDirection = Vector3.Up;
                    environmentMapCamera.UpDirection = -Vector3.UnitZ;
                    break;

                case CubeMapFace.PositiveZ:
                    environmentMapCamera.FrontDirection = -Vector3.UnitZ;
                    environmentMapCamera.UpDirection = Vector3.Down;
                    break;

                case CubeMapFace.NegativeZ:
                    environmentMapCamera.FrontDirection = Vector3.UnitZ;
                    environmentMapCamera.UpDirection = Vector3.Down;
                    break;
            }

            environmentMapCamera.BuildView();

            if (GameState.EnvironmentMapEnabled())
            {
                PotentialGameObjectReflectableCount++;
            }

            if (GameState.EnvironmentMapEnabled() && environmentMapCamera.GetFrustum().Contains(gameObject.BoundingBox) != ContainmentType.Disjoint)
            {
                gameObject.VisibleForEnvironmentMap[face] = true;
                VisibleGameObjectReflectableCount++;
            }
            else
            {
                gameObject.VisibleForEnvironmentMap[face] = false;
            }
        }
    }
}