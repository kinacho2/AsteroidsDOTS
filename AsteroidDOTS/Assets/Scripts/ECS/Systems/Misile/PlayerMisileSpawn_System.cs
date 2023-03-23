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
    public class PlayerMisileSpawn_System : SystemBase
    {
        private Entity misileEntityPrefab;
        private World defaultWorld;

        private NativeArray<float> MisileAngles;
        //GameObject MisilePrefab;
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

            MisileAngles = new NativeArray<float>(weaponsDB.MisileAngleDeg, Allocator.Persistent);


            var MisilePrefab = weaponsDB.MisilePrefab;
            var polygonCollider = MisilePrefab.GetComponent<PolygonCollider2D>();
            var meshFilter = MisilePrefab.GetComponentInChildren<MeshFilter>();
            meshFilter.sharedMesh = new Mesh();
            AMeshTools.CreateMeshWithMassCenter(polygonCollider.points, MisilePrefab.transform.localScale, meshFilter.sharedMesh);

            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            misileEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(MisilePrefab, settings);
        }

        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);
            var deltaTime = Time.DeltaTime;
                Entities.WithAll<PlayerComponent>()
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

                            for (int i = 0; i < weapon.misileAmount; i++)
                            {
                                var angle = math.radians(MisileAngles[i]);
                                var rot = math.mul(rotation.Value, quaternion.RotateZ(math.radians(-90) + angle));
                                var fordward = math.mul(rotation.Value, math.down()).ToFloat2();

                                var velocity = AGeometry.RotateZ(fordward, angle) * weapon.misileSpeed;

                                var len = math.length(physics.Linear);
                                if (len > 0)
                                {
                                    velocity += fordward * math.dot(fordward, physics.Linear / len) * len;
                                }
                                InstantiateMisile(translation.Value, rot, math.length(velocity), weapon, ref cmdBuffer);
                            }

                            Events_System.OnEntityShoot.PostEvent(new EntityShoot { weapon = weapon.type, position = translation.Value });
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

        public void InstantiateMisile(float3 position, quaternion rotation, float speed, WeaponComponent weapon, ref EntityCommandBuffer cmdBuffer)
        {
            var entity = cmdBuffer.Instantiate(misileEntityPrefab);
            cmdBuffer.AddComponent<LimitCheckComponent>(entity);
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