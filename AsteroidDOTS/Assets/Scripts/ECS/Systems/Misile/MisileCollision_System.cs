using Asteroids.ECS.Components;
using Asteroids.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;
//using UnityEngine;

public class MisileCollision_System : SystemBase
{
    private PhysicsWorldSystem physicsWorldSystem;
    protected override void OnCreate()
    {
        base.OnCreate();
        physicsWorldSystem = World.GetExistingSystem<PhysicsWorldSystem>();
    }
    protected override void OnUpdate()
    {
        var physicsWorld = physicsWorldSystem.PhysicsWorld;
        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
        float deltaTime = Time.DeltaTime;

        Entities
            .WithoutBurst()
            .ForEach((Entity misile, int entityInQueryIndex,
                in MisileComponent stats,
                in Translation tr,
                in Rotation rot) =>
            {
                var forward = math.mul(rot.Value, math.right()).ToFloat2();
                var end = tr.Value.ToFloat2() + forward * stats.range;
                if (physicsWorld.CastRay(
                    new RaycastInput 
                    { 
                        Start = tr.Value.ToFloat2(), 
                        End = end,
                        Filter = CollisionFilter.Default,
                        
                    }, 
                    out var hit)) 
                {
                    var hitEntity = physicsWorld.AllBodies[hit.PhysicsBodyIndex].Entity;
                    if (HasComponent<AsteroidComponent>(hitEntity)) 
                    {
                        var asteroid = EntityManager.GetComponentData<AsteroidComponent>(hitEntity);
                        var asteroidTr = EntityManager.GetComponentData<Translation>(hitEntity);
                        asteroid.health--;
                        asteroid.explodeDirection = math.normalize((asteroidTr.Value - tr.Value).ToFloat2());

                        cmdBuffer.DestroyEntity(misile);
                        cmdBuffer.SetComponent(hitEntity, asteroid);

                    }
                    //cmdBuffer.DestroyEntity(misile);
                }
            })
            .Run();

        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }
}
