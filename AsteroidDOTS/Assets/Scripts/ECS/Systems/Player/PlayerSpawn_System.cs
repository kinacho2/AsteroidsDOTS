using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Setup;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace Asteroids.ECS.Systems
{
    public class PlayerSpawn_System : ShipSpawner_System
    {
        private Entity EntityPrefab;
        protected override void OnCreate()
        {
            base.OnCreate();
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            var playerDataSO = Configs.PlayerData;
            var data = playerDataSO.Ships[0];
            EntityPrefab = CreateShipPrefab(playerDataSO.ShipPrefab, data);

            InstantiatePlayerEntity(EntityManager, EntityPrefab, data);
        }

        protected override void OnUpdate()
        {
            
        }

        private void InstantiatePlayerEntity(EntityManager entityManager, Entity entityPrefab, ShipData data)
        {
            var entity = InstantiateShipEntity(entityManager, entityPrefab, data);
            entityManager.AddComponent<PlayerComponent>(entity);
            var weaponData = Configs.WeaponDB.Get(0);
            entityManager.AddComponentData(entity, new PlayerWeaponComponent
            {
                misileAmount = weaponData.misileAmount,
                misileSpeed = weaponData.misileSpeed,
                misileLifeTime = weaponData.misileLifeTime,
                range = weaponData.range,
                type = 0,
            });
        }

    }
}