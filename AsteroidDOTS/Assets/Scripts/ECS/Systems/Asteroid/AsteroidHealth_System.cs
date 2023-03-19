using Asteroids.ECS.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class AsteroidHealth_System : SystemBase
{
    protected override void OnUpdate()
    {
        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex,
                
                in AsteroidComponent asteroid
                ) =>
            {
                if (asteroid.health <= 0)
                {
                    cmdBuffer.DestroyEntity(entity);
                    //Spawn asteroids
                }
            })
            .Run();

        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }
}
