using Asteroids.ECS.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class PowerUpCheck_System : SystemBase
{
    protected override void OnUpdate()
    {
        bool enableShield = Input.GetKey(KeyCode.R);

        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities.WithAll<PlayerShieldComponent>()
                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex,
                    in Translation tr,
                    in PlayerShieldComponent shieldRef
                    ) =>
                {
                    var shield = EntityManager.GetComponentData<ShieldComponent>(shieldRef.ShieldEntity);
                    /*
                    if (!shield.enabled && enableShield)
                    {
                        shield.enabled = true;
                        cmdBuffer.SetComponent(shieldRef.ShieldEntity, shield);
                    }
                    if(shield.enabled && !enableShield)
                    {
                        shield.enabled = false;
                        cmdBuffer.SetComponent(shieldRef.ShieldEntity, shield);
                    }/**/
                    //cmdBuffer.SetComponent(shieldRef.ShieldEntity, tr);
                })
                .Run();

        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }
}
