using Asteroids.Data;
using Asteroids.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Setup
{
    public delegate void InitializedConfigs();
    public static class Configs
    {
        public static event InitializedConfigs OnInitializedConfig;
        public static float2 CameraLimits { get; private set; }
        public static GameObject MisilePrefab { get; private set; }
        public static AsteroidDataSO AsteroidDB { get; private set; }
        public static void InitializeConfigs(Camera camera, GameObject misilePrefab, AsteroidDataSO asteroidDB)
        {
            AsteroidDB = asteroidDB;
            MisilePrefab = misilePrefab;
            var worldPoint = camera.ViewportToWorldPoint(Vector2.one);
            CameraLimits = new float2(worldPoint.y * camera.pixelWidth / camera.pixelHeight, worldPoint.y);

            OnInitializedConfig?.Invoke();

        }

        public static float2 GetRandomPositionOutOfScreen()
        {
            var dir = AGeometry.RotateZ(math.right().ToFloat2(), Random.Range(0, math.PI * 2));
            var len = math.length(CameraLimits + new float2(1,1));
            return dir * len;
        }

        public static float2 GetRandomVelocity(float maxSpeed)
        {
            return AGeometry.RotateZ(math.right().ToFloat2(), Random.Range(0, math.PI * 2)) * Random.Range(0, maxSpeed);
        }
    }
}
