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

    [Header("���� ���� �Ÿ�")]
    [SerializeField] private float attackALess;
    [SerializeField] private float attackBOver;
    [Header("���� �ð�")]
    [SerializeField][Tooltip("ȸ�ǵ��� �ҿ�ð�")] private float retreatingInterval;
    [SerializeField][Tooltip("������ �ҿ�ð�")] private float backwardInterval;
    [SerializeField][Tooltip("�������� ���� ���ð�")] private float attackWaitInterval;
    [SerializeField][Tooltip("�������� ���� ���ð�(�߰�)")] private float randomWait1Interval;
    [SerializeField][Tooltip("�������� ���� ���ð�(����)")] private float randomWait2Interval;
    [SerializeField][Tooltip("���̵� ���� ���� ���� ���ð�")] private float sideWalkInterval;
    [Header("���� �ӵ�")]
    [SerializeField][Tooltip("ȸ�� �ӵ�")] private float retreatSpeed;
    [SerializeField][Tooltip("���� �ӵ�")] private float backwardSpeed;
    [SerializeField][Tooltip("���Ÿ� ���� �߰� �ӵ�")] private float attackBSpeed;
    [SerializeField][Tooltip("�߰� �ӵ�")] private float chaseSpeed;
    [SerializeField][Tooltip("�¿� �̵���� �ӵ�")] private float sidewalkSpeed;
    [Header("���� ��Ÿ��")]
    [SerializeField][Tooltip("���� A ��Ÿ��")] private float attackACooldown;
    [SerializeField][Tooltip("���� B ��Ÿ��")] private float attackBCooldown;
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

        #region ���
        ActionNode deadAction = new ActionNode(DoDead);
        ConditionNode isDeadCondition = new ConditionNode(IsDead, deadAction, null);
        #endregion ���

        #region �ƾ�
        ActionNode stayAction = new ActionNode(DoStay);
        ConditionNode isCutsceneCondition = new ConditionNode(IsCutScene, stayAction, null);
        #endregion �ƾ�

        mainNodes.Add(isDeadCondition);
        mainNodes.Add(isCutsceneCondition);
        mainNodes.Add(AttackAndRetreat());
        mainNodes.Add(Move());

        rootNode = new Sequence(mainNodes);
    }

    private Node AttackAndRetreat()
    {
        // ����/�ǰ�
        List<Node> attackNodes = new List<Node>();

        #region �ǰ�
        List<Node> retreatNodes = new List<Node>();
        ActionNode isHurtedAction = new ActionNode(IsHurted);
        Waitor retreatWaitor = new Waitor(retreatingInterval);
        ActionNode retreatAction = new ActionNode(DoRetreat);

        retreatNodes.Add(isHurtedAction);
        retreatNodes.Add(retreatWaitor);
        retreatNodes.Add(retreatAction);

        Sequence retreatSequence = new Sequence(retreatNodes);
        #endregion �ǰ�

        ActionNode resetHurtRetreatAction = new ActionNode(ResetHurtRetreat);
        Waitor attackWaitor = new Waitor(attackWaitInterval);

        #region ����

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
        #endregion ����

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
        //�̵�
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
        //�ʱ��� ����
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
        //����� ����
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
        //�ش� ��ġ���� �÷��̾ �ٶ󺸸� �� ��� �̵�
        //�÷��̾�� ���ΰ� ������ �߰� ���� �������� �̵�
        agent.updateRotation = false;
        agent.speed = sidewalkSpeed;

        Vector3 direction = (player.transform.position - transform.position).normalized;

        transform.rotation = Quaternion.LookRotation(direction);

        //���� ���� ����
        Vector3 left = Vector3.Cross(transform.up, direction).normalized;
        Vector3 right = Vector3.Cross(direction, transform.up).normalized;
        Vector3 finalDirection;

        //�¿��� ����
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
        //Ư�� �Ÿ����� �߰�
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
        //ȸ�� �ൿ
        //�÷��̾ �ٶ� ���·�
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
        //���������� üũ
        return isAttack;
    }

    private NodeStates IsHurted()
    {
        //�ǰݵǾ����� Ȯ��
        if (isHurt)
        {
            animator.SetTrigger(ScavengerHash.Hurt);
            return NodeStates.SUCCESS;
        }
        return NodeStates.FAILURE;
    }

    private NodeStates DoStay()
    {
        //���(�ƹ� �ൿ�� ���� ����) �ش� ��ġ���� BT ����
        text.text = "CutScene Playing";
        return NodeStates.FAILURE;
    }

    private bool IsCutScene()
    {
        //�ƾ��� ��������� Ȯ��
        return isCutscene;
    }

    private NodeStates DoDead()
    {
        //���
        isDead = false;
        PlayAnimationDead();
        return NodeStates.FAILURE;
    }

    private bool IsDead()
    {
        //����Ͽ�����(ü�� 0) Ȯ��
        return isDead;
    }

    private float DistanceToPlayer()
    {
        return Vector3.Distance(player.transform.position, transform.position);
    }

    #region �ִϸ��̼�
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
    #endregion �ִϸ��̼�

}
