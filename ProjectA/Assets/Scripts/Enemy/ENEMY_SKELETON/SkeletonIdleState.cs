using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState : IState
{
    Skeleton enemy;

    // Update is called once per frame
    void Update()
    {
        //�÷��̾ ������ ��� ���󰡱�
        if (enemy.IsFaced)
        {
            enemy.SkeletonStateMachine.TransitionTo(enemy.SkeletonStateMachine.moveState);
        }
    }
}
