using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct EnemyComponent : IComponentData
    {
        public float viewDistance;
        public EnemyAIState AIState;
        public float stateTimer;
#if UNITY_EDITOR
        public UnityEngine.Color debugColor;
#endif
    }

    public enum EnemyAIState
    {
        Idle,
        Aggro,
        Attacking,
        Evading,
    }
}
