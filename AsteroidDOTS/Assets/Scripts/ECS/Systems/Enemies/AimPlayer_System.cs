using Asteroids.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Asteroids.ECS.Systems
{
    public class AimPlayer_System : SystemBase
    {
        protected override void OnUpdate()
        {
            //check Game state
            var gameState = GetEntityQuery(typeof(GameStateComponent)).GetSingleton<GameStateComponent>();
            if (gameState.state == GameState.Finished)
            {
                return;
            }

            EntityQuery query = GetEntityQuery(typeof(PlayerComponent), ComponentType.ReadOnly<Translation>());
            var playerTr = EntityManager.GetComponentData<Translation>(query.GetSingletonEntity());

            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

            var parallelWriter = cmdBuffer.AsParallelWriter();
            var deltaTime = Time.DeltaTime;
            Entities
                .WithAll<ShipInputComponent, ShipStatsComponent, EnemyComponent>()
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref AimComponent aim,
                    in EnemyComponent enemyAI,
                    in Translation tr,
                    in Rotation rot,
                    in ShipRendererComponent renderRef
                    ) =>
                {
                    if (enemyAI.AIState == EnemyAIState.Attacking)
                    {
                        aim.aimTimer = aim.timeAming;
                    }
                    else
                    {
                        if (aim.aimTimer <= 0)
                            parallelWriter.SetComponent(entityInQueryIndex, renderRef.ShieldEntity, new NonUniformScale { Value = new float3(0, 0, 1) });
                        else 
                            aim.aimTimer -= deltaTime;
                    }
                    if(aim.aimTimer > 0)
                    {
                        var dir = (playerTr.Value - tr.Value);
                        float dist = math.length(dir);
                        dir = math.normalize(dir);
                        var forward = math.mul(rot.Value, math.down());
                        var angle = math.atan2(dir.y, dir.x) - math.atan2(forward.y, forward.x);
                        var quat = quaternion.RotateZ(angle);
                        var pos = math.mul(quat, math.down());

                        parallelWriter.SetComponent(entityInQueryIndex, renderRef.ShieldEntity, new Translation { Value = pos * dist * 0.5f });
                        parallelWriter.SetComponent(entityInQueryIndex, renderRef.ShieldEntity, new NonUniformScale { Value = new float3(aim.aimWidth, dist, 1) });
                        parallelWriter.SetComponent(entityInQueryIndex, renderRef.ShieldEntity, new Rotation { Value = quat });
                        parallelWriter.SetComponent(entityInQueryIndex, renderRef.ShieldEntity, new Translation { Value = pos * dist * 0.5f });
                    }
                })
                .ScheduleParallel();

            Dependency.Complete();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }
    }
}