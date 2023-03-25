using Asteroids.Data;
using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct PowerComponent : IComponentData
    {
        public PowerType type;
    }
}