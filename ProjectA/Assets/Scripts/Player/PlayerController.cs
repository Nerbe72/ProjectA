using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    #region 필수
    private KeyManager keyManager;
    private CameraManager cameraManager;
    private TargetManager targetManager;

    private PlayerAnimation playerAnimation;
    private PlayerInput playerInput;
    private PlayerStat playerStat;
    private PlayerWeapon playerWeapon;

    private CharacterController characterController;

    [SerializeField] private GameObject followThis;
    #endregion

    #region 변수
    [Header("동작 변수")]
    public bool Hurting = false;
    public bool Attacking = false;
    public bool Dodging = false;
    public bool Dying = false;

    private bool respawn = false;
    private bool blockHurt = false;
    private bool blockGroundCheck = false;

    private bool isGround = false;
    private bool isSlope = false;
    
    private Vector3 slopeVelocity;
    private Vector3 moveDirection;
    private float rotationSpeed = 1500;
    private Vector2 walkingState = Vector2.zero;
    [SerializeField] private float dodgeEndTime;
    [SerializeField] private float MoveSpeed;
    [SerializeField] private float RunMultiply;
    [SerializeField] private float DodgeSpeed;


    WaitForSeconds dodgeWorkTime = new WaitForSeconds(0.15f);
    WaitForSeconds hurtWait = new WaitForSeconds(1f);
    private Coroutine c_dodgeHolder;
    private Coroutine c_hurtHolder;
    #endregion

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }

        playerInput = GetComponent<PlayerInput>();
        playerStat = GetComponent<PlayerStat>();
        playerAnimation = GetComponent<PlayerAnimation>();
        playerWeapon = GetComponent<PlayerWeapon>();
        characterController = GetComponent<CharacterController>();

        PlayerStat.Instance = playerStat;
        PlayerInput.Instance = playerInput;
    }

    private void Start()
    {
        //stats = SaveManager.Instance.GetRegularStat();
        targetManager = TargetManager.Instance;
        keyManager = KeyManager.Instance;
        cameraManager = CameraManager.Instance;

        playerWeapon.EquipWeapon(Hand.Right, WeaponManager.Instance.GetWeaponFromId(1000));
        playerWeapon.EquipWeapon(Hand.Right, WeaponManager.Instance.GetWeaponFromId(2000));
    }

    private void Update()
    {
        ReadInputDodge();
        ReadInputAttack();
    }

    private void FixedUpdate()
    {
        CheckGround();
        Rotation();
        Move();
        Dodge();
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        characterController = gameObject.GetComponent<CharacterController>();
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * characterController.height * 0.001f));
    }
#endif

    #region Actions
    private void CheckGround()
    {
        if (blockGroundCheck)
        {
            isGround = true;
            return;
        }

        isGround = Physics.Raycast(transform.position, Vector3.down, characterController.height * 0.001f);
    }

    private void Rotation()
    {
        if (Dodging || Attacking || Hurting || Dying) return;

        if (Mathf.Abs(playerInput.MoveInput.x) <= 0.015f && Mathf.Abs(playerInput.MoveInput.z) <= 0.015f) return;

        Vector3 forward = cameraManager.main.transform.forward;
        Vector3 right = cameraManager.main.transform.right;

        
        //타겟팅 상황에 따라
        if (targetManager.CurrentTarget != null && !playerInput.IsRun)
        {
            transform.rotation = Quaternion.Euler(cameraManager.GetEulerY());
            playerAnimation.SetWeightTarget(playerStat.currentWeapon, true);
        }
        else
        {
            Vector3 targetDirection = forward * playerInput.MoveInput.z + right * playerInput.MoveInput.x;
            targetDirection.y = 0f;
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            playerAnimation.SetWeightTarget(playerStat.currentWeapon, false);
        }
    }

    private void Move()
    {
        if (Attacking || Dodging || Hurting || Dying)
        {
            moveDirection = Vector3.zero;
            return;
        }

        Vector3 forward = cameraManager.main.transform.forward;
        Vector3 right = cameraManager.main.transform.right;

        moveDirection = (forward * playerInput.MoveInput.z + right * playerInput.MoveInput.x) * (playerInput.IsRun ? RunMultiply : 1f);
        moveDirection.y = isGround ? 0f : -9.81f;

        characterController.Move(moveDirection * MoveSpeed * Time.deltaTime);

        playerAnimation.PlayAnimationWalk(playerInput.MoveInput * (playerInput.IsRun ? RunMultiply : 1f), (targetManager.CurrentTarget != null && !playerInput.IsRun));
    }

    private void ReadInputDodge()
    {
        if (Attacking || Dodging || Dying) return;
        if (!playerInput.IsDodge) return;
        playerAnimation.PlayAnimationDodge();
    }

    private void ReadInputAttack()
    {
        if (Dodging || Dying) return;
        playerAnimation.PlayAnimationAttack(playerInput.IsAttack);
    }

    private void Dodge()
    {
        if (!Dodging || Dying) return;

        float currentEndTime = targetManager.CurrentTarget != null && !playerInput.IsRun ? dodgeEndTime / 3 : dodgeEndTime;
        Vector3 dodgeDirection = transform.forward;

        if (targetManager.CurrentTarget != null && !playerInput.IsRun)
            dodgeDirection = transform.forward * playerInput.MoveInput.z + transform.right * playerInput.MoveInput.x;

        characterController.Move(dodgeDirection * DodgeSpeed * Time.deltaTime);
    }

    //적에 의해 호출
    public void Hurt(EnemyAttackType _data)
    {
        if (blockHurt || Dying) return;
        if (c_dodgeHolder != null) return;

        int result = Math.Clamp(playerStat.currentHealth - (_data.MeleeDamage), 0, playerStat.GetStat().Health);

        playerAnimation.PlayHurt();

        if (result == 0)
        {
            Dead();
        }

        playerStat.SetHealth(result);
    }

    private void Dead()
    {
        playerAnimation.PlayDead();
    }

    private void Respawn()
    {
        transform.position = new Vector3(-20.28f, 0, 1.84f);

        playerAnimation.StopDead();
        playerStat.ResetHealth();
        respawn = false;
        ReleaseDead();
    }
    #endregion

    #region AnimationEvents
    public void HoldDodging()
    {
        Dodging = true;
    }

    public void ReleaseDodging()
    {
        Dodging = false;
    }

    public void HoldAttacking()
    {
        Attacking = true;
    }

    public void ReleaseAttacking()
    {
        Attacking = false;
    }

    //피격 잠금
    public void LockHurt()
    {
        blockHurt = true;
    }

    public void UnlockHurt()
    {
        blockHurt = false;
    }

    public void HoldHurting()
    {
        Hurting = true;
    }

    public void ReleaseHurting()
    {
        Hurting = false;
    }

    public void HoldAttackCollider()
    {
        playerWeapon.Attack(playerStat.currentWeapon.weaponId, true);
    }

    public void ReleaseAttackCollider()
    {
        playerWeapon.Attack(playerStat.currentWeapon.weaponId, false);
    }

    public void HoldDead()
    {
        Dying = true;
    }

    public void ReleaseDead()
    {
        Dying = false;
    }
    #endregion
}
