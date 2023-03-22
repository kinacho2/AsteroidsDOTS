
using Unity.Mathematics;

namespace Asteroids.ECS.Events
{
    public struct PlayerMove
    {
        public float3 position;
    }

    public struct PlayerShoot
    {
        public int weapon;
        public float3 position;
    }

    public struct PlayerCollision
    {
        public bool shield;
        public float3 position;
    }

    public struct PlayerDestroyed
    {
        public float3 position;
    }

    public struct PlayerLoseShield
    {
        public float3 position;
    }

    public struct AsteroidsCollision
    {
        public int type;
        public float3 position;
    }

    public struct AsteroidDestroyed
    {
        public int type;
        public float3 position;
    }

    public struct PickPower
    {
        public int type;
        public float3 position;
    }

    public struct MisileHit
    {
        public float3 position;
    }

}
