using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct PlayerDataComponent : IComponentData
    {
        public int maxHealth;
        public int shieldHealth;
        public float maxSpeed;
        public float acceleration;
        public float rotationSpeedDeg;
        public float restitution;
        public float stunnedTime;
        public float shootCooldown;
    }
}