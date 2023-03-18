using Asteroids.ECS.Components;
using Asteroids.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;
using UnityEngine;

public class PlayerCollision_System : SystemBase
{
    protected override void OnUpdate()
    {
        var physicsWorldSystem = World.GetExistingSystem<PhysicsWorldSystem>();
        var physicsWorld = physicsWorldSystem.PhysicsWorld;

        var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

        var deltaTime = Time.DeltaTime;
        Entities
            .WithoutBurst()
            .ForEach((Entity entity, int entityInQueryIndex, 
                ref PlayerStatsComponent stats,
                ref PhysicsColliderBlob collider,
                ref Translation tr, 
                ref Rotation rot,
                ref PhysicsVelocity velocity,
                in PlayerDataComponent data
                ) =>
            {
                if (stats.stunnedTimer <= 0)
                {
                    if (physicsWorld.OverlapCollider(
                        new OverlapColliderInput
                        {
                            Collider = collider.Collider,
                            Transform = new PhysicsTransform(tr.Value, rot.Value),
                            Filter = collider.Collider.Value.Filter
                        },
                        out OverlapColliderHit hit))
                    {
                        var hitEntity = physicsWorld.AllBodies[hit.PhysicsBodyIndex].Entity;

                        var otherVelocity = EntityManager.GetComponentData<PhysicsVelocity>(hitEntity);
                        var otherMass = EntityManager.GetComponentData<PhysicsMass>(hitEntity);
                        var mass = EntityManager.GetComponentData<PhysicsMass>(entity);

                        //compute velocity
                        var m1 = mass.GetMass();
                        var m2 = otherMass.GetMass();
                        var invMass = 1 / (m1 + m2);
                        var v0 = velocity.Linear - otherVelocity.Linear;
                        var v1 = (m1 - m2) * v0 * invMass * data.restitution;
                        var v2 = 2 * m1 * v0 * invMass * data.restitution;
                        velocity.Linear = v1;

                        //compute angular velocity
                        var otherTranslation = EntityManager.GetComponentData<Translation>(hitEntity);

                        var fordward = math.normalize(velocity.Linear);
                        var dir = math.normalize(otherTranslation.Value - tr.Value);
                        var cross = math.cross(fordward.ToFloat3(), dir);
                        var asin = math.asin(cross.z) * math.length(v0);
                        var w0 = velocity.Angular - otherVelocity.Angular;
                        var w1 = (m1 - m2) * asin * invMass * data.restitution;
                        var w2 = 2 * m1 * asin * invMass * data.restitution;

                        velocity.Angular = w1;
                        cmdBuffer.SetComponent(hitEntity, new PhysicsVelocity { Angular = otherVelocity.Angular + w2, Linear = v2 });

                        //set stats
                        stats.stunnedTimer = data.stunnedTime;
                        stats.health -= 1;

                        if(stats.health <= 0)
                        {
                            cmdBuffer.DestroyEntity(entity);
                        }
                    }
                }
                else
                {
                    stats.stunnedTimer -= deltaTime;
                }
            })
            .Run();

        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }
}
