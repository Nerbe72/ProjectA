using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    public ZombieStateMachine zombieStateMachine;

    [SerializeField] private CapsuleCollider attackCollider;

    private void Awake()
    {
        transform.position = SpawnPoint;
        transform.rotation = SpawnRotation;

        zombieStateMachine = new ZombieStateMachine(this);
    }

    private void OnEnable()
    {
        transform.position = SpawnPoint;
        transform.rotation = SpawnRotation;
    }

    private void Update()
    {
        zombieStateMachine.Update();
        PlayAnimationWalk();
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
                PlayerController.Instance.Hurt(enemyStat.AttackTypes[(int)EnemyAttack.A]);
                break;
            default:
                break;
        }
    }
}
