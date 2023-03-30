using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct MissileComponent : IComponentData
    {
        public Entity owner;
        public float speed;
        public float range;
        public float timer;
    }
}