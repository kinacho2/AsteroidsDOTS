using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Setup
{
    public class InitializeConfigs : MonoBehaviour
    {
        [SerializeField] Camera Camera;
        [SerializeField] Material LineMaterial;
        void Awake()
        {
            Configs.InitializeConfigs(Camera, LineMaterial);
            Destroy(gameObject);
        }

    }
}
