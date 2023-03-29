using Asteroids.Data;
using Asteroids.ECS.Components;
using Asteroids.Setup;
using Asteroids.Tools;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace Asteroids.ECS.Systems
{
    public abstract class ShipSpawn_System : SystemBase
    {
        protected Entity CreateShipPrefab(GameObject prefab, ShipData data)
        {
            InitializeLineShape(prefab, data);
            var settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, null);
            return GameObjectConversionUtility.ConvertGameObjectHierarchy(prefab, settings);
        }

        protected void InitializeLineShape(GameObject prefab, ShipData data)
        {
            var polygonCollider = prefab.GetComponent<PolygonCollider2D>();
            if (polygonCollider)
                polygonCollider.points = data.shape;

            var meshes = prefab.GetComponentsInChildren<MeshFilter>();
            var meshFilter = meshes.Where((x) => x.tag != "Shield").FirstOrDefault();
            meshFilter.sharedMesh = new Mesh();
            AMeshTools.CreateMeshWithMassCenter(data.shape, prefab.transform.localScale, meshFilter.sharedMesh);

            var shieldMeshFilter = meshes.Where((x) => x.tag == "Shield").FirstOrDefault();
            if (shieldMeshFilter)
                AMeshTools.CreateCircleMesh(shieldMeshFilter, 0.5f, 20);
        }

        protected Entity InstantiateShipEntity(EntityManager entityManager, Entity shipPrefab, ShipData data, WeaponData weapon, EntityType type)
        {
            var entity = entityManager.Instantiate(shipPrefab);
            entityManager.AddComponentData(entity, new LimitCheckComponent { cameraLimits = Configs.CameraLimits });
            entityManager.AddComponent<ShipInputComponent>(entity);
            entityManager.AddComponentData(entity, new ShipStatsComponent
            {
                entityType = type,
                stunnedTimer = 0,
                shieldHealth = 0,
            });
            entityManager.AddComponentData(entity, new HealthComponent
            {
                health = data.health
            });
            entityManager.AddComponentData(entity, new ShipDataComponent
            {
                maxHealth = data.health,
                shieldHealth = data.shieldHealth,
                acceleration = data.acceleration,
                maxSpeed = data.maxSpeed,
                restitution = data.restitution,
                rotationSpeedDeg = data.rotationSpeedDeg,
                stunnedTime = data.stunnedTime,
                invTime = data.invTime,
                shootCooldown = data.shootCooldown,
            });
            entityManager.AddComponentData(entity, new WeaponComponent
            {
                misileAmount = weapon.misileAmount,
                misileSpeed = weapon.misileSpeed,
                misileLifeTime = weapon.misileLifeTime,
                range = weapon.range,
                level = 0,
            });

            return entity;
        }
    }
}