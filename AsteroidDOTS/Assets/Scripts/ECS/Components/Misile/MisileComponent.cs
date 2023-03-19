using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct MisileComponent : IComponentData
    {
        public float speed;
        public float range;
        public float timer;
    }
}