using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;

namespace Asteroids.ECS.Systems
{
    public class MissileCollision_System : SystemBase
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
                .ForEach((Entity missileEntity, int entityInQueryIndex,
                    in MissileComponent missile,
                    in Translation tr,
                    in Rotation rot) =>
                {
                    var forward = math.mul(rot.Value, math.right()).ToFloat2();
                    var end = tr.Value.ToFloat2() + forward * missile.range;
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
                        if (hitEntity != missile.owner && HasComponent<HealthComponent>(hitEntity))
                        {
                            var health = EntityManager.GetComponentData<HealthComponent>(hitEntity);
                            var asteroidTr = EntityManager.GetComponentData<Translation>(hitEntity);

                            if (HasComponent<AsteroidComponent>(hitEntity))
                            {
                                var asteroid = EntityManager.GetComponentData<AsteroidComponent>(hitEntity);
                                asteroid.explodeDirection = math.normalize((asteroidTr.Value - tr.Value).ToFloat2());
                                cmdBuffer.SetComponent(hitEntity, asteroid);
                            }
                            cmdBuffer.DestroyEntity(missileEntity);
                            if (HasComponent<ShipStatsComponent>(hitEntity))
                            {
                                var stats = EntityManager.GetComponentData<ShipStatsComponent>(hitEntity);
                                var data = EntityManager.GetComponentData<ShipDataComponent>(hitEntity);
                                if (stats.invTime <= 0)
                                {
                                    stats.invTime = data.invTime;
                                    if (stats.shieldHealth > 0)
                                        stats.shieldHealth--;
                                    else
                                        health.health--;
                                    cmdBuffer.SetComponent(hitEntity, stats);
                                }
                            }
                            else health.health--;

                            cmdBuffer.SetComponent(hitEntity, health);

                            Events_System.OnMissileHit.PostEvent(new Events.MissileHit { position = tr.Value });
                        }
                    }
                })
                .Run();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }
    }
}