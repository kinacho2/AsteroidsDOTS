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
        void Awake()
        {
            Configs.InitializeConfigs(Camera, AsteroidDB, PowerDB, WeaponDB, AudioDB);
            Destroy(gameObject);
        }

    }
}
