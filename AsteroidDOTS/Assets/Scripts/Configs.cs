using Asteroids.Audio;
using Asteroids.Data;
using Asteroids.Tools;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Asteroids.Setup
{
    public delegate void InitializedConfigs();
    public static class Configs
    {
        public const int EVENTS_QUEUE_COUNT = 10;
        public const int AUDIO_PLAY_COUNT = 15;

        public static event InitializedConfigs OnInitializedConfig;

#if UNITY_EDITOR
        public static bool DebugMode { get; set; }
#endif
        public static bool IsInitialized { get; private set; } = false;
        public static float2 CameraLimits { get; private set; }
        public static GameDataSO GameData { get; private set; }
        public static ShipDataSO PlayerData { get; private set; }
        public static ShipDataSO EnemyDB { get; private set; }
        public static AsteroidDataSO AsteroidDB { get; private set; }
        public static PowerDataSO PowerDB { get; private set; }
        //public static WeaponDataSO WeaponDB { get; private set; }
        public static AudioDataSO AudioDB { get; private set; }
        public static SoundManager SoundManager { get; private set; }

        public static void InitializeConfigs(Camera camera,
            SoundManager soundManager,
            GameDataSO gameData,
            ShipDataSO playerData,
            ShipDataSO enemyDB,
            AsteroidDataSO asteroidDB,
            PowerDataSO powerDB,
            AudioDataSO audioDB)
        {
            GameData = gameData;
            PlayerData = playerData;
            EnemyDB = enemyDB;
            AsteroidDB = asteroidDB;
            PowerDB = powerDB;
            AudioDB = audioDB;
            SoundManager = soundManager;
            soundManager.Initialize(AudioDB);

            var worldPoint = camera.ViewportToWorldPoint(Vector2.one);
            CameraLimits = new float2(worldPoint.y * camera.pixelWidth / camera.pixelHeight, worldPoint.y);
            CameraLimits += new float2(0.5f, 0.5f);

            OnInitializedConfig?.Invoke();
            IsInitialized = true;
        }

        public static float2 GetRandomPositionOutOfScreen(ref Random Random)
        {
            var dir = AGeometry.RotateZ(math.right().ToFloat2(), Random.NextFloat(0, math.PI * 2));
            var len = math.length(CameraLimits);
            return dir * (len + 1);
        }

        public static float2 GetRandomPositionInsideScreen(ref Random Random)
        {
            var discard = Random.NextFloat(-CameraLimits.x, CameraLimits.x);
            var pos = new float2(Random.NextFloat(-CameraLimits.x, CameraLimits.x), Random.NextFloat(-CameraLimits.y, CameraLimits.y));
            Debug.Log("(" + CameraLimits.x + "," + CameraLimits.y + ")");
            return pos;
        }

        public static float2 GetRandomVelocity(float maxSpeed, ref Random Random)
        {
            return AGeometry.RotateZ(math.right().ToFloat2(), Random.NextFloat(0, math.PI * 2)) * Random.NextFloat(0, maxSpeed);
        }
    }
}
