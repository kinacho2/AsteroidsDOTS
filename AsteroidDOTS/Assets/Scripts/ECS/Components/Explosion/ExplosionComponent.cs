using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct ExplosionComponent : IComponentData
    {
        public float radius;
        public float expansionSpeed;
        public float lifeTime;
    }
}
