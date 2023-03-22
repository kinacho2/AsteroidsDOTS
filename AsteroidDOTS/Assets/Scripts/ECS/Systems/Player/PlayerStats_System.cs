using Asteroids.ECS.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;

namespace Asteroids.ECS.Systems
{
    public delegate void IntUpdate(int value, int maxValue);
    public delegate void FloatUpdate(float value, float maxValue);
    public class PlayerStats_System : SystemBase
    {
        public static event IntUpdate OnHealthUpdate;
        public static event IntUpdate OnShieldUpdate;
        public static event FloatUpdate OnStunnedUpdate;

        private PlayerStatsComponent _currentPlayerStats;
        private PlayerStatsComponent _lastPlayerStats;
        private PlayerDataComponent _playerData;

        protected override void OnCreate()
        {
            base.OnCreate();
            _lastPlayerStats = new PlayerStatsComponent { health = -1, shieldHealth = -1, stunnedTimer = 0, shootTimer = 0 };
        }
        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

            Entities
            .WithoutBurst()
            .ForEach((Entity player, int entityInQueryIndex,
                in PlayerStatsComponent stats,
                in PlayerDataComponent data,
                in PlayerRendererComponent rendererRef
                
                ) =>
            {
                _currentPlayerStats = stats;
                _playerData = data;


                //health
                if (stats.health <= 0)
                {
                    cmdBuffer.DestroyEntity(player);
                    Events_System.OnPlayerDestroyed.PostEvent(new Events.PlayerDestroyed());
                    return;
                }

                //shield
                var shield = EntityManager.GetComponentData<ShieldComponent>(rendererRef.ShieldEntity);
                if (stats.shieldHealth <= 0)
                {
                    if (shield.enabled)
                    {
                        shield.enabled = false;
                        cmdBuffer.SetComponent(rendererRef.ShieldEntity, shield);
                        Events_System.OnPlayerLoseShield.PostEvent(new Events.PlayerLoseShield());
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

                //invTime
                if(stats.invTime > 0)
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
            if (_lastPlayerStats.health != _currentPlayerStats.health)
            {
                _lastPlayerStats.health = _currentPlayerStats.health;
                OnHealthUpdate?.Invoke(_currentPlayerStats.health, _playerData.maxHealth);
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