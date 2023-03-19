using Asteroids.ECS.Components;
using Asteroids.Setup;
using Asteroids.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;
using UnityEngine;

public class MisileSpawn_System : SystemBase
{
    private Entity misileEntityPrefab;
    private World defaultWorld;

    protected override void OnCreate()
    {
        base.OnCreate();
        Configs.OnInitializedConfig += Configs_OnInitializedConfig;
       
    }

    private void Configs_OnInitializedConfig()
    {
        Configs.OnInitializedConfig -= Configs_OnInitializedConfig;
        //TODO Misile DB
        AMeshTools.InitializePolygonShape(Configs.MisilePrefab, Configs.MisilePrefab.GetComponent<PolygonCollider2D>().points);
        defaultWorld = World.DefaultGameObjectInjectionWorld;
        var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
        misileEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Configs.MisilePrefab, settings);
    }

    protected override void OnUpdate()
    {
        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
        var deltaTime = Time.DeltaTime;
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex,
                ref PlayerInputComponent input,
                ref PlayerStatsComponent stats,
                in Translation translation,
                in Rotation rotation,
                in PhysicsVelocity physics,
                in PlayerDataComponent data
                ) =>
            {
                if (stats.shootTimer <= 0)
                {
                    if (stats.stunnedTimer <=0 && input.shoot)
                    {
                        stats.shootTimer = data.shootCooldown;
                        input.shoot = false;
                        var pos = translation.Value;
                        var rot = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-90)));
                        var fordward = math.mul(rotation.Value, math.down()).ToFloat2();
                        
                        var velocity = fordward * 5;

                        var len = math.length(physics.Linear);
                        if(len > 0)
                        {
                            velocity += fordward * math.dot(fordward, physics.Linear/len) * len;
                        }
                        InstantiateMisile(translation.Value, rot, math.length(velocity), 0, ref cmdBuffer);
                    }
                }
                else
                {
                    stats.shootTimer -= deltaTime;
                }

            })
            .Run();
        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }

    public void InstantiateMisile(float3 position, quaternion rotation, float speed, float angular, ref EntityCommandBuffer cmdBuffer)
    {
        var entity = cmdBuffer.Instantiate(misileEntityPrefab);
        cmdBuffer.AddComponent<LimitCheckComponent>(entity);
        cmdBuffer.AddComponent(entity, new MisileComponent { speed = speed, timer = 1, range = 1});
        //cmdBuffer.SetComponent(entity, new PhysicsVelocity { Angular = angular, Linear = velocity });
        cmdBuffer.SetComponent(entity, new Translation { Value = position });
        cmdBuffer.SetComponent(entity, new Rotation { Value = rotation });
    }
}
