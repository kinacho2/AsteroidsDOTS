using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;


namespace Asteroids.ECS.Components
{
    public struct HyperspaceTravelComponent : IComponentData
    {
        public HyperspaceTravelState state;
        public float timeBeforeTravel;
        public float timeAfterTravel;
        public float timeReloading;

        public float chargeTimer;

        public bool chargingPressed;

    }

    public enum HyperspaceTravelState
    {
        Enabled,
        Charging,
        Traveling,
        Reloading,
    }
}