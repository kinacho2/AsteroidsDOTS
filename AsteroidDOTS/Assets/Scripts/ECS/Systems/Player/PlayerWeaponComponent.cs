using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Asteroids.ECS.Components
{
    public struct PlayerWeaponComponent : IComponentData
    {
        public float misileSpeed;
        public float misileLifeTime;
        public float range;
    }
}