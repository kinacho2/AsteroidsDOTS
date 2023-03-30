using Asteroids.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
namespace Asteroids.ECS.Systems
{
    public class MissileMovement_System : SystemBase
    {
        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            float deltaTime = Time.DeltaTime;

            Entities
                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref MissileComponent stats,
                    ref Translation tr,
                    ref Rotation rot) =>
                {
                    var forward = math.mul(rot.Value, math.right());
                    tr.Value += forward * stats.speed * deltaTime;

                    stats.timer -= deltaTime;

                    if (stats.timer <= 0)
                        cmdBuffer.DestroyEntity(entity);
                })
                .Run();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }
    }
}