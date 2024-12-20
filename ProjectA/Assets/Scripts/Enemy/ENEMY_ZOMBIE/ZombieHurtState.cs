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
        enemy.agent.SetDestination(enemy.transform.position);
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
        if (!enemy.CheckPlayerDistanceIn(1.5f))
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.moveState);
    }
}
