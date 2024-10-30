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
public class Enemy : MonoBehaviour
{
    private PlayerController player;

    
    [Tooltip("스폰 위치")] public Vector3 SpawnPoint; //스폰되는 위치. awake시에 설정
    [Tooltip("스폰 회전값")] public Quaternion SpawnRotation; //스폰시 바라보는 방향

    [HideInInspector] public NavMeshAgent agent;
    protected Animator animator;

    protected int idFaced = 0;
    protected int idMove = 0;
    protected int idAttack = 0;
    protected int idHurt = 0;
    protected int idDead = 0;

    protected int currentHp;
    protected int currentDamage;
    protected int currentDefense;

    [SerializeField] protected EnemyData enemyStat;

    public bool IsFaced = false;
    public bool isHurt = false;
    public bool isDead = false;

    protected bool isHurting = false;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = PlayerController.Instance;

        idFaced = Animator.StringToHash("Faced");
        idMove = Animator.StringToHash("Move");
        idAttack = Animator.StringToHash("Attack");
        idHurt = Animator.StringToHash("Hurt");
        idDead = Animator.StringToHash("Dead");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (!other.CompareTag("PlayerAttack")) return;

        Hurt(player.GetDamageGiven());
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        //인식각 및 범위 표시
        Handles.color = Color.red;
        Vector3 rotatedForward = Quaternion.Euler(0, -enemyStat.sightAngle * 0.5f, 0) * transform.forward; //시작 각도 (정면-각/2)
        Handles.DrawSolidArc(transform.position + enemyStat.sightHeight, Vector3.up, rotatedForward, enemyStat.sightAngle, enemyStat.sightDistance);

        if (Application.isPlaying)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (player.transform.position - transform.position));
        }
    }
#endif

    private void InitStat()
    {
        currentHp = enemyStat.Hp;
        //currentDamage = enemyStat.MeleeDamage;
        //currentDefense = enemyStat.MeleeDefense;
    }

    public bool CheckPlayerDistanceIn(float _dist)
    {
        return _dist >= Vector3.Distance(player.transform.position, transform.position);
    }

    public bool CheckSpawnDistanceOut(float _dist)
    {
        return _dist <= Vector3.Distance(SpawnPoint, transform.position);
    }

    public bool Chase(float _dist = 0.1f)
    {
        LookPlayer();

        if (CheckPlayerDistanceIn(_dist))
        {
            agent.isStopped = true;
            agent.destination = transform.position;
            return true;
        }

        agent.isStopped = false;
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

        //플레이어와의 거리가 시야보다 먼 경우 무시
        if (distanceToPlayer > enemyStat.sightDistance) return false;

        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

        //플레이어가 해당 각도 내에 없으면 무시
        if (angleToPlayer > enemyStat.sightAngle * 0.5f) return false;

        //플레이어 방향에 '벽'이 있으면 무시
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
        animator.SetTrigger("DeadT");
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

    public void TriggerAnimationAttack()
    {
        animator.SetTrigger(idAttack);
        agent.isStopped = true;
    }

    public void ResetAttackTrigger()
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

    public void SetHurt(bool _isTrue)
    {
        isHurt = _isTrue;
    }

    public bool GetHurt()
    {
        return isHurt;
    }

    //피격
    public void Hurt((int melee, int magic) _taken)
    {
        //가할 수 있는 최대 데미지: 1000. 단, 방어력이 -가 된 경우 1000 초과 가능
        int takeMelee = (int)Mathf.Floor(_taken.melee * (1 - (enemyStat.MeleeDefense / (1000 + enemyStat.MeleeDefense))));
        //가할 수 있는 최대 데미지: 1100. 단, 마법방어력이 -가 된 경우 1100 초과 가능
        int takeMagic = (int)Mathf.Floor(_taken.magic * (1 - (enemyStat.MagicDefense / (1100 + enemyStat.MagicDefense))));

        int nextHp = Math.Clamp(currentHp - takeMelee - takeMagic, 0, enemyStat.Hp);

        currentHp = nextHp;

        SetHurt();

        if (currentHp == 0)
        {
            //dead
            SetDead(true);
        }
    }

    public void Dead()
    {
        gameObject.SetActive(false);
    }

    public void SetHurt()
    {
        isHurt = true;
    }

    public void ResetHurt()
    {
        isHurt = false;
    }

    public void HoldHurting()
    {
        isHurting = true;
    }

    public void ReleaseHurting()
    {
        isHurting = false;
    }
}
