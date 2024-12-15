using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAttackState : IState
{
    Zombie enemy;

    public ZombieAttackState(Zombie _enemy)
    {
        enemy = _enemy;
    }

    public void Enter()
    {
        enemy.TriggerAnimationAttack();
    }

    public void Update()
    {
        //ÇÇ°Ý
        if (enemy.isHurt)
        {
            enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.hurtState);
            return;
        }

        if (enemy.IsAnimationAttack())
        {
            enemy.ResetAttack();
            return;
        }

        enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.moveState);
    }
}
