using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Asteroids.ECS.Components
{
    public struct PowerComponent : IComponentData
    {
        public int type;
    }
}