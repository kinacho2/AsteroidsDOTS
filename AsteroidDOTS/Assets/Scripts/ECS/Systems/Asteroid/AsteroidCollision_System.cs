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

namespace Asteroids.ECS.Systems
{
    public class AsteroidCollision_System : SystemBase
    {
        private PhysicsWorldSystem physicsWorldSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            physicsWorldSystem = World.GetExistingSystem<PhysicsWorldSystem>();
        }
        protected override void OnUpdate()
        {
            //if (true) return;
            var physicsWorld = physicsWorldSystem.PhysicsWorld;
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

            Entities
                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref PhysicsColliderBlob collider,
                    ref Translation tr,
                    ref Rotation rot,
                    ref PhysicsVelocity velocity,
                    in AsteroidComponent asteroid
                    ) =>
                {
                    if (physicsWorld.OverlapCollider(
                        new OverlapColliderInput
                        {
                            Collider = collider.Collider,
                            Transform = new PhysicsTransform(tr.Value, rot.Value),
                            Filter = collider.Collider.Value.Filter,
                        },
                        out var hit))
                    {
                        var hitEntity = physicsWorld.AllBodies[hit.PhysicsBodyIndex].Entity;

                        if (HasComponent<AsteroidComponent>(hitEntity) && hitEntity.Index != entity.Index)
                        {
                            
                            var otherVelocity = EntityManager.GetComponentData<PhysicsVelocity>(hitEntity);
                            var otherMass = EntityManager.GetComponentData<PhysicsMass>(hitEntity);
                            var mass = EntityManager.GetComponentData<PhysicsMass>(entity);

                            var otherTranslation = EntityManager.GetComponentData<Translation>(hitEntity);

                            //compute velocity
                            var m1 = mass.GetMass();
                            var m2 = otherMass.GetMass();
                            var invMass = 1 / (m1 + m2);
                            var v0 = velocity.Linear - otherVelocity.Linear;
                            var v1 = (m1 - m2 * 0.5f) * v0 * invMass;
                            var v2 = 2 * m1 * v0 * invMass;


                            //compute angular velocity

                            if (math.lengthsq(velocity.Linear) > 0)
                            {
                                var fordward = math.normalize(velocity.Linear);
                                var dir = math.normalize(otherTranslation.Value - tr.Value);

                                //rotation
                                var cross = math.cross(fordward.ToFloat3(), dir);
                                var asin = math.asin(cross.z) * math.length(v0);
                                //var w0 = otherVelocity.Angular - velocity.Angular;
                                var w1 = (m1 - m2 * 0.5f) * asin * invMass;
                                var w2 = 2 * m1 * asin * invMass;

                                //velocity.Angular = w1;
                                //velocity.Linear = -dir.ToFloat2() * math.length(v1);
                                cmdBuffer.SetComponent(entity, new PhysicsVelocity { Angular = w1, Linear = 
                                    math.normalize(
                                    -dir.ToFloat2()*4
                                    + v1
                                    ) 
                                    //* math.length(velocity.Linear)
                                    * math.length(v1) 
                                });
                            }
                            else
                            {
                                //velocity.Linear = v1;
                                cmdBuffer.SetComponent(entity, new PhysicsVelocity { Angular = velocity.Angular, Linear = v1 });

                            }
                            //cmdBuffer.SetComponent(entity, new AsteroidComponent { health = asteroid.health - 1 });

                            //asteroid.health--;
                        }


                    }
                   
                })
                .Run();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }
    }
}