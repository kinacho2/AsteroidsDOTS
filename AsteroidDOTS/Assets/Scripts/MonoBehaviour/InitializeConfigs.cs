using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Setup
{
    public class InitializeConfigs : MonoBehaviour
    {
        [SerializeField] Camera Camera;
        void Awake()
        {
            Configs.InitializeConfigs(Camera);
            Destroy(gameObject);
        }

    }
}
