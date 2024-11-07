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
        List<Node> depth3 = new List<Node>();
        List<Node> depth2 = new List<Node>();
        List<Node> depth1 = new List<Node>();
        rootNode = new Sequence(depth1);



        //depth2

        //depth1
        ActionNode actionDead = new ActionNode(DoIsDead);
        
        depth1.Add(actionDead);

    }

    private NodeStates DoIsDead()
    {
        //사망 상태라면 success

        return NodeStates.FAILURE;
    }
}
