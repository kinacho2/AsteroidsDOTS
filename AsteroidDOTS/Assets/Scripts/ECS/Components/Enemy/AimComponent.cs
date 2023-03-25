using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct AimComponent : IComponentData
    {
        public float timeAming;
        public float aimWidth;
        public float aimTimer;
    }
}
