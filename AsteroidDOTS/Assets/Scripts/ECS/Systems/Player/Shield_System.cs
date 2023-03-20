using Asteroids.ECS.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class Shield_System : SystemBase
{
    protected override void OnUpdate()
    {
        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities.WithEntityQueryOptions(EntityQueryOptions.IncludeDisabled)
                //.WithoutBurst()
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
