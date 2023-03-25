using Asteroids.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Asteroids.ECS.Systems
{
    public class Explosion_System : SystemBase
    {

        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            float deltaTime = Time.DeltaTime;
            
            var parallelWriter = cmdBuffer.AsParallelWriter();
            Entities.WithAll<ExplosionComponent>()
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref ExplosionComponent explosion,
                    ref Scale scale) =>
                {

                    explosion.radius += explosion.expansionSpeed * deltaTime;
                    scale.Value = explosion.radius;
                    explosion.lifeTime -= deltaTime;

                    if (explosion.lifeTime <= 0)
                        parallelWriter.DestroyEntity(entityInQueryIndex, entity);
                    //EntityManager.DestroyEntity(entity);

                }).ScheduleParallel();

            Dependency.Complete();
            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }
    }
}