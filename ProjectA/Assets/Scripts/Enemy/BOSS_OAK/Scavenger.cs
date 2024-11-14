using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : Enemy
{
    private Sequence rootNode;

    private bool firstMet; //for cutscene

    private bool isChase;
    private bool isIdle;
    private bool isAttack;

    private Vector3 targetPosition;

    private void Start()
    {
        InitBT();
    }

    private void InitBT()
    {





        ActionNode actionDead = new ActionNode(DoDead);
        


    }

    private NodeStates DoDead()
    {
        //사망 상태라면 success

        return NodeStates.FAILURE;
    }
}
