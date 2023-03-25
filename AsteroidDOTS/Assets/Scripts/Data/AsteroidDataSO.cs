using UnityEngine;

namespace Asteroids.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Asteroid Data", fileName = "AsteroidData.asset", order = 0)]
    public class AsteroidDataSO : ScriptableObject
    {
        [field: SerializeField]
        public int InitialCount { get; protected set; }

        [field: SerializeField]
        public GameObject Prefab { get; private set; }

        [SerializeField]
        private AsteroidData[] Asteroids;

        public int DataCount => Asteroids.Length;

        public AsteroidData Get(AsteroidType type)
        {
            //assuming all types are included on the Array in the right position
            return Asteroids[((int)type)];
        }

        public int ShapesCount()
        {
            int n = 0;
            foreach (var data in Asteroids)
            {
                n += data.shapes.Length;
            }
            return n;
        }
    }

    [System.Serializable]
    public struct AsteroidData
    {
        public AsteroidType type;
        public float maxSpeed;
        public int health;
        public float mass;
        public float size;
        public ShapeData[] shapes;
    }

    [System.Serializable]
    public struct ShapeData
    {
        public Vector2[] points;
    }

    public enum AsteroidType : int
    {
        Bigger = 0,
        Medium = 1,
        Small = 2,
        Tiny = 3,
    }
}