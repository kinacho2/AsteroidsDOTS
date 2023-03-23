using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;

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
            var deltaTime = Time.DeltaTime;
            
            Entities
                .WithoutBurst()
                .ForEach((Entity asteroidEntity, int entityInQueryIndex,
                    ref PhysicsColliderBlob collider,
                    ref Translation tr,
                    ref Rotation rot,
                    ref PhysicsVelocity velocity,
                    in AsteroidComponent asteroid
                    ) =>
                {
                    var allHits = new NativeList<OverlapColliderHit>(Allocator.Temp);

                    if (physicsWorld.OverlapCollider(
                        new OverlapColliderInput
                        {
                            Collider = collider.Collider,
                            Transform = new PhysicsTransform(tr.Value, rot.Value),
                            Filter = collider.Collider.Value.Filter,
                        },
                        ref allHits))
                    {
                        var hit = allHits[0];

                        if (physicsWorld.AllBodies[hit.PhysicsBodyIndex].Entity.Index == asteroidEntity.Index && allHits.Length > 1)
                        {
                            hit = allHits[1];
                        }
                        var hitEntity = physicsWorld.AllBodies[hit.PhysicsBodyIndex].Entity;

                        if (HasComponent<AsteroidComponent>(hitEntity) && hitEntity.Index != asteroidEntity.Index)
                        {
                            var otherVelocity = EntityManager.GetComponentData<PhysicsVelocity>(hitEntity);

                            var len1 = math.length(velocity.Linear);
                            var len2 = math.length(otherVelocity.Linear);

                            if (len1 > len2)
                            {
                                var otherTranslation = EntityManager.GetComponentData<Translation>(hitEntity);
                                var otherAsteroid = EntityManager.GetComponentData<AsteroidComponent>(hitEntity);
                                var otherMass = EntityManager.GetComponentData<PhysicsMass>(hitEntity);
                                var mass = EntityManager.GetComponentData<PhysicsMass>(asteroidEntity);

                                //compute velocity
                                var m1 = mass.GetMass();
                                var m2 = otherMass.GetMass();
                                var invMass = 1.0f / (m1 + m2);
                                var v0_1 = otherVelocity.Linear - velocity.Linear;
                                var v0_2 = velocity.Linear - otherVelocity.Linear;
                                var v1 = 2 * m2 * v0_1 * invMass + velocity.Linear;
                                var v2 = 2 * m1 * v0_2 * invMass + otherVelocity.Linear;

                                var dir = math.normalize(otherTranslation.Value - tr.Value);


                                var v1len = math.clamp(math.length(v1), 0, asteroid.maxSpeed);
                                var v2len = math.clamp(math.length(v1), 0, asteroid.maxSpeed);
                                var linear1 = math.normalize(v1 - dir.ToFloat2() * len1 * 0.5f) * math.length(v1);
                                var linear2 = math.normalize(v2 + dir.ToFloat2() * len2 * 0.5f) * math.length(v2);

                                
                                var w1 = velocity.Angular;
                                var w2 = otherVelocity.Angular;

                                //compute angular velocity
                                if (math.lengthsq(velocity.Linear) > 0)
                                {
                                    var fordward = math.normalize(velocity.Linear);

                                    //rotation
                                    var cross = math.cross(fordward.ToFloat3(), dir);
                                    var asin = math.asin(cross.z);

                                    w1 = 2 * m2 * asin * math.length(v0_1) * invMass;
                                    w2 = 2 * m1 * asin * math.length(v0_2) * invMass;
                                }
                                cmdBuffer.SetComponent(hitEntity, new PhysicsVelocity
                                {
                                    Angular = w2,
                                    Linear = linear2
                                });
                                cmdBuffer.SetComponent(asteroidEntity, new PhysicsVelocity
                                {
                                    Angular = w1,
                                    Linear = linear1
                                });

                                //change translation on collision prevent another collision in the next frame
                                var tr2 = otherTranslation.Value + linear2.ToFloat3() * deltaTime;
                                var tr1 = tr.Value + linear1.ToFloat3() * deltaTime;
                                cmdBuffer.SetComponent(hitEntity, new Translation
                                {
                                    Value = tr2
                                });
                                cmdBuffer.SetComponent(asteroidEntity, new Translation
                                {
                                    Value = tr1
                                });

                                //tiny asteroids destruction when hit a medium or big asteroid
                                
                                var thisAsteroid = asteroid;
                                var tinyType = (int)AsteroidType.Tiny;
                                if (thisAsteroid.type == tinyType && otherAsteroid.type < tinyType - 1)
                                {
                                    thisAsteroid.health -= 1;
                                    thisAsteroid.explodeDirection = dir.ToFloat2();
                                    cmdBuffer.SetComponent(asteroidEntity, thisAsteroid);
                                }
                                if (otherAsteroid.type == tinyType && thisAsteroid.type < tinyType - 1)
                                {
                                    otherAsteroid.health -= 1;
                                    otherAsteroid.explodeDirection = dir.ToFloat2();
                                    cmdBuffer.SetComponent(hitEntity, otherAsteroid);
                                }

                                Events_System.OnAsteroidsCollision.PostEvent(new Events.AsteroidsCollision
                                {
                                    type = math.max(thisAsteroid.type, otherAsteroid.type),
                                    position = thisAsteroid.type > otherAsteroid.type? tr.Value : otherTranslation.Value
                                    
                                });
                            }
                        }
                    }

                    allHits.Dispose();
                })
                .Run();
            
            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }
    }
}