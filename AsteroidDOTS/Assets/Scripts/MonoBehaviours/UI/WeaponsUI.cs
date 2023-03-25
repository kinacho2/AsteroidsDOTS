using Asteroids.Data;
using Asteroids.ECS.Events;
using Asteroids.ECS.Systems;
using Asteroids.Setup;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids.UI
{
    public class WeaponsUI : MonoBehaviour
    {
        [SerializeField] private Image[] Images;
        private WeaponDataSO WeaponsDB;

        private EventConsumer _consumer;

        private int _weaponIndex;

        // Start is called before the first frame update
        void Start()
        {
            WeaponsDB = Configs.PlayerData.WeaponsDB;
            _weaponIndex = 0;
            EnableImages();

            _consumer = Events_System.OnPickPower.Subscribe(Configs.EVENTS_QUEUE_COUNT);
        }


        private void EnableImages()
        {
            if (_weaponIndex >= WeaponsDB.Count) return;
            var misileAmount = WeaponsDB.Get(_weaponIndex).misileAmount;
            for (int i = 0; i < Images.Length; i++)
            {
                Images[i].enabled = (i < misileAmount);
            }
            _weaponIndex++;
        }
        // Update is called once per frame
        void Update()
        {
            if (Events_System.OnPickPower.TryGetEvent(_consumer, out var power))
            {
                if (power.type == PowerType.Weapon)
                    EnableImages();
            }
        }
    }
}