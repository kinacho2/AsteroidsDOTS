using Asteroids.ECS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace Asteroids.ECS.Systems
{
    public delegate void IntUpdate(int value, int maxValue);
    public delegate void FloatUpdate(float value, float maxValue);
    public class ShipStats_System : SystemBase
    {
        public static event IntUpdate OnHealthUpdate;
        public static event IntUpdate OnShieldUpdate;
        public static event FloatUpdate OnStunnedUpdate;

        private ShipStatsComponent _currentPlayerStats;
        private ShipStatsComponent _lastPlayerStats;

        private HealthComponent _currentPlayerHealth;
        private HealthComponent _lastPlayerHealth;
        private ShipDataComponent _playerData;

        protected override void OnCreate()
        {
            base.OnCreate();
            _lastPlayerStats = new ShipStatsComponent { shieldHealth = -1, stunnedTimer = 0, shootTimer = 0 };
            _lastPlayerHealth = new HealthComponent { health = -1 };
        }
        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

            Entities
            .WithoutBurst()
            .ForEach((Entity ship, int entityInQueryIndex,
                in ShipStatsComponent stats,
                in HealthComponent health,
                in ShipDataComponent data,
                in ShipRendererComponent rendererRef,
                in Translation tr
                ) =>
            {
                if (HasComponent<PlayerComponent>(ship))
                {
                    _currentPlayerStats = stats;
                    _currentPlayerHealth = health;
                    _playerData = data;

                    //shield
                    var shield = EntityManager.GetComponentData<ShieldComponent>(rendererRef.ShieldEntity);
                    if (stats.shieldHealth <= 0)
                    {
                        if (shield.enabled)
                        {
                            shield.enabled = false;

                            if (!shield.firstDisabled)
                                Events_System.OnPlayerLoseShield.PostEvent(new Events.PlayerLoseShield());
                            shield.firstDisabled = false;

                            cmdBuffer.SetComponent(rendererRef.ShieldEntity, shield);
                        }
                    }
                    else
                    {
                        if (!shield.enabled)
                        {
                            shield.enabled = true;
                            cmdBuffer.SetComponent(rendererRef.ShieldEntity, shield);
                        }
                    }
                }
                //health
                if (health.health <= 0)
                {
                    if (stats.entityType == Data.EntityType.Enemy)
                        cmdBuffer.DestroyEntity(ship);
                    Events_System.OnEntityDestroyed.PostEvent(new Events.EntityDestroyed() { position = tr.Value, entityType = stats.entityType, size = 1 });
                    return;
                }

                //invTime
                if (stats.invTime > 0)
                {
                    int value = (int)(stats.invTime * 10);
                    if (value % 2 == 0)
                    {
                        cmdBuffer.AddComponent<Disabled>(rendererRef.Renderer);
                    }
                    else
                        cmdBuffer.RemoveComponent<Disabled>(rendererRef.Renderer);
                }
                else
                {
                    cmdBuffer.RemoveComponent<Disabled>(rendererRef.Renderer);
                }
            })
            .Run();

            cmdBuffer.Playback(EntityManager);
            cmdBuffer.Dispose();
            if (_lastPlayerHealth.health != _currentPlayerHealth.health)
            {
                _lastPlayerHealth = _currentPlayerHealth;
                OnHealthUpdate?.Invoke(_currentPlayerHealth.health, _playerData.maxHealth);
            }
            if (_lastPlayerStats.stunnedTimer != _currentPlayerStats.stunnedTimer)
            {
                _lastPlayerStats.stunnedTimer = _currentPlayerStats.stunnedTimer;
                OnStunnedUpdate?.Invoke(_currentPlayerStats.stunnedTimer, _playerData.stunnedTime);
            }
            if (_lastPlayerStats.shieldHealth != _currentPlayerStats.shieldHealth)
            {
                _lastPlayerStats.shieldHealth = _currentPlayerStats.shieldHealth;
                OnShieldUpdate?.Invoke(_currentPlayerStats.shieldHealth, _playerData.shieldHealth);
            }
        }
    }
}