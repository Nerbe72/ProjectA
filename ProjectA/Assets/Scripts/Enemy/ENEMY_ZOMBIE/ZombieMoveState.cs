using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMoveState : IState
{
    Zombie enemy;
    PlayerController player;

    float destinationDistance = 1.4f;
    float distanceLimit = 10f;

    public ZombieMoveState(Zombie _enemy)
    {
        enemy = _enemy;
    }

    public void Enter()
    {
        player = PlayerController.Instance;
    }

    public void Update()
    {

        if (enemy.Chase(destinationDistance))
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.attackState);
            return;
        }

        if (enemy.CheckSpawnDistanceOut(distanceLimit))
        {
            enemy.ReturnSpawnPoint();
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.idleState);
            return;
        }
    }
}
