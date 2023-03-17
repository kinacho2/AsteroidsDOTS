using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct PlayerInputComponent : IComponentData
    {
        public float2 direction;
        public bool shoot;
    }
}