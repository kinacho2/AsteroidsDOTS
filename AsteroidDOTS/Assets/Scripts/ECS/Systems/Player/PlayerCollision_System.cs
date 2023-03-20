using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;
namespace Asteroids.ECS.Systems
{
    public class PlayerCollision_System : SystemBase
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

            var deltaTime = Time.DeltaTime;
            Entities
                .WithoutBurst()
                .ForEach((Entity player, int entityInQueryIndex,
                    ref PlayerStatsComponent stats,
                    ref PhysicsColliderBlob collider,
                    ref Translation tr,
                    ref Rotation rot,
                    ref PhysicsVelocity velocity,
                    in PlayerDataComponent data
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

                        var otherVelocity = EntityManager.GetComponentData<PhysicsVelocity>(hitEntity);
                        var otherMass = EntityManager.GetComponentData<PhysicsMass>(hitEntity);
                        var mass = EntityManager.GetComponentData<PhysicsMass>(player);

                        var len1 = math.length(velocity.Linear);
                        var len2 = math.length(otherVelocity.Linear);

                        //compute velocity
                        var m1 = mass.GetMass();
                        var m2 = otherMass.GetMass();
                        var invMass = 1.0f / (m1 + m2);
                        var v0_1 = otherVelocity.Linear - velocity.Linear;
                        var v0_2 = velocity.Linear - otherVelocity.Linear;
                        var v1 = 2 * m2 * v0_1 * invMass + velocity.Linear;
                        var v2 = 2 * m1 * v0_2 * invMass + otherVelocity.Linear;

                        var otherTranslation = EntityManager.GetComponentData<Translation>(hitEntity);

                        var dir = math.normalize(otherTranslation.Value - tr.Value);

                        var linear1 = math.normalize(v1 - dir.ToFloat2() * len1 * 1 / data.restitution) * math.length(v1);
                        var linear2 = math.normalize(v2 + dir.ToFloat2() * len2 * 0.5f) * math.length(v2);

                        var w1 = velocity.Angular;
                        var w2 = otherVelocity.Angular;

                        //compute angular velocity
                        if (math.lengthsq(velocity.Linear) > 0)
                        {
                            var fordward = math.normalize(velocity.Linear);
                            var cross = math.cross(fordward.ToFloat3(), dir);
                            var asin = math.asin(cross.z);

                            w1 = 2 * m2 * asin * math.length(v0_1) * invMass * data.restitution;
                            w2 = 2 * m1 * asin * math.length(v0_2) * invMass;

                            velocity.Angular = w1;
                        }
                        velocity.Linear = v1;
                        cmdBuffer.SetComponent(player, new PhysicsVelocity { Angular = w1, Linear = v1 });
                        cmdBuffer.SetComponent(hitEntity, new PhysicsVelocity { Angular = w2, Linear = v2 });

                        var tr2 = otherTranslation.Value + linear2.ToFloat3() * deltaTime;
                        var tr1 = tr.Value + linear1.ToFloat3() * deltaTime;
                        cmdBuffer.SetComponent(hitEntity, new Translation { Value = tr2 });
                        cmdBuffer.SetComponent(player, new Translation{ Value = tr1 });

                        if (stats.stunnedTimer <= 0)
                        {
                            //set stats
                            if (stats.shieldHealth <= 0)
                            {
                                stats.stunnedTimer = data.stunnedTime;
                                stats.health -= 1;
                            }
                            else
                            {
                                stats.shieldHealth -= 1;
                            }
                        }
                    }

                    stats.stunnedTimer = math.max(0, stats.stunnedTimer - deltaTime);

                })
                .Run();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }
    }
}