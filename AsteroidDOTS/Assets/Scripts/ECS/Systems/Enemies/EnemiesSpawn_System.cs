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
        private Color[] debugColors;
#endif
        private Random Random;
        protected override void OnCreate()
        {
            base.OnCreate();
            Random = new Random(0x6E9249B6u);

            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
        }

        private void Configs_OnInitializedConfig()
        {

            var enemyDataSo = Configs.EnemyDB;
            EnemiesDB = enemyDataSo.Ships;
            Enemies = new NativeArray<Entity>(EnemiesDB.Length, Allocator.Persistent);

            for (int i = 0; i < EnemiesDB.Length; i++)
            {
                var data = EnemiesDB[i];
                Enemies[i] = CreateShipPrefab(enemyDataSo.ShipPrefab, data);
            }
#if UNITY_EDITOR
            debugColors = new Color[EnemiesDB.Length];
            debugColors[0] = Color.blue;
            debugColors[1] = Color.yellow;
            debugColors[2] = Color.white;
            debugColors[3] = Color.magenta;
#endif
            for (int i=0; i < Enemies.Length; i++)
            {
                var rand = Random.NextInt(0, Enemies.Length) % Enemies.Length;
                InstantiateEnemyEntity(EntityManager, Enemies[i], EnemiesDB[i], i);
            }
            //InstantiateEnemyEntity(EntityManager, data);
        }

        protected override void OnUpdate()
        {
            
        }

        private void InstantiateEnemyEntity(EntityManager entityManager, Entity entityPrefab, ShipData data, int index)
        {
            var weapon = Configs.EnemyDB.WeaponsDB.Get(index);
            var entity = InstantiateShipEntity(entityManager, entityPrefab, data, weapon, EntityType.Enemy);
            entityManager.AddComponentData(entity, new EnemyComponent { AIState = EnemyAIState.Idle, stateTimer = 0, viewDistance = 5
#if UNITY_EDITOR
                ,
                debugColor = debugColors[index]
#endif
            });
            var pos = Configs.GetRandomPositionOutOfScreen();
            entityManager.SetComponentData(entity, new Translation { Value = pos.ToFloat3() });
            var rot = quaternion.RotateZ(math.radians(Random.NextFloat(0, math.PI * 2)));
            entityManager.SetComponentData(entity, new Rotation { Value = rot });
        }


        protected override void OnDestroy()
        {
            Enemies.Dispose();
            base.OnDestroy();
        }
    }
}