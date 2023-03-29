using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct EnemyComponent : IComponentData
    {
        public EnemyAIState AIState;
        public float stateTimer;
        public float viewDistance;
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
