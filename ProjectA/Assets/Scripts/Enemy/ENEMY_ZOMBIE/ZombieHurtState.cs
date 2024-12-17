using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieHurtState : IState
{
    private Zombie enemy;

    public ZombieHurtState(Zombie _enemy)
    {
        enemy = _enemy;
    }

    public void Enter()
    {
        enemy.PlayAnimationHurt();
    }

    public void Update()
    {
        if (enemy.isHit) return;

        //���
        if (enemy.isDead)
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.deadState);
            return;
        }

        //����
        if (enemy.Chase(enemy.destinationDistance))
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.attackState);
            return;
        }

        //�̵�
        enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.moveState);
    }
}
