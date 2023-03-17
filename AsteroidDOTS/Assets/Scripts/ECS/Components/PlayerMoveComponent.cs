using Unity.Entities;
using Unity.Mathematics;
namespace Asteroids.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct PlayerMoveComponent : IComponentData
    {
        public float maxSpeed;
        public float3 velocity;
        public float acceleration;
        public float rotationSpeedDeg;

        public float2 cameraLimits;
    }
}