using Asteroids.ECS.Components;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;

public class BombCollision_System : SystemBase
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
            .ForEach((Entity entity, int entityInQueryIndex,
                ref BombComponent bomb,
                ref Translation tr,
                ref Rotation rot,
                ref Scale scale,
                in PhysicsColliderBlob collider) =>
            {

                bomb.radius += bomb.expansionSpeed * deltaTime;
                scale.Value = bomb.radius;
                bomb.lifeTime -= deltaTime;

                var allHits = new NativeList<OverlapColliderHit>(6 * 3 * 3 * 3, Allocator.Temp);

                var Collider = collider.Collider;
                ref var circle = ref Collider.GetColliderRef<PhysicsCircleCollider>();
                var geometry = circle.Geometry;
                geometry.Radius = bomb.radius * 0.5f;
                circle.Geometry = geometry;

                if (physicsWorld.OverlapCollider(
                    new OverlapColliderInput
                    {
                        Collider = Collider,
                        Transform = new PhysicsTransform(tr.Value, rot.Value),
                        Filter = collider.Collider.Value.Filter,
                    },
                    ref allHits))
                {
                    //Debug.Log("collision");
                    foreach (var hit in allHits)
                    {
                        var hitEntity = physicsWorld.AllBodies[hit.PhysicsBodyIndex].Entity;
                        if (HasComponent<AsteroidComponent>(hitEntity))
                        {
                            var asteroid = EntityManager.GetComponentData<AsteroidComponent>(hitEntity);
                            var health = EntityManager.GetComponentData<HealthComponent>(hitEntity);
                            var asteroidTr = EntityManager.GetComponentData<Translation>(hitEntity);
                            if (asteroid.lastBombID != bomb.ID)
                            {
                                health.health = 0;
                                asteroid.lastBombID = bomb.ID;
                                asteroid.explodeDirection = math.normalize(asteroidTr.Value - tr.Value).ToFloat2();
                                cmdBuffer.SetComponent(hitEntity, asteroid);
                                cmdBuffer.SetComponent(hitEntity, health);
                            }

                        }
                    }
                }

                allHits.Dispose();

                if (bomb.lifeTime <= 0)
                    cmdBuffer.DestroyEntity(entity);

            }).Run();

        cmdBuffer.Playback(EntityManager);
        cmdBuffer.Dispose();
    }
}
