using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieMoveState : IState
{
    Zombie enemy;
    PlayerController player;

    

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
        //사망
        if (enemy.isDead)
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.deadState);
            return;
        }

        //피격
        if (enemy.isHit)
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.hurtState);
            return;
        }

        //공격
        if (enemy.Chase(enemy.destinationDistance))
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.attackState);
            return;
        }

        //이동중 특정 거리를 벗어남
        if (enemy.CheckSpawnDistanceOut(enemy.distanceLimit))
        {
            enemy.ReturnSpawnPoint();
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.idleState);
            return;
        }
    }
}
