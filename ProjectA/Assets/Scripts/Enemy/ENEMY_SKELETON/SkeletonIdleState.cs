using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonIdleState : IState
{
    Skeleton enemy;

    // Update is called once per frame
    void Update()
    {
        //플레이어를 조우한 경우 따라가기
        if (enemy.isFaced)
        {
            enemy.SkeletonStateMachine.TransitionTo(enemy.SkeletonStateMachine.moveState);
        }
    }
}
