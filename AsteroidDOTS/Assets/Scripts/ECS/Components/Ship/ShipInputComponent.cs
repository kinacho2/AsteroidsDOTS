using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids.ECS.Components
{
    public struct ShipInputComponent : IComponentData
    {
        public float2 direction;
        public bool shoot;
    }
}