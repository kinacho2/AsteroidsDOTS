using Asteroids.ECS.Components;
using Unity.Collections;
using Unity.Entities;

public class Shield_System : SystemBase
{
    protected override void OnUpdate()
    {
        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

        //TODO(Need to change this code with SingletonEntity)
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
