using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    public ZombieStateMachine zombieStateMachine;

    [SerializeField] private CapsuleCollider attackCollider;

    public float destinationDistance { get { return 1.4f; } }
    public float distanceLimit { get { return 10f; } }
    public bool IsWaiting { get; set; }

    private Coroutine fsmDelayCo;

    protected override void Awake()
    {
        base.Awake();
        transform.position = SpawnPoint;
        transform.rotation = SpawnRotation;

        zombieStateMachine = new ZombieStateMachine(this);
    }

    private void OnEnable()
    {
        transform.localScale = Vector3.one;
        transform.position = SpawnPoint;
        transform.rotation = SpawnRotation;
        currentHp = enemyStat.Hp;
        zombieStateMachine.TransitionTo(zombieStateMachine.idleState);
    }

    private void Update()
    {
        PlayAnimationWalk();

        if (IsWaiting && fsmDelayCo == null)
        {
            fsmDelayCo = StartCoroutine(FSMDelay());
            return;
        }

        if (!IsWaiting)
            zombieStateMachine.Update();
    }

    public void ToggleAttackCollider()
    {
        attackCollider.enabled = true;
    }

    public void ToggleAttackColliderE()
    {
        attackCollider.enabled = false;
    }

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

    private IEnumerator FSMDelay()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        IsWaiting = false;
        fsmDelayCo = null;
        yield break;
    }
}
