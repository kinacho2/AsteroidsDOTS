using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Asteroids.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Asteroid Data", fileName = "AsteroidData.asset", order = 0)]

    public class AsteroidDataSO : ScriptableObject
    {
        [field: SerializeField]
        public GameObject[] PrefabPool { get; private set; }
        [SerializeField]
        private AsteroidData[] Asteroids;

        public AsteroidData Get(AsteroidType type)
        {
            //assuming all types are included on the Array in the right position
            return Asteroids[((int)type)];
        }

    }

    [System.Serializable] 
    public struct AsteroidData
    {
        public AsteroidType type;
        public float maxSpeed;
        public int health;
        public ShapeData[] shapes;
    }

    [System.Serializable]
    public struct ShapeData
    {
        public Vector2[] points;
    }

    public enum AsteroidType
    {
        Bigger = 0,
        Medium = 1,
        Small = 2,
        Tiny = 3,
    }

    

}