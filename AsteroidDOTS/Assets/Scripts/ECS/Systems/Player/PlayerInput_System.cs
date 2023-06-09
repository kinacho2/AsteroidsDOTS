using Asteroids.ECS.Components;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace Asteroids.ECS.Systems
{
    public class PlayerInput_System : SystemBase
    {
        protected override void OnUpdate()
        {
            var gameState = GetEntityQuery(typeof(GameStateComponent)).GetSingleton<GameStateComponent>();
            if (gameState.state == GameState.Finished)
            {
                return;
            }

            float tr = Input.GetAxis("Vertical");
            float rot = Input.GetAxis("Horizontal");
            bool shoot = Input.GetKey(KeyCode.Space);
            bool charging = Input.GetKey(KeyCode.E);

            EntityQuery query = GetEntityQuery(typeof(PlayerComponent));
            var entity = query.GetSingletonEntity();

            ShipInputComponent input = EntityManager.GetComponentData<ShipInputComponent>(entity);
            HyperspaceTravelComponent travel = EntityManager.GetComponentData<HyperspaceTravelComponent>(entity);

            ref var dir = ref input.direction;

            if (dir.y != tr)
            {
                if (math.abs(tr) > math.abs(dir.y))
                    Events_System.OnPlayerStartMove.PostEvent(new Events.PlayerMove());
                else if (math.abs(tr) < math.abs(dir.y))
                    Events_System.OnPlayerStopMove.PostEvent(new Events.PlayerMove());
            }

            dir.y = tr;
            dir.x = -rot;
            input.shoot = shoot;

            travel.chargingPressed = charging;

            EntityManager.SetComponentData<ShipInputComponent>(entity, input);
            EntityManager.SetComponentData<HyperspaceTravelComponent>(entity, travel);

        }
    }
}