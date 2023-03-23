using Asteroids.Data;
using Unity.Entities;
using Unity.Mathematics;
namespace Asteroids.ECS.Components
{
    public struct ShipStatsComponent : IComponentData
    {
        //public int health;
        public EntityType entityType;
        public int shieldHealth;
        public float stunnedTimer;
        public float invTime;
        public float shootTimer;
    }
}