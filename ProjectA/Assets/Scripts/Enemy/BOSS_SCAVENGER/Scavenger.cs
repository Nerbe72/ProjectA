using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ScavengerAttackType
{
    A,
    B,
}

public class Scavenger : Enemy
{
    private Sequence rootNode;

    private bool firstMet; //for cutscene

    [SerializeField] private bool isPlayingCutscene;
    private bool isChase;
    private bool isIdle;
    private bool isAttack;

    private Vector3 targetPosition;

    [SerializeField] private float attackShortDistnace;

    public static class ScavengerHash
    {
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Side = Animator.StringToHash("Side");
        public static readonly int AttackType = Animator.StringToHash("AttackType");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hurt = Animator.StringToHash("Hurt");
        public static readonly int DeadB = Animator.StringToHash("Dead");
    }

    protected override void InitForChild()
    {
        //ȸ���� �ڵ�� ���� ����
        agent.updateRotation = false;

        InitBT();
    }

    private void Update()
    {
        //���� �ѱ��(���� �ǰ�)
        if (Input.GetKeyDown(KeyCode.G))
        {
            Hurt((100, 0));
        }

        //���� �ѱ��(���)
        if (Input.GetKeyDown(KeyCode.J))
        {
            Hurt((99999, 99999));
        }

        rootNode.Evaluate();
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
        //1set
        ActionNode chaseAction = new ActionNode(DoChase);
        ActionNode sidewalkAction = new ActionNode(DoSidewalk);

        randomNodes.Add(chaseAction);
        randomNodes.Add(sidewalkAction);

        Selector randomSelector = new Selector(randomNodes);
        //1set end

        //2set
        Waitor attackWaitor = new Waitor(2f);
        //2set end

        //3set
        ActionNode attackAAction = new ActionNode(AttackA);
        ActionNode attackBAction = new ActionNode(AttackB);
        ConditionNode isClosedCondition = new ConditionNode(IsCloseToPlayer, attackAAction, attackBAction);
        //3set end

        actionNodes.Add(randomSelector);
        actionNodes.Add(attackWaitor);
        actionNodes.Add(isClosedCondition);

        Sequence actionSequence = new Sequence(actionNodes);

        //�ǰ�üũ//
        ActionNode retreatAction = new ActionNode(DoRetreat);
        ActionNode actionContinueAttack = new ActionNode(DoContinueAttacking);
        ConditionNode attackingCondition = new ConditionNode(IsAttacking, actionContinueAttack);

        retreatNodes.Add(attackingCondition);
        retreatNodes.Add(retreatAction);

        ActionNode hurtedAction = new ActionNode(IsHurted);
        Selector retreatSelector = new Selector(retreatNodes);

        hurtNodes.Add(hurtedAction);
        hurtNodes.Add(retreatSelector);
        
        Selector hurtSelector = new Selector(hurtNodes);

        //�ƾ�//
        ActionNode stayAction = new ActionNode(DoStay);
        ConditionNode cutsceneCondition = new ConditionNode(IsCutScene, stayAction);

        //���//
        ActionNode deadAction = new ActionNode(DoDead);
        ConditionNode deadCondition = new ConditionNode(IsDead, deadAction, null);

        //����//
        ActionNode notFacedAction = new ActionNode(NotFaced);
        ConditionNode facedCondition = new ConditionNode(IsFaced, null, notFacedAction);

        //depth2//
        mainList.Add(facedCondition);
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

    private NodeStates AttackA()
    {
        //�ʱ��� ����
        animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.A);
        animator.SetTrigger(ScavengerHash.Attack);
        return NodeStates.SUCCESS;
    }

    private NodeStates AttackB()
    {
        //����� ����
        animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.B);
        animator.SetTrigger(ScavengerHash.Attack);
        return NodeStates.SUCCESS;
    }

    private bool IsCloseToPlayer()
    {
        //�÷��̾���� ���� Ȯ��
        float dist = Vector3.Distance(player.transform.position, transform.position);

        if (dist <= attackShortDistnace) return true;

        return false;
    }

    private NodeStates DoSidewalk()
    {
        //�ش� ��ġ���� �÷��̾ �ٶ󺸸� �� ��� �̵�
        
        //�÷��̾�� ���ΰ� ������ �߰� ���� �������� �̵�.
        //�÷��̾� �������κ��� �÷��̾��� ��ġ�� ��/�� ���� Ȯ�� �� �ش� �������� �̵�
        //�÷��̾� �ٶ󺸱�

        return NodeStates.SUCCESS;
    }

    private NodeStates DoChase()
    {
        //Ư�� �Ÿ����� �߰�
        agent.stoppingDistance = Mathf.Clamp(attackShortDistnace - 1f, 0.5f, 10f);
        agent.Move(player.transform.position);

        return NodeStates.SUCCESS;
    }

    private NodeStates DoRetreat()
    {
        //ȸ�� �ൿ
        //�÷��̾ �ٶ� ���·�
        Vector3 targetDir = player.transform.position;
        Vector3 backDir = -targetDir;

        agent.SetDestination(transform.position + backDir);

        transform.rotation = Quaternion.LookRotation(targetDir);

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

        if (isHurting) return NodeStates.SUCCESS;

        return NodeStates.FAILURE;
    }

    private NodeStates DoStay()
    {
        //���(�ƹ� �ൿ�� ���� ����) �ش� ��ġ���� BT ����
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

    private bool IsFaced()
    {
        //���� Ȯ��
        return isFaced;
    }

    private NodeStates NotFaced()
    {
        //�������� ���� ��� BT ����
        return NodeStates.SUCCESS;
    }

    public void SetAttacking()
    {
        Debug.Log("Attack Start");
        isAttack = true;
    }

    public void ResetAttacking()
    {
        Debug.Log("Attack End");
        isAttack = false;
    }
}
