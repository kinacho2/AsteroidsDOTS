using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Setup;
using Unity.Entities;

namespace Asteroids.ECS.Systems
{
    public class PlayerSpawn_System : ShipSpawn_System
    {
        private Entity EntityPrefab;
        protected override void OnCreate()
        {
            base.OnCreate();
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            Configs.OnInitializedConfig -= Configs_OnInitializedConfig;

            var playerDataSO = Configs.PlayerData;
            var data = playerDataSO.Ships[0];
            EntityPrefab = CreateShipPrefab(playerDataSO.ShipPrefab, data);

            InstantiatePlayerEntity(EntityManager, EntityPrefab, data, Configs.GameData.HyperspaceTravelData);
        }

        protected override void OnUpdate()
        {

        }

        private void InstantiatePlayerEntity(EntityManager entityManager, Entity entityPrefab, ShipData data, HyperspaceTravelData travelData)
        {
            var weapon = Configs.PlayerData.WeaponsDB.Get(0);
            var entity = InstantiateShipEntity(entityManager, entityPrefab, data, weapon, EntityType.Player);
            entityManager.AddComponent<PlayerComponent>(entity);
            entityManager.AddComponentData(entity, new HyperspaceTravelComponent
            {
                state = HyperspaceTravelState.Enabled,
                chargeTimer = 0,
                timeAfterTravel = travelData.timeAfterTravel,
                timeReloading = travelData.timeReloading,
                timeBeforeTravel = travelData.timeBeforeTravel,
                chargingPressed = false,
            });
        }

    }
}