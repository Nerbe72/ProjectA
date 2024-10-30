using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieDeadState : IState
{
    private Zombie enemy;

    public ZombieDeadState(Zombie _enemy)
    {
        enemy = _enemy;
    }

    public void Enter()
    {
        enemy.PlayAnimationDead();
    }
}
