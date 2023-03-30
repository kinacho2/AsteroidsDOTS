using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct WeaponComponent : IComponentData
    {
        public int level;
        public float missileSpeed;
        public float missileLifeTime;
        public float range;
        public int missileAmount;
    }
}