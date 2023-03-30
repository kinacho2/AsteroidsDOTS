using UnityEngine;

namespace Asteroids.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Weapons Data", fileName = "WeaponsData.asset", order = 0)]
    public class WeaponDataSO : ScriptableObject
    {
        [field: SerializeField] 
        public GameObject BombPrefab { get; protected set; }

        [field: SerializeField] 
        public GameObject MissilePrefab { get; protected set; }

        [field: SerializeField] 
        public WeaponData[] Weapons { get; protected set; }

        [field: SerializeField] 
        public float[] MissileAngleDeg { get; protected set; }

        [field: SerializeField] public Vector2[] MissileShape { get; protected set; }
        public WeaponData Get(int range)
        {
            return Weapons[range];
        }

        public float GetAngle(int idx)
        {
            return MissileAngleDeg[idx];
        }

        public int Count => Weapons.Length;
    }

    [System.Serializable]
    public struct WeaponData
    {
        public int missileAmount;
        public float missileSpeed;
        public float missileLifeTime;
        public float range;
    }
}