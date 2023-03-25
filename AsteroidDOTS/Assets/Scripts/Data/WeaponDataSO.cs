using UnityEngine;

namespace Asteroids.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Weapons Data", fileName = "WeaponsData.asset", order = 0)]
    public class WeaponDataSO : ScriptableObject
    {
        [field: SerializeField] 
        public GameObject BombPrefab { get; protected set; }

        [field: SerializeField] 
        public GameObject MisilePrefab { get; protected set; }

        [field: SerializeField] 
        public WeaponData[] Weapons { get; protected set; }

        [field: SerializeField] 
        public float[] MisileAngleDeg { get; protected set; }

        [field: SerializeField] public Vector2[] MisileShape { get; protected set; }
        public WeaponData Get(int range)
        {
            return Weapons[range];
        }

        public float GetAngle(int idx)
        {
            return MisileAngleDeg[idx];
        }

        public int Count => Weapons.Length;
    }

    [System.Serializable]
    public struct WeaponData
    {
        public int misileAmount;
        public float misileSpeed;
        public float misileLifeTime;
        public float range;
    }
}