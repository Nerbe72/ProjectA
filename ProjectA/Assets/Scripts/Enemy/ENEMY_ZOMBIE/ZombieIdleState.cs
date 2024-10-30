using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieIdleState : IState
{
    Zombie enemy;
    bool firstMet;

    public ZombieIdleState(Zombie _enemy)
    {
        enemy = _enemy;
    }

    public void Update()
    {
        //피격
        if (enemy.isHurt)
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.hurtState);
            return;
        }

        //플레이어를 조우한 경우 따라가기
        if (enemy.IsPlayerInSight())
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.moveState);
            return;
        }

        //원위치 복귀
        if (Vector3.Distance(enemy.SpawnPoint, enemy.transform.position) <= 0.1f)
        {
            if (enemy.transform.rotation != enemy.SpawnRotation)
            {
                enemy.transform.rotation = Quaternion.RotateTowards(enemy.transform.rotation, enemy.SpawnRotation, 30);
                enemy.agent.isStopped = true;
            }
        }
    }
}
