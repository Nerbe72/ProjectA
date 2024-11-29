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
        //회전은 코드로 직접 수정
        agent.updateRotation = false;

        InitBT();
    }

    private void Update()
    {
        //패턴 넘기기(강제 피격)
        if (Input.GetKeyDown(KeyCode.G))
        {
            Hurt((100, 0));
        }

        //패턴 넘기기(사망)
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

        //공격, 이동//
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

        //피격체크//
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

        //컷씬//
        ActionNode stayAction = new ActionNode(DoStay);
        ConditionNode cutsceneCondition = new ConditionNode(IsCutScene, stayAction);

        //사망//
        ActionNode deadAction = new ActionNode(DoDead);
        ConditionNode deadCondition = new ConditionNode(IsDead, deadAction, null);

        //조우//
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
        //초근접 공격
        animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.A);
        animator.SetTrigger(ScavengerHash.Attack);
        return NodeStates.SUCCESS;
    }

    private NodeStates AttackB()
    {
        //약근접 공격
        animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.B);
        animator.SetTrigger(ScavengerHash.Attack);
        return NodeStates.SUCCESS;
    }

    private bool IsCloseToPlayer()
    {
        //플레이어와의 근접 확인
        float dist = Vector3.Distance(player.transform.position, transform.position);

        if (dist <= attackShortDistnace) return true;

        return false;
    }

    private NodeStates DoSidewalk()
    {
        //해당 위치에서 플레이어를 바라보며 좌 우로 이동
        
        //플레이어와 본인간 직선을 긋고 수직 방향으로 이동.
        //플레이어 정면으로부터 플레이어의 위치가 왼/오 인지 확인 후 해당 방향으로 이동
        //플레이어 바라보기

        return NodeStates.SUCCESS;
    }

    private NodeStates DoChase()
    {
        //특정 거리까지 추격
        agent.stoppingDistance = Mathf.Clamp(attackShortDistnace - 1f, 0.5f, 10f);
        agent.Move(player.transform.position);

        return NodeStates.SUCCESS;
    }

    private NodeStates DoRetreat()
    {
        //회피 행동
        //플레이어를 바라본 상태로
        Vector3 targetDir = player.transform.position;
        Vector3 backDir = -targetDir;

        agent.SetDestination(transform.position + backDir);

        transform.rotation = Quaternion.LookRotation(targetDir);

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

        if (isHurting) return NodeStates.SUCCESS;

        return NodeStates.FAILURE;
    }

    private NodeStates DoStay()
    {
        //대기(아무 행동도 하지 않음) 해당 위치에서 BT 종료
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

    private bool IsFaced()
    {
        //조우 확인
        return isFaced;
    }

    private NodeStates NotFaced()
    {
        //조우하지 않은 경우 BT 종료
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
