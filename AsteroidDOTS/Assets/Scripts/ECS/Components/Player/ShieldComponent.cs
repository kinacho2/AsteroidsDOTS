using Unity.Entities;

namespace Asteroids.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct ShieldComponent : IComponentData
    {
        public bool enabled;
        public bool firstDisabled;
    }
}