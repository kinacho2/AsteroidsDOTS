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
    public class EnemyMissileSpawn_System : SystemBase
    {
        private Entity missileEntityPrefab;
        private World defaultWorld;
        private NativeArray<float> MissileAngles;

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

            MissileAngles = new NativeArray<float>(weaponsDB.MissileAngleDeg, Allocator.Persistent);

            var MissilePrefab = weaponsDB.MissilePrefab;
            var points = weaponsDB.MissileShape;
            var meshFilter = MissilePrefab.GetComponentInChildren<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            AMeshTools.CreateMeshWithMassCenter(points, MissilePrefab.transform.localScale, meshFilter.sharedMesh);

            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            missileEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(MissilePrefab, settings);
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

                            var fordward = math.normalize((playerTr.Value - tr.Value).ToFloat2());

                            var rot = math.atan2(fordward.y, fordward.x);

                            var velocity = fordward * weapon.missileSpeed;

                            InstantiateMissile(tr.Value, quaternion.RotateZ(rot), math.length(velocity), weapon, ref cmdBuffer);

                            Events_System.OnEntityShoot.PostEvent(new EntityShoot { weapon = weapon.level, position = tr.Value });
                        }
                    }
                    else
                        stats.shootTimer -= deltaTime;

                })
                .Run();
            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        public void InstantiateMissile(float3 position, quaternion rotation, float speed, WeaponComponent weapon, ref EntityCommandBuffer cmdBuffer)
        {
            var entity = cmdBuffer.Instantiate(missileEntityPrefab);
            //cmdBuffer.AddComponent(entity, new LimitCheckComponent { cameraLimits = Configs.CameraLimits });
            cmdBuffer.AddComponent(entity, new MissileComponent { speed = speed, timer = weapon.missileLifeTime, range = weapon.range });
            cmdBuffer.SetComponent(entity, new Translation { Value = position });
            cmdBuffer.SetComponent(entity, new Rotation { Value = rotation });
        }

        protected override void OnDestroy()
        {
            MissileAngles.Dispose();
            base.OnDestroy();
        }
    }
}