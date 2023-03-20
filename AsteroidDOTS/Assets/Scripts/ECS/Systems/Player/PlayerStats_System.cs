using Asteroids.ECS.Components;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

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
        protected override void OnUpdate()
        {
            var cmdBuffer = new EntityCommandBuffer(Allocator.TempJob);

            Entities
            .WithoutBurst()
            .ForEach((Entity player, int entityInQueryIndex,
                in PlayerStatsComponent stats,
                in PlayerDataComponent data,
                in PlayerShieldComponent shieldRef
                ) =>
            {
                _currentPlayerStats = stats;
                _playerData = data;

                if (stats.health <= 0)
                {
                    cmdBuffer.DestroyEntity(player);
                }
                if (stats.shieldHealth <= 0)
                {
                    var shield = EntityManager.GetComponentData<ShieldComponent>(shieldRef.ShieldEntity);
                    if (shield.enabled)
                    {
                        shield.enabled = false;
                        cmdBuffer.SetComponent(shieldRef.ShieldEntity, shield);
                    }
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