using Asteroids.Data;
using Asteroids.ECS.Events;
using Asteroids.ECS.Systems;
using Asteroids.Setup;
using UnityEngine;
using UnityEngine.UI;

namespace Asteroids.UI
{
    public class UIPowers : MonoBehaviour
    {
        [SerializeField] private Image HyperspaceTravelImage;
        [SerializeField] private Image WeaponImage;

        private EventConsumer _consumerTravel;

        private HyperspaceTravelData TravelData;

        float _timer;
        float _time;

        private void Awake()
        {
            Configs.OnInitializedConfig += Configs_OnInitializedConfig;
            
        }

        private void Configs_OnInitializedConfig()
        {
            Configs.OnInitializedConfig -= Configs_OnInitializedConfig;
             
            _consumerTravel = Events_System.OnHyperspaceTravel.Subscribe();
            TravelData = Configs.GameData.HyperspaceTravelData;

            _time = _timer = TravelData.timeReloading;
        }

        private void Update()
        {
            if (Events_System.OnHyperspaceTravel.TryGetEvent(_consumerTravel, out var travel))
            {
                _timer = 0;
            }

            HyperspaceTravelImage.fillAmount = _timer / _time;
            _timer = Mathf.Min(_timer + Time.deltaTime, _time);
        }
    }
}