using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyAttack
{
    A,
    B,
    C,
    D,
}

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
public class Enemy : MonoBehaviour
{
    protected PlayerController player;

    
    [Tooltip("���� ��ġ")] public Vector3 SpawnPoint; //�����Ǵ� ��ġ. awake�ÿ� ����
    [Tooltip("���� ȸ����")] public Quaternion SpawnRotation; //������ �ٶ󺸴� ����

    [HideInInspector] public NavMeshAgent agent;
    protected Animator animator;

    protected int idFaced = 0;
    protected int idMove = 0;
    protected int idAttack = 0;
    protected int idHurt = 0;
    protected int idDead = 0;

    protected int currentHp;
    protected int currentDefense;

    [SerializeField] protected EnemyData enemyStat;

    public bool isFaced = false;
    public bool isHit = false;
    public bool isDead = false;
    protected bool isCutscene = false;

    /// <summary>
    /// ����(only FSM)
    /// </summary>
    protected bool isHitting = false;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = PlayerController.Instance;

        idFaced = Animator.StringToHash("Faced");
        idMove = Animator.StringToHash("Move");
        idAttack = Animator.StringToHash("Attack");
        idHurt = Animator.StringToHash("Hurt");
        idDead = Animator.StringToHash("Dead");

        InitStat();
        InitForChild();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (!other.CompareTag("PlayerAttack")) return;
        Hit(player.GetDamageGiven());
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //�νİ� �� ���� ǥ��
        Handles.color = Color.red;
        Vector3 rotatedForward = Quaternion.Euler(0, -enemyStat.sightAngle * 0.5f, 0) * transform.forward; //���� ���� (����-��/2)
        Handles.DrawSolidArc(transform.position + enemyStat.sightHeight, Vector3.up, rotatedForward, enemyStat.sightAngle, enemyStat.sightDistance);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (player.transform.position - transform.position));
        }

        SpawnPoint = transform.position;
        SpawnRotation = transform.rotation;
    }
#endif

    protected virtual void InitForChild()
    {

    }

    protected virtual void InitStat()
    {
        currentHp = enemyStat.Hp;
        currentDefense = enemyStat.MeleeDefense;
    }

    public bool CheckPlayerDistanceIn(float _dist)
    {
        return _dist * _dist >= (player.transform.position - transform.position).sqrMagnitude;
    }

    public bool CheckSpawnDistanceOut(float _dist)
    {
        return _dist * _dist <= (SpawnPoint - transform.position).sqrMagnitude;
    }

    /// <summary>
    /// for zombie fsm
    /// </summary>
    public bool Chase(float _dist = 0.1f)
    {
        LookPlayer();

        if (CheckPlayerDistanceIn(_dist))
        {
            agent.isStopped = true;
            agent.destination = transform.position;
            return true;
        }

        agent.destination = player.transform.position;
        return false;
    }

    public void LookPlayer()
    {
        transform.LookAt(player.transform.position);
    }

    public void ReturnSpawnPoint()
    {
        agent.destination = SpawnPoint;
    }

    public bool IsPlayerInSight()
    {
        Vector3 directionToPlayer = (player.transform.position - transform.position).normalized + enemyStat.sightHeight / 2;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        //�÷��̾���� �Ÿ��� �þߺ��� �� ��� ����
        if (distanceToPlayer > enemyStat.sightDistance) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        //�÷��̾ �ش� ���� ���� ������ ����
        if (angleToPlayer > enemyStat.sightAngle * 0.5f) return false;

        //�÷��̾� ���⿡ '��'�� ������ ����
        if (Physics.Raycast(transform.position, directionToPlayer, distanceToPlayer, LayerMask.GetMask("Wall"))) return false;

        return true;
    }

    public void ReturnPosition()
    {
        if (Vector3.Distance(SpawnPoint, agent.transform.position) <= 0.1f)
        {
            if (transform.rotation != SpawnRotation)
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, SpawnRotation, 30);
                agent.isStopped = true;
            }
        }
    }

    public Vector3 GetStartPosition()
    {
        return SpawnPoint;
    }

    public bool IsAnimationAttack()
    {
        return animator.GetCurrentAnimatorStateInfo(0).tagHash == idAttack;
    }

    public void PlayAnimationFaced()
    {
        animator.SetTrigger(idFaced);
    }

    public void PlayAnimationDead()
    {
        try
        {
            animator.SetTrigger("DeadT");
        }
        catch { }
        animator.SetBool(idDead, isDead);
    }

    public void PlayAnimationHurt()
    {
        animator.SetTrigger(idHurt);
    }

    public void PlayAnimationWalk()
    {
        animator.SetBool(idMove, agent.desiredVelocity != Vector3.zero);
    }

    //����
    public void TriggerAnimationAttack()
    {
        animator.SetTrigger(idAttack);
        agent.SetDestination(transform.position);
    }

    public void ResetAttack()
    {
        animator.ResetTrigger(idAttack);
    }

    public virtual void Attack(Collider _other, EnemyAttack _pattern = EnemyAttack.A)
    {

    }

    public void SetDead(bool _isTrue)
    {
        isDead = _isTrue;
    }

    public bool GetHurt()
    {
        return isHit;
    }

    public void Hit((int melee, int magic) _taken)
    {
        //���� �� �ִ� �ִ� ������: 1000. ��, ������ -�� �� ��� 1000 �ʰ� ����
        int takeMelee = (int)Mathf.Floor(_taken.melee * (1 - (enemyStat.MeleeDefense / (1000 + enemyStat.MeleeDefense))));
        //���� �� �ִ� �ִ� ������: 1100. ��, ���������� -�� �� ��� 1100 �ʰ� ����
        int takeMagic = (int)Mathf.Floor(_taken.magic * (1 - (enemyStat.MagicDefense / (1100 + enemyStat.MagicDefense))));

        int nextHp = Math.Clamp(currentHp - takeMelee - takeMagic, 0, enemyStat.Hp);

        currentHp = nextHp;

        SetHurt();

        if (currentHp == 0)
        {
            //dead
            ResetHurt();
            SetDead(true);
        }
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    public void SetHurt()
    {
        isHit = true;
    }

    public void ResetHurt()
    {
        isHit = false;
    }

    public void HoldHurting()
    {
        isHitting = true;
    }

    public void ReleaseHurting()
    {
        isHitting = false;
    }

    public void SetFaced()
    {
        isFaced = true;
    }

    public void SetCutscene()
    {
        isCutscene = true;
    }

    public void ResetCutscene()
    {
        isCutscene = false;
    }
}
