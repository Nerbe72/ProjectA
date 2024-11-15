using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scavenger : Enemy
{
    private Sequence rootNode;

    private bool firstMet; //for cutscene

    private bool isPlayingCutscene;
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
        List<Node> mainList = new List<Node>();
        List<Node> actionList = new List<Node>();
        List<Node> main = new List<Node>();
        List<Node> randomNodes = new List<Node>();
        List<Node> actionNodes = new List<Node>();
        List<Node> hurtNodes = new List<Node>();
        List<Node> retreatNodes = new List<Node>();

        //공격, 이동//




        Selector randomSelector = new Selector(randomNodes);

        Sequence actionSequence = new Sequence(actionNodes);

        //피격체크//

        ActionNode retreatAction = new ActionNode(DoRetreat);

        ActionNode actionContinueAttack = new ActionNode(DoContinueAttacking);
        ConditionNode attackingCondition = new ConditionNode(IsAttacking, actionContinueAttack);

        retreatNodes.Add(attackingCondition);
        retreatNodes.Add(retreatAction);

        ActionNode actionHurted = new ActionNode(IsHurted);
        Selector retreatSelector = new Selector(retreatNodes);

        hurtNodes.Add(actionHurted);
        hurtNodes.Add(retreatSelector);
        
        Selector hurtSelector = new Selector(hurtNodes);

        //컷씬//
        ActionNode stayAction = new ActionNode(DoStay);

        ConditionNode cutsceneCondition = new ConditionNode(IsCutScene, stayAction);

        //사망//
        ActionNode deadAction = new ActionNode(DoDead);

        ConditionNode deadCondition = new ConditionNode(IsDead, deadAction, null);

        //depth2//
        mainList.Add(deadCondition);
        mainList.Add(cutsceneCondition);
        mainList.Add(hurtSelector);
        mainList.Add(actionSequence);

        Selector mainSelector = new Selector(mainList);

        //depth1
        main.Add(mainSelector);

        //depth0
        rootNode = new Sequence(main);

    }

    private NodeStates DoRetreat()
    {
        //회피 행동
        return NodeStates.SUCCESS;
    }

    private NodeStates DoContinueAttacking()
    {
        //공격 지속(회피 무시)
        return NodeStates.SUCCESS;
    }

    private bool IsAttacking()
    {
        //공격중인지 체크
        return isAttack;
    }

    private NodeStates IsHurted()
    {
        //피격되었는지 확인
        return NodeStates.SUCCESS;
    }

    private NodeStates DoStay()
    {
        //대기(아무 행동도 하지 않음)
        return NodeStates.SUCCESS;
    }

    private bool IsCutScene()
    {
        //컷씬이 재생중인지 확인
        return isPlayingCutscene;
    }

    private NodeStates DoDead()
    {
        //사망동작
        return NodeStates.SUCCESS;
    }

    private bool IsDead()
    {
        //사망하였는지(체력 0) 확인
        return isDead;
    }
}
