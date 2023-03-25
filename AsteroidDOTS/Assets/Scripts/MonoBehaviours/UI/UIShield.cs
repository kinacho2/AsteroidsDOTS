using Asteroids.ECS.Systems;
namespace Asteroids.UI
{
    public class UIShield : UIHealth
    {
        protected override void Start()
        {
            ShipStats_System.OnShieldUpdate += PlayerStats_System_OnHealthUpdate;
        }
    }
}