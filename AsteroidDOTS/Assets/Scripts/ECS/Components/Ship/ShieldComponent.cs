using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace Asteroids.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct ShieldComponent : IComponentData
    {
        public bool enabled;
        public bool firstDisabled;
    }
}