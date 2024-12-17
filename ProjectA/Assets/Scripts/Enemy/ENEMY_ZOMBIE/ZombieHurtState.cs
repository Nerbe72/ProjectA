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

        //사망
        if (enemy.isDead)
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.deadState);
            return;
        }

        //공격
        if (enemy.Chase(enemy.destinationDistance))
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.attackState);
            return;
        }

        //이동
        enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.moveState);
    }
}
