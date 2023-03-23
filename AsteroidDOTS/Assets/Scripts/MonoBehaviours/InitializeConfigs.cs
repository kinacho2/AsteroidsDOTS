using Asteroids.Audio;
using Asteroids.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Setup
{
    public class InitializeConfigs : MonoBehaviour
    {
        [SerializeField] Camera Camera;
        [SerializeField] GameObject MisilePrefab;
        [SerializeField] AsteroidDataSO AsteroidDB;
        [SerializeField] PowerDataSO PowerDB;
        [SerializeField] WeaponDataSO WeaponDB;
        [SerializeField] AudioDataSO AudioDB;
        [SerializeField] ShipDataSO PlayerData;
        [SerializeField] ShipDataSO EnemyData;
        [SerializeField] SoundManager SoundManager;

#if UNITY_EDITOR
        [SerializeField] bool DebugMode;
#endif
        void Awake()
        {
            Configs.InitializeConfigs(Camera, SoundManager, PlayerData, EnemyData, AsteroidDB, PowerDB, WeaponDB, AudioDB);

#if UNITY_EDITOR
            Configs.DebugMode = DebugMode;
#endif

            Destroy(gameObject);
        }

    }
}
