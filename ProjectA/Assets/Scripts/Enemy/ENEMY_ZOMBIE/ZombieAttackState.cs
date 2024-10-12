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
        if (enemy.IsAnimationAttack())
        {
            enemy.ResetAttackTrigger();
            return;
        }

        enemy.zombieStateMachine.TransitionTo(enemy.zombieStateMachine.moveState);
    }
}
