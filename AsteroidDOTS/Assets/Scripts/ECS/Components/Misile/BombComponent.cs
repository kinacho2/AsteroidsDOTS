using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct BombComponent : IComponentData
    {
        public uint ID;
        public float radius;
        public float expansionSpeed;
        public float lifeTime;
    }
}