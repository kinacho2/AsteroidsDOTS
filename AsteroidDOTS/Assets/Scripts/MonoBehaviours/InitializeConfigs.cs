using Asteroids.Audio;
using Asteroids.Data;
using UnityEngine;

namespace Asteroids.Setup
{
    public class InitializeConfigs : MonoBehaviour
    {
        [SerializeField] Camera Camera;
        [SerializeField] GameDataSO GameData;
        [SerializeField] AsteroidDataSO AsteroidDB;
        [SerializeField] PowerDataSO PowerDB;
        [SerializeField] AudioDataSO AudioDB;
        [SerializeField] ShipDataSO PlayerData;
        [SerializeField] ShipDataSO EnemyData;
        [SerializeField] SoundManager SoundManager;
#if UNITY_EDITOR
        [SerializeField] bool DebugMode;
#endif

        void Start()
        {
            Configs.InitializeConfigs(Camera, SoundManager, GameData, PlayerData, EnemyData, AsteroidDB, PowerDB, AudioDB);
#if UNITY_EDITOR
            Configs.DebugMode = DebugMode;
#endif
            Destroy(gameObject);
        }

    }
}
