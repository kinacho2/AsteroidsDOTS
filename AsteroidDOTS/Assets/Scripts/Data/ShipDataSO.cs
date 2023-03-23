using UnityEngine;

namespace Asteroids.Data
{
    [CreateAssetMenu(menuName = "Asteroids/Ship Data", fileName = "ShipData.asset", order = 0)]

    public class ShipDataSO : ScriptableObject
    {
        [field: SerializeField]
        public GameObject ShipPrefab { get; protected set; }

        [field: SerializeField]
        public WeaponDataSO WeaponsDB { get; protected set; }

        [field: SerializeField]
        public ShipData[] Ships { get; protected set; }
    }


    [System.Serializable]
    public struct ShipData
    {
        public float maxSpeed;
        public float acceleration;
        public float rotationSpeedDeg;
        public float restitution;
        public float stunnedTime;
        public float invTime;
        public float shootCooldown;
        public int health;
        public int shieldHealth;
        public Vector2[] shape;
    }
}
