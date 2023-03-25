using Unity.Entities;

namespace Asteroids.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct ShipRendererComponent : IComponentData
    {
        public Entity Renderer;
        public Entity ShieldEntity;
    }
}
