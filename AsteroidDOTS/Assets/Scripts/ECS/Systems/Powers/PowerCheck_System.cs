using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Setup;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;
using UnityEngine;

namespace Asteroids.ECS.Systems {
    public class PowerCheck_System : SystemBase
    {
        private PhysicsWorldSystem physicsWorldSystem;
        private NativeArray<WeaponData> Weapons;
        protected override void OnCreate()
        {
            base.OnCreate();
            physicsWorldSystem = World.GetExistingSystem<PhysicsWorldSystem>();
            
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            Configs.OnInitializedConfig -= Configs_OnInitializedConfig;

            Weapons = new NativeArray<WeaponData>(Configs.PlayerData.WeaponsDB.Weapons, Allocator.Persistent);

        }

        protected override void OnUpdate()
        {
            var physicsWorld = physicsWorldSystem.PhysicsWorld;
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

            Entities.WithAll<ShipRendererComponent, PlayerComponent>()
                    .WithoutBurst()
                    .ForEach((Entity player, int entityInQueryIndex,
                        ref PhysicsColliderBlob collider,
                        ref Translation tr,
                        ref Rotation rot,
                        ref ShipStatsComponent stats,
                        ref HealthComponent health,
                        ref ShipDataComponent data
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
                            if (HasComponent<PowerComponent>(hitEntity))
                            {

                                var power = EntityManager.GetComponentData<PowerComponent>(hitEntity);
                                var powerTr = EntityManager.GetComponentData<Translation>(hitEntity);
                                switch (power.type)
                                {
                                    case PowerType.Shield:
                                        stats.shieldHealth = data.shieldHealth;
                                        
                                        break;
                                    case PowerType.Weapon:
                                        var weapon = EntityManager.GetComponentData<WeaponComponent>(player);
                                        if(weapon.type + 1 < Weapons.Length)
                                        {
                                            weapon.type++;
                                            weapon.misileAmount = Weapons[weapon.type].misileAmount;
                                            weapon.misileLifeTime = Weapons[weapon.type].misileLifeTime;
                                            weapon.misileSpeed = Weapons[weapon.type].misileSpeed;
                                            weapon.range = Weapons[weapon.type].range;
                                            cmdBuffer.SetComponent(player, weapon);
                                        }
                                        break;
                                    case PowerType.Health:
                                        health.health = math.min(health.health + 2, data.maxHealth);
                                        break;
                                }

                                cmdBuffer.DestroyEntity(hitEntity);

                                Events_System.OnPickPower.PostEvent(new Events.PickPower { position = tr.Value, type = power.type, player = player });
                            }
                    }
                    })
                    .Run();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
        }

        protected override void OnDestroy()
        {
            Weapons.Dispose();
            base.OnDestroy();
        }
    }
}