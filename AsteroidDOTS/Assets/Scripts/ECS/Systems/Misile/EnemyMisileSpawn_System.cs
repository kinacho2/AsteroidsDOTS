using Asteroids.ECS.Components;
using Asteroids.ECS.Events;
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
    public class EnemyMisileSpawn_System : SystemBase
    {
        private Entity misileEntityPrefab;
        private World defaultWorld;
        private NativeArray<float> MisileAngles;

        protected override void OnCreate()
        {
            base.OnCreate();
            defaultWorld = World.DefaultGameObjectInjectionWorld;

            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            Configs.OnInitializedConfig -= Configs_OnInitializedConfig;

            var weaponsDB = Configs.EnemyDB.WeaponsDB;

            MisileAngles = new NativeArray<float>(weaponsDB.MisileAngleDeg, Allocator.Persistent);

            var MisilePrefab = weaponsDB.MisilePrefab;
            var points = weaponsDB.MisileShape;
            var meshFilter = MisilePrefab.GetComponentInChildren<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            AMeshTools.CreateMeshWithMassCenter(points, MisilePrefab.transform.localScale, meshFilter.sharedMesh);

            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            misileEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(MisilePrefab, settings);
        }

        protected override void OnUpdate()
        {
            //check Game state
            var gameState = GetEntityQuery(typeof(GameStateComponent)).GetSingleton<GameStateComponent>();
            if (gameState.state == GameState.Finished)
                return;

            EntityQuery query = GetEntityQuery(typeof(PlayerComponent), ComponentType.ReadOnly<Translation>());
            var playerTr = EntityManager.GetComponentData<Translation>(query.GetSingletonEntity());

            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            var deltaTime = Time.DeltaTime;
            Entities
                .WithAll<EnemyComponent>()
                .WithoutBurst()
                .ForEach((Entity ship, int entityInQueryIndex,
                    ref ShipInputComponent input,
                    ref ShipStatsComponent stats,
                    in Translation tr,
                    in Rotation rotation,
                    in PhysicsVelocity physics,
                    in ShipDataComponent data
                    ) =>
                {
                    if (stats.shootTimer <= 0)
                    {
                        if (stats.stunnedTimer <= 0 && input.shoot)
                        {
                            var weapon = EntityManager.GetComponentData<WeaponComponent>(ship);
                            stats.shootTimer = data.shootCooldown;
                            input.shoot = false;
                            var pos = tr.Value;

                            //var angle = math.radians(MisileAngles[i]);
                            var fordward = math.normalize((playerTr.Value - tr.Value).ToFloat2());

                            var rot = math.atan2(fordward.y, fordward.x);

                            var velocity = fordward * weapon.misileSpeed;

                            InstantiateMisile(tr.Value, quaternion.RotateZ(rot), math.length(velocity), weapon, ref cmdBuffer);

                            Events_System.OnEntityShoot.PostEvent(new EntityShoot { weapon = weapon.type, position = tr.Value });
                        }
                    }
                    else
                        stats.shootTimer -= deltaTime;

                })
                .Run();
            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        public void InstantiateMisile(float3 position, quaternion rotation, float speed, WeaponComponent weapon, ref EntityCommandBuffer cmdBuffer)
        {
            var entity = cmdBuffer.Instantiate(misileEntityPrefab);
            //cmdBuffer.AddComponent(entity, new LimitCheckComponent { cameraLimits = Configs.CameraLimits });
            cmdBuffer.AddComponent(entity, new MisileComponent { speed = speed, timer = weapon.misileLifeTime, range = weapon.range });
            cmdBuffer.SetComponent(entity, new Translation { Value = position });
            cmdBuffer.SetComponent(entity, new Rotation { Value = rotation });
        }

        protected override void OnDestroy()
        {
            MisileAngles.Dispose();
            base.OnDestroy();
        }
    }
}