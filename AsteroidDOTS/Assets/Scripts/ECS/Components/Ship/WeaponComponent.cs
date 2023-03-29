using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct WeaponComponent : IComponentData
    {
        public int level;
        public float misileSpeed;
        public float misileLifeTime;
        public float range;
        public int misileAmount;
    }
}