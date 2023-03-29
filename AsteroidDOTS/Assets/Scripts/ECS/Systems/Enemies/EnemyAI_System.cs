using Asteroids.ECS.Components;
using Asteroids.Setup;
using Asteroids.Tools;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;
using UnityEngine;

namespace Asteroids.ECS.Systems
{
    public class EnemyAI_System : SystemBase
    {
        private PhysicsWorldSystem physicsWorldSystem;
        protected override void OnCreate()
        {
            base.OnCreate();
            physicsWorldSystem = World.GetExistingSystem<PhysicsWorldSystem>();
        }
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

            Entities
                .WithAll<ShipInputComponent, ShipStatsComponent, EnemyComponent>()
                .WithoutBurst()
                .ForEach((Entity entity, int entityInQueryIndex,
                    ref ShipInputComponent input,
                    ref ShipStatsComponent stats,
                    ref EnemyComponent enemyAI,
                    ref Translation tr,
                    ref Rotation rot,
                    in ShipDataComponent data
                    ) =>
                {
                    if (stats.stunnedTimer > 0) return;
                    ref var dir = ref input.direction;

                    switch (enemyAI.AIState)
                    {
                        case EnemyAIState.Idle:
                            EnemyIdleState(ref enemyAI, ref input, tr, rot, data, playerTr);
                            break;
                        case EnemyAIState.Aggro:
                            EnemyAggroState(ref enemyAI, ref input, ref stats, tr, rot, data, playerTr);
                            break;
                        case EnemyAIState.Attacking:
                            EnemyAttackingState(ref enemyAI, ref stats, ref input, tr, data, playerTr);
                            break;
                        case EnemyAIState.Evading:
                            EnemyEvadingState(ref enemyAI, ref input);
                            break;
                    }
                })
                .Run();

#if UNITY_EDITOR
            if (Configs.DebugMode)
            {
                Entities.WithAll<EnemyComponent>()
                    .WithoutBurst()
                    .ForEach((Entity entity, int entityInQueryIndex, ref Translation tr, ref Rotation rotation, ref PhysicsVelocity physics, in EnemyComponent enemyAI, in ShipDataComponent data
                        ) =>
                    {
                        var dspeed = 1 * data.acceleration * Time.DeltaTime;

                        //compute velocity
                        var velocity = physics.Linear;
                        var fordward = math.mul(rotation.Value, math.down()).ToFloat2();
                        velocity += fordward * dspeed;

                        Debug.DrawLine(tr.Value.ToVector3(), (tr.Value + velocity.ToFloat3() * Time.DeltaTime).ToVector3(), enemyAI.debugColor, 6);
                    })
                    .Run();
            }
#endif

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        private void EnemyIdleState(ref EnemyComponent enemyAI, ref ShipInputComponent input, in Translation tr, in Rotation rot, in ShipDataComponent data, in Translation playerTr)
        {
            ref var dir = ref input.direction;

            var viewDst = enemyAI.viewDistance;

            if (CheckForEvade(ref enemyAI, ref input, tr, rot, data))
                return;

            if (math.lengthsq(tr.Value.ToFloat2() - playerTr.Value.ToFloat2()) < viewDst * viewDst)
            {
#if UNITY_EDITOR
                if (Configs.DebugMode)
                {
                    Debug.DrawLine(tr.Value.ToVector3(), playerTr.Value.ToVector3(), Color.red, 1);
                }
#endif
                enemyAI.AIState = EnemyAIState.Aggro;
                return;
            }

            dir.y = 1;
            dir.x = 0;
        }

        private void EnemyEvadingState(ref EnemyComponent enemyAI, ref ShipInputComponent input)
        {
            enemyAI.stateTimer -= Time.DeltaTime;
            if (enemyAI.stateTimer <= 0)
            {
                enemyAI.AIState = EnemyAIState.Idle;
                return;
            }

        }

        private void EnemyAggroState(ref EnemyComponent enemyAI, ref ShipInputComponent input, ref ShipStatsComponent stats, in Translation tr, in Rotation rot, in ShipDataComponent data, in Translation playerTr)
        {
            ref var inputDir = ref input.direction;
            var viewDst = enemyAI.viewDistance;

            var dir = (playerTr.Value - tr.Value).ToFloat2();
            var dirNorm = math.normalize(dir);
            var forward = math.mul(rot.Value, math.down()).ToFloat2();
            var dot = math.dot(forward, dirNorm);

            if (math.abs(dot) > 0.01f)
            {
                inputDir.x = -math.sign(dot);
            }
            else if (stats.shootTimer <= 0)
            {
                enemyAI.AIState = EnemyAIState.Attacking;
                return;
            }
            else
            {
                stats.shootTimer -= Time.DeltaTime;
            }

            if (CheckForEvade(ref enemyAI, ref input, tr, rot, data))
                return;

            if (math.lengthsq(tr.Value.ToFloat2() - playerTr.Value.ToFloat2()) > viewDst * viewDst * 1.5f)
            {
                enemyAI.AIState = EnemyAIState.Idle;
            }

        }

        private void EnemyAttackingState(ref EnemyComponent enemyAI, ref ShipStatsComponent stats, ref ShipInputComponent input, in Translation tr, in ShipDataComponent data, in Translation playerTr)
        {
            input.shoot = true;
            stats.shootTimer = data.shootCooldown;
            enemyAI.AIState = EnemyAIState.Aggro;
#if UNITY_EDITOR
            Debug.DrawLine(tr.Value.ToVector3(), playerTr.Value.ToVector3(), enemyAI.debugColor, 0.5f);
#endif
        }

        private bool CheckForEvade(ref EnemyComponent enemyAI, ref ShipInputComponent input, in Translation tr, in Rotation rot, in ShipDataComponent data)
        {
            ref var dir = ref input.direction;

            var forward = math.mul(rot.Value, math.down()).ToFloat2();
            var perpendicular = new float2(forward.y, -forward.x);
            var forwardSqr = forward;
            forward *= enemyAI.viewDistance;

            var pos = tr.Value.ToFloat2();

            var ray0 = new RaycastInput
            {
                Start = pos,
                End = pos + forward,
                Filter = CollisionFilter.Default,
            };
            var ray1 = new RaycastInput
            {
                Start = pos + perpendicular * 0.3f,
                End = pos + perpendicular * 0.3f + forward,
                Filter = CollisionFilter.Default,
            };
            var ray2 = new RaycastInput
            {
                Start = pos - perpendicular * 0.3f,
                End = pos - perpendicular * 0.3f + forward,
                Filter = CollisionFilter.Default,
            };

            var physicsWorld = physicsWorldSystem.PhysicsWorld;

            //bool ray0 = 
            bool c0 = physicsWorld.CastRay(ray0, out var hit0);
            bool c1 = physicsWorld.CastRay(ray1, out var hit1);
            bool c2 = physicsWorld.CastRay(ray2, out var hit2);

            //check asteroid collision
            if (c0 || c1 || c2)
            {
                var hit = c0 ? hit0 : c1 ? hit1 : hit2;
                var hitEntity = physicsWorld.AllBodies[hit.PhysicsBodyIndex].Entity;
                if (HasComponent<PhysicsVelocity>(hitEntity))
                {
                    var otherVelocity = EntityManager.GetComponentData<PhysicsVelocity>(hitEntity);
                    var otherForward = math.normalize(otherVelocity.Linear);

                    dir.x = math.sign(math.cross(otherForward.ToFloat3(), forwardSqr.ToFloat3()).z);
                    enemyAI.AIState = EnemyAIState.Evading;
                    enemyAI.stateTimer = 1 / data.rotationSpeedDeg * 90;//perpendicular

#if UNITY_EDITOR
                    if (Configs.DebugMode)
                    {
                        Debug.DrawLine(pos.ToVector3(), (pos + forward).ToVector3(), Color.red, 1);
                        Debug.DrawLine((pos + perpendicular * 0.3f).ToVector3(), ((pos + perpendicular * 0.3f) + forward).ToVector3(), Color.red, 1);
                        Debug.DrawLine((pos - perpendicular * 0.3f).ToVector3(), ((pos - perpendicular * 0.3f) + forward).ToVector3(), Color.red, 1);
                    }
#endif
                    return true;
                }
            }

            return false;
        }
    }
}
