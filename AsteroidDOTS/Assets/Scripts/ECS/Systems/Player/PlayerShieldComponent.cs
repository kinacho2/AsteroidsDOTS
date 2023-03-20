using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace Asteroids.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct PlayerShieldComponent : IComponentData
    {
        public Entity ShieldEntity;
    }
}
