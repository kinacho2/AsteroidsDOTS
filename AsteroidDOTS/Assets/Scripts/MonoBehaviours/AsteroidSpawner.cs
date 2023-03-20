using Asteroids.ECS.Components;
using Asteroids.Setup;
using Asteroids.Tools;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.U2D.Entities.Physics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Asteroids.Core.Spawners
{
    public class AsteroidSpawner : MonoBehaviour
    {
        [SerializeField] GameObject Prefab;
        [SerializeField] int type;
        protected static Entity[] entityPrefabs;
        protected static World defaultWorld;
        protected static EntityManager entityManager;
        // Start is called before the first frame update
        void Start()
        {
            defaultWorld = World.DefaultGameObjectInjectionWorld;
            entityManager = defaultWorld.EntityManager;
            //Configs.OnInitializedConfig += Configs_OnInitializedConfig;
            Configs_OnInitializedConfig();
        }

        private void Configs_OnInitializedConfig()
        {
            var settings = GameObjectConversionSettings.FromWorld(defaultWorld, null);
            var asteroidData = Configs.AsteroidDB.Get(Data.AsteroidType.Bigger);

            entityPrefabs = new Entity[asteroidData.shapes.Length];

            for (int i = 0; i < entityPrefabs.Length; i++)
            {
                AMeshTools.InitializePolygonShape(Prefab, asteroidData.shapes[i].points);
                var collider = Prefab.GetComponent<PolygonCollider2D>();
                entityPrefabs[i] = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, settings);
                //entityManager.AddComponentData(entityPrefabs[i], GetPolygonCollider(collider));
            }


            //var entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(Prefab, settings);
            for (int i = 0; i < 6; i++)
            {
                InstantiateAsteroid(
                    entityPrefabs[Random.Range(0, entityPrefabs.Length)],
                    Configs.GetRandomPositionOutOfScreen(),
                    Configs.GetRandomVelocity(asteroidData.maxSpeed),
                    Random.Range(0, Mathf.PI / 2),
                    type);
            }
        }

        public static void InstantiateAsteroid(Entity entityPrefab, float2 position, float2 velocity, float angular, int type)
        {
            var entity = entityManager.Instantiate(entityPrefab);
            entityManager.AddComponent<LimitCheckComponent>(entity);
            entityManager.AddComponentData(entity, new AsteroidComponent { health = 3, type = type });
            entityManager.SetComponentData(entity, new PhysicsVelocity { Angular = angular, Linear = velocity });
            entityManager.SetComponentData(entity, new Translation { Value = position.ToFloat3() });
            //var mass = entityManager.GetComponentData<PhysicsMass>(entity);
            //entityManager.SetComponentData(entity, new PhysicsMass { InverseInertia = mass.InverseInertia, LocalCenterOfMass = mass.LocalCenterOfMass, InverseMass = 1 / Random.Range(5, 10) });
        }

    }
}
