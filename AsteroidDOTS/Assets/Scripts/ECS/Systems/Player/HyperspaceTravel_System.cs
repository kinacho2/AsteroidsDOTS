using Asteroids.ECS.Components;
using Asteroids.ECS.Events;
using Asteroids.Setup;
using Asteroids.Tools;
using Unity.Entities;
using Unity.Transforms;
using Random = Unity.Mathematics.Random;

namespace Asteroids.ECS.Systems
{
    public class HyperspaceTravel_System : SystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
        }
        protected override void OnUpdate()
        {
            var gameState = GetEntityQuery(typeof(GameStateComponent)).GetSingleton<GameStateComponent>();
            if (gameState.state == GameState.Finished)
            {
                return;
            }

            EntityQuery query = GetEntityQuery(typeof(PlayerComponent));
            var entity = query.GetSingletonEntity();
            var tr = EntityManager.GetComponentData<Translation>(entity);
            var travel = EntityManager.GetComponentData<HyperspaceTravelComponent>(entity);

            switch (travel.state)
            {
                case HyperspaceTravelState.Enabled:
                    if (travel.chargingPressed)
                    {
                        travel.state = HyperspaceTravelState.Charging;
                        Events_System.OnHyperspaceTravelStart.PostEvent(new PlayerMove());
                    }
                    break;

                case HyperspaceTravelState.Charging:
                    travel.chargeTimer += Time.DeltaTime;
                    if (travel.chargeTimer >= travel.timeBeforeTravel)
                    {
                        travel.chargeTimer = 0;
                        travel.state = HyperspaceTravelState.Traveling;
                        var Random = new Random((uint)Time.ElapsedTime);
                        tr.Value = Configs.GetRandomPositionInsideScreen(ref Random).ToFloat3();
                        EntityManager.SetComponentData<Translation>(entity, tr);
                        Events_System.OnHyperspaceTravel.PostEvent(new PlayerMove());
                    }
                    else if (!travel.chargingPressed)
                    {
                        travel.chargeTimer = 0;
                        travel.state = HyperspaceTravelState.Enabled;
                        Events_System.OnHyperspaceTravelStop.PostEvent(new PlayerMove());
                    }
                    break;

                case HyperspaceTravelState.Traveling:
                    travel.chargeTimer += Time.DeltaTime;
                    if (travel.chargeTimer >= travel.timeAfterTravel)
                    {
                        travel.chargeTimer = 0;
                        travel.state = HyperspaceTravelState.Reloading;
                        Events_System.OnHyperspaceTravelStop.PostEvent(new PlayerMove());
                    }
                    break;
                case HyperspaceTravelState.Reloading:
                    travel.chargeTimer += Time.DeltaTime;
                    if (travel.chargeTimer >= travel.timeReloading)
                    {
                        travel.chargeTimer = 0;
                        travel.state = HyperspaceTravelState.Enabled;
                    }
                    break;
            }

            EntityManager.SetComponentData<HyperspaceTravelComponent>(entity, travel);

        }
    }
}