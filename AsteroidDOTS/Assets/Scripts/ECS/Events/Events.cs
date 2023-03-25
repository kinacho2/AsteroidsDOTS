using Asteroids.Data;
using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids.ECS.Events
{
    public struct PlayerMove
    {
        public float3 position;
    }

    public struct EntityShoot
    {
        public int weapon;
        public float3 position;
    }

    public struct PlayerCollision
    {
        public bool shield;
        public float3 position;
    }

    public struct EntityDestroyed
    {
        public EntityType entityType;
        public float3 position;
        public float size;
    }

    public struct PlayerLoseShield
    {
        public float3 position;
    }

    public struct AsteroidsCollision
    {
        public AsteroidType type;
        public float3 position;
    }

    public struct AsteroidDestroyed
    {
        public AsteroidType type;
        public float3 position;
        public float size;
    }

    public struct PickPower
    {
        public Entity player;
        public PowerType type;
        public float3 position;
    }

    public struct MisileHit
    {
        public float3 position;
    }
}
