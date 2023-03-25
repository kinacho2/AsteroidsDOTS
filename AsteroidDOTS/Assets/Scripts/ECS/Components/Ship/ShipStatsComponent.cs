using Asteroids.Data;
using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct ShipStatsComponent : IComponentData
    {
        public EntityType entityType;
        public int shieldHealth;
        public float stunnedTimer;
        public float invTime;
        public float shootTimer;
    }
}