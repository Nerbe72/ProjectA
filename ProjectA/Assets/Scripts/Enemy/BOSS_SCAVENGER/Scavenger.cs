using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

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

public static class ScavengerHash
{
    public static readonly int Walk = Animator.StringToHash("Walk");
    public static readonly int Side = Animator.StringToHash("Side");
    public static readonly int AttackType = Animator.StringToHash("AttackType");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int Hurt = Animator.StringToHash("Hurt");
    public static readonly int DeadB = Animator.StringToHash("Dead");
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

    [Header("���� ü�¹�")]
    [SerializeField] private Canvas bossUICanvas;
    [SerializeField] private TMP_Text bossName;
    [SerializeField] private Slider bossHealthSlider;
    [Header("Debug")]
    [SerializeField] private TMP_Text text;

    private bool CanAttack => (Time.time - lastAttackTime) >= attackInterval;
    private bool CanChangePattern => (Time.time - lastPatternChangeTime) >= patternCooldownTime;

    private Coroutine healthCo;
    private float healthTemp = 0f;

    private void Update()
    {
        //���� �ѱ��(���� �ǰ�)
        if (Input.GetKeyDown(KeyCode.G))
        {
            Hit((100, 0));
        }

        //���� �ѱ��(���)
        if (Input.GetKeyDown(KeyCode.J))
        {
            Hit((99999, 99999));
        }

        //�ƾ� ���
        if (!isCutscene)
        {
            rootNode.Evaluate();
            if (!bossUICanvas.gameObject.activeSelf)
                bossUICanvas.gameObject.SetActive(true);
        }

        Vector3 localVelocity = transform.InverseTransformDirection(agent.velocity);
        animator.SetFloat(ScavengerHash.Walk, Mathf.Lerp(animator.GetFloat(ScavengerHash.Walk), localVelocity.z, Time.deltaTime * 3f));
        animator.SetFloat(ScavengerHash.Side, Mathf.Lerp(animator.GetFloat(ScavengerHash.Side), localVelocity.x, Time.deltaTime * 3f));
        Debug.Log($"{animator.GetFloat(ScavengerHash.Walk)} , {animator.GetFloat(ScavengerHash.Side)}");
    }

    protected override void InitStat()
    {
        base.InitStat();
        bossHealthSlider.maxValue = currentHp;
        bossHealthSlider.value = currentHp;
        bossName.text = enemyStat.Name;
    }

    protected override void InitForChild()
    {
        agent.acceleration = float.MaxValue;
        agent.stoppingDistance = 1.5f;
        rootNode = CreateBT();
    }

    private Node CreateBT()
    {
        return new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new ActionNode(CheckIsDead),
                new ActionNode(DoDead)
            }),
            new Sequence(new List<Node>
            {
                new ActionNode(CheckIsHit),
                new ActionNode(CheckIsNotAttacking),
                new ActionNode(PerformHit)
            }),
            new Sequence(new List<Node>
            {
                new ActionNode(CheckIsNotHitting),
                new ActionNode(AttemptAttack),
                new Selector(new List<Node>
                {
                    new ActionNode(PerformMeleeAttack),
                    new ActionNode(PerformRangedAttack)
                })
            }),
            new Sequence(new List<Node>
            {
                new ActionNode(CheckIsNotAttacking),
                new ActionNode(CheckIsNotHitting),
                new RandomSelector(new List<Node>
                {
                    new ActionNode(DoRetreat),
                    new ActionNode(DoSideWalk),
                    new ActionNode(DoChase)
                })
            }),
            new Sequence(new List<Node>
            {
                new ActionNode(PatternCooldown)
            })
        });
    }

    private NodeStates CheckIsNotAttacking()
    {
        if (isAttack)
        {
            return NodeStates.FAILURE;
        }
        return NodeStates.SUCCESS;
    }

    private NodeStates CheckIsHit()
    {
        if (isHit)
        {
            //�ǰ� ���� �ʱ�ȭ
            ResetHurt();
            if (healthCo != null)
                StopCoroutine(healthCo);
            healthCo = StartCoroutine(HpBarLerp());
            return NodeStates.SUCCESS;
        }
        return NodeStates.FAILURE;
    }

    private NodeStates CheckIsNotHitting()
    {
        if (isHitting)
        {
            return NodeStates.FAILURE;
        }
        return NodeStates.SUCCESS;
    }

    private NodeStates PerformHit()
    {
        isHitting = true;

        if (DistanceToPlayer() <= 2.5f)
        {
            // �÷��̾���� �Ÿ��� 2.5 �̳��� ��� ���� �� �ִϸ��̼� ���
            agent.updateRotation = false;
            Vector3 targetDir = transform.position - player.transform.position;
            agent.speed = evadeSpeed;
            agent.SetDestination(transform.position + targetDir.normalized * 4f); // 2.5��ŭ ª�� �Ÿ� ����
            transform.rotation = Quaternion.LookRotation(-targetDir); // �÷��̾ �ٶ� ä ����
            animator.SetTrigger(ScavengerHash.Hurt);
            text.text = "<color=yellow>Hit Retreat</color>";
            return NodeStates.SUCCESS;
        }
        else
        {
            // �÷��̾���� �Ÿ��� 2.5 �̻��� ��� �ִϸ��̼Ǹ� ���
            animator.SetTrigger(ScavengerHash.Hurt);
            agent.SetDestination(transform.position);
            text.text = "<color=yellow>Hit Animation</color>";
            return NodeStates.SUCCESS;
        }
    }

    private NodeStates CheckIsDead()
    {
        if (isDead)
        {
            return NodeStates.SUCCESS;
        }
        return NodeStates.FAILURE;
    }

    private NodeStates DoDead()
    {
        PlayAnimationDead();
        if (bossUICanvas.gameObject.activeSelf)
            bossUICanvas.gameObject.SetActive(false);
        return NodeStates.RUNNING;
    }

    private NodeStates AttemptAttack()
    {
        if (!isAttack && CanAttack)
        {
            return NodeStates.SUCCESS;
        }

        if (isAttack)
            lastAttackTime = Time.time;
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
            
            text.text = "<color=red>Perform Melee Attack</color>";
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
            Vector3 targetPosition = player.transform.position;
            agent.SetDestination(targetPosition);
            animator.SetInteger(ScavengerHash.AttackType, (int)ScavengerAttackType.B);
            animator.SetTrigger(ScavengerHash.Attack);
            
            text.text = "<color=red>Perform Ranged Attack</color>";
            return NodeStates.RUNNING;
        }
        return NodeStates.FAILURE;
    }

    private float CalculateRangedAttackSpeed(float distance)
    {
        // ���� �ð��� ���� �ӵ� ���
        return distance / 1.2f; // �ִϸ��̼� ���� �ð�: 1.2
    }

    private NodeStates DoRetreat()
    {
        agent.updateRotation = false;
        Vector3 targetDir = (player.transform.position - transform.position).normalized;
        agent.speed = retreatSpeed;
        transform.rotation = Quaternion.LookRotation(targetDir);
        agent.SetDestination(transform.position - targetDir * 3f);
        text.text = "<color=yellow>Retreat</color>";
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
        agent.SetDestination(transform.position + finalDirection * 3f);
        text.text = "Side Walk";
        return NodeStates.SUCCESS;
    }

    private NodeStates DoChase()
    {
        agent.updateRotation = true;
        agent.speed = chaseSpeed;
        
        Vector3 targetPosition = player.transform.position;
        agent.SetDestination(targetPosition);
        text.text = "<color=green>Chase</color>";
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

    public override void Attack(Collider _other, EnemyAttack _pattern = EnemyAttack.A)
    {
        if (_other == null) return;
        if (_other == this) return;
        if (!_other.CompareTag("Hurtable") && !_other.CompareTag("Player")) return;

        switch (_pattern)
        {
            case EnemyAttack.A:
                PlayerController.Instance.Hurt(enemyStat, EnemyAttack.A);
                break;
            default:
                break;
        }
    }

    public void FlagDeath()
    {
        //������Ʈ ���� �� ���ó��
    }

    private IEnumerator HpBarLerp()
    {
        float time = 0;
        float temp = bossHealthSlider.value;

        while (true)
        {
            time += Time.deltaTime * 9f;

            bossHealthSlider.value = Mathf.Lerp(temp, currentHp, time);

            if (time >= 1f) break;

            yield return new WaitForEndOfFrame();
        }

        healthCo = null;
        yield break;
    }
}
