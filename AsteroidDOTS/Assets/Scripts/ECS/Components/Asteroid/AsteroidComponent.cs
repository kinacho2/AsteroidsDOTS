using Unity.Entities;

namespace Asteroids.ECS.Components
{
    public struct AsteroidComponent : IComponentData
    {
        public int health;
        public int type;
    }
}
