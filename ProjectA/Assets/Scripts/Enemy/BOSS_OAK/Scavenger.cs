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

        //����, �̵�//




        Selector randomSelector = new Selector(randomNodes);

        Sequence actionSequence = new Sequence(actionNodes);

        //�ǰ�üũ//

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

        //�ƾ�//
        ActionNode stayAction = new ActionNode(DoStay);

        ConditionNode cutsceneCondition = new ConditionNode(IsCutScene, stayAction);

        //���//
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
        //ȸ�� �ൿ
        return NodeStates.SUCCESS;
    }

    private NodeStates DoContinueAttacking()
    {
        //���� ����(ȸ�� ����)
        return NodeStates.SUCCESS;
    }

    private bool IsAttacking()
    {
        //���������� üũ
        return isAttack;
    }

    private NodeStates IsHurted()
    {
        //�ǰݵǾ����� Ȯ��
        return NodeStates.SUCCESS;
    }

    private NodeStates DoStay()
    {
        //���(�ƹ� �ൿ�� ���� ����)
        return NodeStates.SUCCESS;
    }

    private bool IsCutScene()
    {
        //�ƾ��� ��������� Ȯ��
        return isPlayingCutscene;
    }

    private NodeStates DoDead()
    {
        //�������
        return NodeStates.SUCCESS;
    }

    private bool IsDead()
    {
        //����Ͽ�����(ü�� 0) Ȯ��
        return isDead;
    }
}
