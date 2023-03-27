using Asteroids.ECS.Components;
using Asteroids.ECS.Systems;
using Unity.Collections;
using Unity.Entities;

public class Shield_System : SystemBase
{
    protected override void OnUpdate()
    {
        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled)
            .ForEach((Entity entity, int entityInQueryIndex,
                in ShieldComponent shield) =>
            {
                if (shield.enabled)
                {
                    cmdBuffer.RemoveComponent<Disabled>(entity);
                }
                else
                {
                    cmdBuffer.AddComponent<Disabled>(entity);
                }
            })
            .Run();
        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }
}
