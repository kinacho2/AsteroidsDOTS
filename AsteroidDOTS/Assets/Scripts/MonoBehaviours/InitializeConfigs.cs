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
        void Awake()
        {
            Configs.InitializeConfigs(Camera, MisilePrefab, AsteroidDB);
            Destroy(gameObject);
        }

    }
}
