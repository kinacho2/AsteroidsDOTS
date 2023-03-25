using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Setup;
using Asteroids.Tools;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Asteroids.ECS.Systems
{
    public class EnemiesSpawn_System : ShipSpawn_System
    {
        private NativeArray<Entity> Enemies;
        private ShipData[] EnemiesDB;
#if UNITY_EDITOR
        private Color[] _debugColors;
#endif
        private Random Random;

        private float _spawnTime = 30;
        private float _spawnTimer;
        private int _enemyCounter = 8;

        protected override void OnCreate()
        {
            base.OnCreate();
            Random = new Random(0x6E0219B6u);

            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {
            var enemyDataSo = Configs.EnemyDB;
            EnemiesDB = enemyDataSo.Ships;
            Enemies = new NativeArray<Entity>(EnemiesDB.Length, Allocator.Persistent);
            var spawnData = Configs.GameData.EnemiesSpawnData;

            _enemyCounter = spawnData.entityCount;
            _spawnTime = spawnData.spawnSeconds;

            for (int i = 0; i < EnemiesDB.Length; i++)
            {
                var data = EnemiesDB[i];
                Enemies[i] = CreateShipPrefab(enemyDataSo.ShipPrefab, data);
            }

#if UNITY_EDITOR
            _debugColors = new Color[EnemiesDB.Length];
            _debugColors[0] = Color.blue;
            _debugColors[1] = Color.yellow;
            _debugColors[2] = Color.white;
            _debugColors[3] = Color.magenta;
#endif

            for (int i=0; i < spawnData.initialEntityCount; i++)
            {
                SpawnRandomEnemy(EntityManager);
                _enemyCounter--;
            }
        }

        protected override void OnUpdate()
        {
            if (_enemyCounter <= 0) return;
            if (_spawnTimer > _spawnTime)
            {
                var query = GetEntityQuery(typeof(EnemyComponent));
                var array = query.ToEntityArray(Allocator.Temp);
                if(array.Length < 3)
                {
                    SpawnRandomEnemy(EntityManager);
                    _enemyCounter--;
                    _spawnTimer = 0;
                }
                array.Dispose();
            }
            _spawnTimer += Time.DeltaTime;
        }


        private void SpawnRandomEnemy(EntityManager entityManager)
        {
            var rand = Random.NextInt(0, Enemies.Length) % Enemies.Length;
            InstantiateEnemyEntity(EntityManager, Enemies[rand], EnemiesDB[rand], rand);
        }

        private void InstantiateEnemyEntity(EntityManager entityManager, Entity entityPrefab, ShipData data, int index)
        {
            var weapon = Configs.EnemyDB.WeaponsDB.Get(index);
            var entity = InstantiateShipEntity(entityManager, entityPrefab, data, weapon, EntityType.Enemy);
            entityManager.AddComponentData(entity, new EnemyComponent { AIState = EnemyAIState.Idle, stateTimer = 0, viewDistance = 5,
#if UNITY_EDITOR
                debugColor = _debugColors[index]
#endif
            });
            entityManager.AddComponentData(entity, new AimComponent { aimWidth = 0.03f, timeAming = 0.05f });

            var pos = Configs.GetRandomPositionOutOfScreen(ref Random);
            entityManager.SetComponentData(entity, new Translation { Value = pos.ToFloat3() });
            var rot = quaternion.RotateZ(math.radians(Random.NextFloat(0, math.PI * 2)));
            entityManager.SetComponentData(entity, new Rotation { Value = rot });
            
            var renderRef = entityManager.GetComponentData<ShipRendererComponent>(entity);
            entityManager.RemoveComponent<Scale>(renderRef.ShieldEntity);
        }


        protected override void OnDestroy()
        {
            Enemies.Dispose();
            base.OnDestroy();
        }
    }
}