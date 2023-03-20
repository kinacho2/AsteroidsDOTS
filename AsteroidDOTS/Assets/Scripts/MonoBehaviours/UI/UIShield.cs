using Asteroids.ECS.Systems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Asteroids.UI
{
    public class UIShield : UIHealth
    {
        // Start is called before the first frame update
        protected override void Start()
        {
            PlayerStats_System.OnShieldUpdate += PlayerStats_System_OnHealthUpdate;
        }


    }
}