using Unity.Entities;
using Unity.Mathematics;

namespace Asteroids.ECS.Components
{
    public struct LimitCheckComponent : IComponentData
    {
        public float2 cameraLimits;
    }
}