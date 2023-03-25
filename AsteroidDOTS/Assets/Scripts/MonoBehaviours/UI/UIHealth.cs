using Asteroids.ECS.Systems;
using UnityEngine;

namespace Asteroids.UI
{
    public class UIHealth : MonoBehaviour
    {
        [SerializeField] protected UnityEngine.UI.Image[] Images;

        protected virtual void Start()
        {
            ShipStats_System.OnHealthUpdate += PlayerStats_System_OnHealthUpdate;
        }

        protected void PlayerStats_System_OnHealthUpdate(int value, int maxValue)
        {
            for (int i = 0; i < Images.Length; i++)
            {
                Images[i].gameObject.SetActive(i < value);
            }
        }
    }
}