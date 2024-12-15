using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

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
    private Node rootNode;
    private ScavengerPattern prePattern = ScavengerPattern.Chase;

    private bool isChase = false;
    private bool isAttack = false;
    private bool isRetreating = false;
    private bool isMovingRight = true;
    private bool isEvading = false;

    private Vector3 targetPosition;

    private float sideWalkChangeTime = 0f;
    private float lastAttackTime = 0f;
    private float lastPatternChangeTime = 0f;

    [SerializeField] private List<CapsuleCollider> attackColliders;

    [Header("���� ���� �Ÿ�")]
    [SerializeField][Tooltip("�ٰŸ� ���� ���� (1.8)")] private float meleeRange;
    [SerializeField][Tooltip("���Ÿ� ���� ���� (1.9)")] private float rangedRange;

    [Header("���� �ð�")]
    [SerializeField][Tooltip("���� �õ� ���� (2.0)")] private float attackInterval;
    [SerializeField][Tooltip("���� ���� �ð� (1.0)")] private float patternCooldownTime;
    [SerializeField][Tooltip("ȸ�� ���� �ð� (0.5)")] private float evadeDuration;
    [SerializeField][Tooltip("���̵� ���� ���� ���� ���ð� (1.0)")] private float sideWalkInterval;

    [Header("���� �ӵ�")]
    [SerializeField][Tooltip("���� �ӵ� (0.4)")] private float retreatSpeed;
    [SerializeField][Tooltip("�߰� �ӵ� (3.5)")] private float chaseSpeed;
    [SerializeField][Tooltip("�¿� �̵� �ӵ� (0.5)")] private float sidewalkSpeed;
    [SerializeField][Tooltip("ȸ�� ���� �ӵ� (8.0)")] private float evadeSpeed;

    [Header("Debug")]
    [SerializeField] private TMP_Text text;

    private bool CanAttack => Time.time - lastAttackTime >= attackInterval;
    private bool CanChangePattern => Time.time - lastPatternChangeTime >= patternCooldownTime;

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

        if (!isCutscene)
            rootNode.Evaluate();

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        animator.SetFloat(ScavengerHash.Side, Mathf.Lerp(animator.GetFloat(ScavengerHash.Side), localVelocity.x, Time.deltaTime * 3f));
        animator.SetFloat(ScavengerHash.Walk, Mathf.Lerp(animator.GetFloat(ScavengerHash.Walk), localVelocity.z, Time.deltaTime * 3f));
    }

    protected override void InitForChild()
    {
        agent.acceleration = float.MaxValue;
        //InitBT();
        rootNode = CreateBT();
    }

    private Node CreateBT()
    {
        return new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new ActionNode(CheckIsDead),
                new ActionNode(DoNothing)
            }),
            new Sequence(new List<Node>
            {
                new ActionNode(AttemptAttack),
                new Selector(new List<Node>
                {
                    new ActionNode(PerformMeleeAttack),
                    new ActionNode(PerformRangedAttack)
                })
            }),
            new Sequence(new List<Node>
            {
                new ActionNode(CheckIsAttacking),
                new RandomSelector(new List<Node>
                {
                    new ActionNode(DoRetreat),
                    new ActionNode(DoSideWalk),
                    new ActionNode(DoChase)
                }),
                new Sequence(new List<Node>
                {
                    new ActionNode(PatternCooldown)
                })
            })
        });
    }

    private NodeStates CheckIsAttacking()
    {
        if (isAttack)
        {
            Debug.Log("Attacking");
            return NodeStates.FAILURE;
        }
        return NodeStates.SUCCESS;
    }

    private NodeStates CheckIsDead()
    {
        if (isDead)
        {
            return NodeStates.SUCCESS;
        }
        return NodeStates.FAILURE;
    }

    private NodeStates DoNothing()
    {
        // �ƹ� ���۵� ���� ����
        text.text = "<color=grey>Cutscene</color>";
        return NodeStates.RUNNING;
    }

    private NodeStates AttemptAttack()
    {
        if (CanAttack)
        {
            return NodeStates.SUCCESS;
        }
        return NodeStates.FAILURE;
    }

    private NodeStates PerformMeleeAttack()
    {
        if (DistanceToPlayer() <= meleeRange)
        {
            isAttack = true;
            agent.velocity = Vector3.zero;
            agent.SetDestination(transform.position);
            animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.A);
            animator.SetTrigger(ScavengerHash.Attack);
            lastAttackTime = Time.time;
            text.text = "<color=red>Perform Melee Attack</color>";
            // ������ ���� �� Ÿ�̸Ӹ� �ٽ� ������Ű�� ���� false�� �����մϴ�.
            return NodeStates.RUNNING;
        }
        return NodeStates.FAILURE;
    }


    private NodeStates PerformRangedAttack()
    {
        if (DistanceToPlayer() > rangedRange)
        {
            isAttack = true;
            float distance = DistanceToPlayer();
            float speed = CalculateRangedAttackSpeed(distance);
            agent.speed = speed;
            agent.stoppingDistance = 1.5f; // �÷��̾�� ��ġ�� �ʰ� ����
            Vector3 targetPosition = player.transform.position;
            agent.SetDestination(targetPosition);
            animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.B);
            animator.SetTrigger(ScavengerHash.Attack);
            lastAttackTime = Time.time;
            text.text = "<color=red>Perform Ranged Attack</color>";
            // ������ ���� �� Ÿ�̸Ӹ� �ٽ� ������Ű�� ���� false�� �����մϴ�.
            return NodeStates.RUNNING;
        }
        return NodeStates.FAILURE;
    }



    private float CalculateRangedAttackSpeed(float distance)
    {
        // ���� �ð��� ���� �ӵ� ���
        return distance / 1.2f; // �ִϸ��̼� ���� �ð�: 1.2
    }

    private NodeStates AttemptEvade()
    {
        if (isEvading)
        {
            agent.speed = evadeSpeed;
            agent.SetDestination(transform.position - transform.forward * evadeSpeed * evadeDuration);
            Invoke(nameof(EndEvade), evadeDuration);
            text.text = "<color=blue>Evade</color>";
            return NodeStates.RUNNING;
        }
        return NodeStates.SUCCESS;
    }

    private void EndEvade()
    {
        isEvading = false;
    }

    private NodeStates DoRetreat()
    {
        agent.updateRotation = false;
        Vector3 targetDir = (player.transform.position - transform.position).normalized;
        agent.speed = retreatSpeed;
        agent.SetDestination(transform.position - targetDir * retreatSpeed);
        transform.rotation = Quaternion.LookRotation(targetDir);
        text.text = "<color=yellow>Retreat</color>";
        Debug.Log("Retreat");
        return NodeStates.SUCCESS;
    }


    private NodeStates DoSideWalk()
    {
        agent.updateRotation = false;
        agent.speed = sidewalkSpeed;
        Vector3 direction = (player.transform.position - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        Vector3 left = Vector3.Cross(transform.up, direction).normalized;
        Vector3 right = Vector3.Cross(direction, transform.up).normalized;
        Vector3 finalDirection;

        sideWalkChangeTime += Time.deltaTime;
        if (sideWalkChangeTime >= sideWalkInterval)
        {
            isMovingRight = !isMovingRight;
            sideWalkChangeTime = 0f;
        }

        finalDirection = isMovingRight ? right : left;
        agent.SetDestination(transform.position + finalDirection * sidewalkSpeed);
        text.text = "Side Walk";
        Debug.Log("Side");
        return NodeStates.SUCCESS;
    }


    private NodeStates DoChase()
    {
        agent.updateRotation = true;
        agent.speed = chaseSpeed;
        agent.stoppingDistance = 1.5f; // �÷��̾�� ��ġ�� �ʰ� ����

        Vector3 targetPosition = player.transform.position;
        agent.SetDestination(targetPosition);
        text.text = "<color=green>Chase</color>";
        Debug.Log("Chase");
        return NodeStates.RUNNING;
    }

    private NodeStates PatternCooldown()
    {
        if (!isAttack && CanChangePattern)
        {
            lastPatternChangeTime = Time.time;
            return NodeStates.SUCCESS;
        }
        return NodeStates.FAILURE;
    }


    private float DistanceToPlayer()
    {
        // �÷��̾���� �Ÿ� ���
        return Vector3.Distance(transform.position, player.transform.position);
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
        int count = attackColliders.Count;
        for(int i = 0; i < count; i++)
        {
            attackColliders[i].enabled = true;
        }
    }

    public void ResetAttackTrigger()
    {
        int count = attackColliders.Count;
        for (int i = 0; i < count; i++)
        {
            attackColliders[i].enabled = false;
        }
    }
    #endregion �ִϸ��̼�

}
