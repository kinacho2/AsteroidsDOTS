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
    public class PlayerMissileSpawn_System : SystemBase
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

            var weaponsDB = Configs.PlayerData.WeaponsDB;

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
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            var deltaTime = Time.DeltaTime;
            Entities
                .WithAll<PlayerComponent>()
                .WithoutBurst()
                .ForEach((Entity ship, int entityInQueryIndex,
                    ref ShipInputComponent input,
                    ref ShipStatsComponent stats,
                    in Translation translation,
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
                            var pos = translation.Value;

                            for (int i = 0; i < weapon.missileAmount; i++)
                            {
                                var angle = math.radians(MissileAngles[i]);
                                var rot = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-90) + angle));
                                var fordward = math.mul(rotation.Value, math.down()).ToFloat2();

                                var velocity = AGeometry.RotateZ(fordward, angle) * weapon.missileSpeed;

                                var len = math.length(physics.Linear);
                                if (len > 0)
                                {
                                    velocity += fordward * math.dot(fordward, physics.Linear / len) * len;
                                }
                                InstantiateMissile(translation.Value, rot, math.length(velocity), weapon, ship, ref cmdBuffer);
                            }

                            Events_System.OnEntityShoot.PostEvent(new EntityShoot { weapon = weapon.level, position = translation.Value });
                        }
                    }
                    else
                    {
                        stats.shootTimer -= deltaTime;
                    }
                })
            .Run();
            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        public void InstantiateMissile(float3 position, quaternion rotation, float speed, WeaponComponent weapon, Entity owner, ref EntityCommandBuffer cmdBuffer)
        {
            var entity = cmdBuffer.Instantiate(missileEntityPrefab);
            //cmdBuffer.AddComponent(entity, new LimitCheckComponent { cameraLimits = Configs.CameraLimits });
            cmdBuffer.AddComponent(entity, new MissileComponent { speed = speed, timer = weapon.missileLifeTime, range = weapon.range, owner = owner });
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