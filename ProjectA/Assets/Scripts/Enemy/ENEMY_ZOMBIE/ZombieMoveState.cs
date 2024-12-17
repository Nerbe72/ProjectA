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
        //���
        if (enemy.isDead)
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.deadState);
            return;
        }

        //�ǰ�
        if (enemy.isHit)
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.hurtState);
            return;
        }

        //����
        if (enemy.Chase(enemy.destinationDistance))
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.attackState);
            return;
        }

        //�̵��� Ư�� �Ÿ��� ���
        if (enemy.CheckSpawnDistanceOut(enemy.distanceLimit))
        {
            enemy.ReturnSpawnPoint();
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.idleState);
            return;
        }
    }
}
