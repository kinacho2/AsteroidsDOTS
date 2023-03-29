using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct AimComponent : IComponentData
    {
        public float timeAming;
        public float aimTimer;
        public float aimWidth;
    }
}
