using Unity.Entities;
using Unity.Mathematics;
namespace Asteroids.ECS.Components
{
    [GenerateAuthoringComponent]
    public struct PlayerStatsComponent : IComponentData
    {
        public int health;
        public float stunnedTimer;
        public float shootTimer;
    }
}