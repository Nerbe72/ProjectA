using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum ScavengerAttackType
{
    A,
    B,
}

public enum ScavengerPattern
{
    Chase,
    SideWalk,
    Retreat,
    Backward,
    Attack,
}

public class Scavenger : Enemy
{
    private Sequence rootNode;
    private ScavengerPattern prePattern = ScavengerPattern.Chase;

    private bool firstMet; //for cutscene
    private bool isChase;
    private bool isIdle;
    private bool isAttack;
    private bool isRetreating;

    private Vector3 targetPosition;

    private float moveTime = 0f;

    [SerializeField] private List<CapsuleCollider> attackCollider;

    [Header("공격 패턴 거리")]
    [SerializeField] private float attackALess;
    [SerializeField] private float attackBOver;
    [Header("패턴 시간")]
    [SerializeField][Tooltip("회피동작 소요시간")] private float retreatingInterval;
    [SerializeField][Tooltip("후퇴동작 소요시간")] private float backwardInterval;
    [SerializeField][Tooltip("공격패턴 변경 대기시간")] private float attackWaitInterval;
    [SerializeField][Tooltip("랜덤패턴 변경 대기시간(추격)")] private float randomWait1Interval;
    [SerializeField][Tooltip("랜덤패턴 변경 대기시간(후퇴)")] private float randomWait2Interval;
    [SerializeField][Tooltip("사이드 스텝 방향 변경 대기시간")] private float sideWalkInterval;
    [Header("패턴 속도")]
    [SerializeField][Tooltip("회피 속도")] private float retreatSpeed;
    [SerializeField][Tooltip("후퇴 속도")] private float backwardSpeed;
    [SerializeField][Tooltip("원거리 공격 추격 속도")] private float attackBSpeed;
    [SerializeField][Tooltip("추격 속도")] private float chaseSpeed;
    [SerializeField][Tooltip("좌우 이동대기 속도")] private float sidewalkSpeed;
    [Header("공격 쿨타임")]
    [SerializeField][Tooltip("공격 A 쿨타임")] private float attackACooldown;
    [SerializeField][Tooltip("공격 B 쿨타임")] private float attackBCooldown;
    [Header("Debug")]
    [SerializeField] private TMP_Text text;

    private bool isMovingRight = true;
    private float sideWalkTimer = 0f;

    private float lastAttackATime = 0f;
    private float lastAttackBTime = 0f;
    private bool CanAttackA => (Time.time - lastAttackATime) >= attackACooldown;
    private bool CanAttackB => (Time.time - lastAttackBTime) >= attackBCooldown;

    public static class ScavengerHash
    {
        public static readonly int Walk = Animator.StringToHash("Walk");
        public static readonly int Side = Animator.StringToHash("Side");
        public static readonly int AttackType = Animator.StringToHash("AttackType");
        public static readonly int Attack = Animator.StringToHash("Attack");
        public static readonly int Hurt = Animator.StringToHash("Hurt");
        public static readonly int DeadB = Animator.StringToHash("Dead");
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

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        animator.SetFloat(ScavengerHash.Side, Mathf.Lerp(animator.GetFloat(ScavengerHash.Side), localVelocity.x, Time.deltaTime * 3f));
        animator.SetFloat(ScavengerHash.Walk, Mathf.Lerp(animator.GetFloat(ScavengerHash.Walk), localVelocity.z, Time.deltaTime * 3f));
    }

    protected override void InitForChild()
    {
        agent.acceleration = float.MaxValue;
        InitBT();
    }

    private void InitBT()
    {
        List<Node> mainNodes = new List<Node>();

        #region 사망
        ActionNode deadAction = new ActionNode(DoDead);
        ConditionNode isDeadCondition = new ConditionNode(IsDead, deadAction, null);
        #endregion 사망

        #region 컷씬
        ActionNode stayAction = new ActionNode(DoStay);
        ConditionNode isCutsceneCondition = new ConditionNode(IsCutScene, stayAction, null);
        #endregion 컷씬

        mainNodes.Add(isDeadCondition);
        mainNodes.Add(isCutsceneCondition);
        mainNodes.Add(AttackAndRetreat());
        mainNodes.Add(Move());

        rootNode = new Sequence(mainNodes);
    }

    private Node AttackAndRetreat()
    {
        // 공격/피격
        List<Node> attackNodes = new List<Node>();

        #region 피격
        List<Node> retreatNodes = new List<Node>();
        ActionNode isHurtedAction = new ActionNode(IsHurted);
        Waitor retreatWaitor = new Waitor(retreatingInterval);
        ActionNode retreatAction = new ActionNode(DoRetreat);

        retreatNodes.Add(isHurtedAction);
        retreatNodes.Add(retreatWaitor);
        retreatNodes.Add(retreatAction);

        Sequence retreatSequence = new Sequence(retreatNodes);
        #endregion 피격

        ActionNode resetHurtRetreatAction = new ActionNode(ResetHurtRetreat);
        Waitor attackWaitor = new Waitor(attackWaitInterval);

        #region 공격

        ActionNode attackAAction = new ActionNode(AttackA);
        ConditionNode isLessCondition = new ConditionNode(() => { return DistanceToPlayer() <= attackALess + 0.1f && CanAttackA; }, attackAAction, null);
        ActionNode attackBAction = new ActionNode(AttackB);
        ConditionNode isOverCondition = new ConditionNode(() => { return DistanceToPlayer() > attackBOver - attackALess && CanAttackB; }, attackBAction, isLessCondition);

        //List<Node> lessDistNodes = new List<Node>();
        //List<Node> overDistNodes = new List<Node>();

        //ActionNode attackAAction = new ActionNode(AttackA);
        //ConditionNode isLessCondition = new ConditionNode(() => {return DistanceToPlayer() <= attackALess + 0.1f; }, attackAAction, null); //debug

        //ActionNode attackBAction = new ActionNode(AttackB);

        //ConditionNode isOverCondition = new ConditionNode(() => { return DistanceToPlayer() > attackBOver - attackALess; }, attackBAction, isLessCondition);
        #endregion 공격

        attackNodes.Add(retreatSequence);
        attackNodes.Add(resetHurtRetreatAction);
        attackNodes.Add(attackWaitor);
        attackNodes.Add(isOverCondition);

        Selector attackSelector = new Selector(attackNodes);
        ConditionNode isAttackingCondition = new ConditionNode(IsAttacking, null, attackSelector);

        return isAttackingCondition;
    }

    private Node Move()
    {
        //이동
        List<Node> random1Nodes = new List<Node>();
        List<Node> random2Nodes = new List<Node>();
        ActionNode randomChaseAction = new ActionNode(DoChase);
        ActionNode randomSideWalkNCAction = new ActionNode(DoSideWalk);

        ActionNode randomSideWalkNBAction = new ActionNode(DoSideWalk);
        ActionNode randomBackwardAction = new ActionNode(DoBackward);

        random1Nodes.Add(randomChaseAction);
        random1Nodes.Add(randomSideWalkNCAction);

        random2Nodes.Add(randomSideWalkNBAction);
        random2Nodes.Add(randomBackwardAction);

        RandomSelector random1Selector = new RandomSelector(random1Nodes, randomWait1Interval);
        RandomSelector random2Selector = new RandomSelector(random2Nodes, randomWait2Interval);

        ConditionNode isDistOverCondition = new ConditionNode(() => { return DistanceToPlayer() >= (attackBOver - attackALess); }, random1Selector, null);

        ConditionNode randomLessCondition = new ConditionNode(IsDistLessForRandom, random2Selector, isDistOverCondition);
        ConditionNode isAttackAndRetreatCondition = new ConditionNode(IsAttackRetreat, null, randomLessCondition);

        return isAttackAndRetreatCondition;
    }

    private bool IsDistLessForRandom()
    {
        return DistanceToPlayer() <= Random.Range(attackALess - 0.05f, attackBOver + 0.06f);
    }

    private NodeStates ResetHurtRetreat()
    {
        isHurt = false;
        if (isRetreating) return NodeStates.SUCCESS;
        return NodeStates.FAILURE;
    }

    private bool IsAttackRetreat()
    {
        if (isAttack || isRetreating) return true;
        return false;
    }

    private NodeStates AttackA()
    {
        //초근접 공격
        agent.velocity = Vector3.zero;
        agent.SetDestination(transform.position);
        animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.A);
        animator.SetTrigger(ScavengerHash.Attack);
        isAttack = true;
        text.text = "<color=red>Attack A</color>";

        lastAttackATime = Time.time;

        return NodeStates.SUCCESS;
    }

    private NodeStates AttackB()
    {
        //약근접 공격
        animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.B);
        animator.SetTrigger(ScavengerHash.Attack);
        isAttack = true;
        agent.updateRotation = true;
        agent.speed = attackBSpeed;

        Vector3 linear = (player.transform.position - transform.position).normalized;
        Vector3 targetPosition = player.transform.position - (linear * (attackALess - 0.1f));
        agent.SetDestination(targetPosition);
        text.text = "<color=red>Attack B</color>";

        lastAttackBTime = Time.time;

        return NodeStates.SUCCESS;
    }

    private NodeStates DoSideWalk()
    {
        //해당 위치에서 플레이어를 바라보며 좌 우로 이동
        //플레이어와 본인간 직선을 긋고 수직 방향으로 이동
        agent.updateRotation = false;
        agent.speed = sidewalkSpeed;

        Vector3 direction = (player.transform.position - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(direction);

        //양쪽 방향 벡터
        Vector3 left = Vector3.Cross(transform.up, direction).normalized;
        Vector3 right = Vector3.Cross(direction, transform.up).normalized;
        Vector3 finalDirection;

        //좌우측 선택
        sideWalkTimer += Time.deltaTime;
        if (sideWalkTimer >= sideWalkInterval)
        {
            isMovingRight = !isMovingRight;
            sideWalkTimer = 0f;
        }

        if (isMovingRight)
            finalDirection = right;
        else
            finalDirection = left;

        agent.SetDestination(transform.position + finalDirection);
        text.text = "<color=blue>Sidewalk</color>";
        return NodeStates.FAILURE;
    }

    private NodeStates DoChase()
    {
        if (prePattern == ScavengerPattern.Backward) return NodeStates.FAILURE;
        //특정 거리까지 추격
        isChase = true;
        agent.updateRotation = true;
        agent.speed = chaseSpeed;

        Vector3 linear = (player.transform.position - transform.position).normalized;
        Vector3 targetPosition = player.transform.position - (linear * (attackALess - 0.1f));
        agent.SetDestination(targetPosition);
        text.text = "<color=green>Chase</color>";
        return NodeStates.FAILURE;
    }

    private NodeStates DoRetreat()
    {
        if (prePattern == ScavengerPattern.Chase) return NodeStates.FAILURE;
        //회피 행동
        //플레이어를 바라본 상태로
        agent.updateRotation = false;

        Vector3 targetDir = (player.transform.position - transform.position).normalized;

        agent.speed = retreatSpeed;
        agent.SetDestination(transform.position - targetDir);
        transform.rotation = Quaternion.LookRotation(targetDir);
        
        text.text = "<color=yellow>Retreat</color>";
        return NodeStates.FAILURE;
    }

    private NodeStates DoBackward()
    {
        agent.updateRotation = false;

        Vector3 targetDir = (player.transform.position - transform.position).normalized;

        agent.speed = 1.5f;
        agent.SetDestination(transform.position - targetDir);
        transform.rotation = Quaternion.LookRotation(targetDir);

        text.text = "<color=yellow>Backward</color>";
        return NodeStates.FAILURE;
    }

    private bool IsAttacking()
    {
        //공격중인지 체크
        return isAttack;
    }

    private NodeStates IsHurted()
    {
        //피격되었는지 확인
        if (isHurt)
        {
            animator.SetTrigger(ScavengerHash.Hurt);
            return NodeStates.SUCCESS;
        }
        return NodeStates.FAILURE;
    }

    private NodeStates DoStay()
    {
        //대기(아무 행동도 하지 않음) 해당 위치에서 BT 종료
        text.text = "CutScene Playing";
        return NodeStates.FAILURE;
    }

    private bool IsCutScene()
    {
        //컷씬이 재생중인지 확인
        return isCutscene;
    }

    private NodeStates DoDead()
    {
        //사망
        isDead = false;
        PlayAnimationDead();
        return NodeStates.FAILURE;
    }

    private bool IsDead()
    {
        //사망하였는지(체력 0) 확인
        return isDead;
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    #region 애니메이션
    public void SetAttacking()
    {
        isAttack = true;
    }

    public void ResetAttacking()
    {
        isAttack = false;
    }

    public void SetRetreating()
    {
        isRetreating = true;
    }

    public void ResetRetreating()
    {
        isRetreating = false;
    }

    public void Stop()
    {
        agent.velocity = Vector3.zero;
    }

    public void SetAttackTrigger()
    {

    }

    public void ResetAttackTrigger()
    {

    }
    #endregion 애니메이션

}
