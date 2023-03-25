using Asteroids.Data;
using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids.ECS.Components
{
    public struct AsteroidComponent : IComponentData
    {
        public AsteroidType type;
        public float size;
        public float maxSpeed;
        public float2 explodeDirection;
        public uint lastBombID;
    }
}
