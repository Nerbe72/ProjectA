using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public static class AnimatorHash
{
    public static readonly int Walk = Animator.StringToHash("Walk");
    public static readonly int Side = Animator.StringToHash("Side");
    public static readonly int Dodge = Animator.StringToHash("Dodge");
    public static readonly int Parry = Animator.StringToHash("Parry");
    public static readonly int AttackType = Animator.StringToHash("AttackType");
    public static readonly int Attack = Animator.StringToHash("Attack");
    public static readonly int Magic = Animator.StringToHash("Magic");
    public static readonly int Hurt = Animator.StringToHash("Hurt");
    public static readonly int DeadT = Animator.StringToHash("DeadT");
    public static readonly int DeadB = Animator.StringToHash("DeadB");
    public static readonly int LayerTarget = 1;
    public static readonly int LayerMagic = (int)WeaponType.Magic;
    public static readonly int LayerMagicT = (int)WeaponType.Magic + 1;
}

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private Coroutine c_targetedLerpHolder;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimationWalk(Vector3 _moveInput, bool _istargeted = false)
    {
        if (_istargeted)
        {
            animator.SetFloat(AnimatorHash.Walk, _moveInput.z);
            animator.SetFloat(AnimatorHash.Side, _moveInput.x);
        }
        else
        {
            float speed = Mathf.Clamp(Mathf.Abs(_moveInput.magnitude), 0, _moveInput.magnitude < 1.5 ? 1f : 1.5f);
            
            animator.SetFloat(AnimatorHash.Walk, Mathf.Lerp(animator.GetFloat("Walk"), speed, 0.5f));
        }
    }

    public void PlayAnimationDodge()
    {
        animator.SetTrigger(AnimatorHash.Dodge);
    }

    public bool PlayAnimationAttack(WeaponData _weaponData, bool _isTrue)
    {
        if (_weaponData == null) return false;

        animator.SetInteger(AnimatorHash.AttackType, Random.Range((int) 0, (int)2));

        animator.SetBool(AnimatorHash.Attack, _isTrue);

        animator.SetBool(AnimatorHash.Magic, WeaponType.Magic == _weaponData.weaponType);

        if (animator.GetCurrentAnimatorStateInfo(0).tagHash == AnimatorHash.Attack)
            return true;

        return false;
    }

    public void PlayAnimationParry()
    {
        animator.SetTrigger(AnimatorHash.Parry);
    }

    public void SetWeightFromWeapon(WeaponData _weapon, bool _setTarget)
    {
        if (_weapon == null) return;

        int target = _setTarget ? (int)_weapon.weaponType + 1 : (int)_weapon.weaponType;
        int targetLow = _setTarget ? (int)_weapon.weaponType : (int)_weapon.weaponType + 1;

        if (animator.GetLayerWeight(target) == 0)
            animator.SetLayerWeight(target, 1);
        if (animator.GetLayerWeight(targetLow) == 1)
            animator.SetLayerWeight(targetLow, 0);
    }

    public void SetWeightTarget(WeaponData _weapon, bool _setTarget)
    {
        int from = 0, to = 1;

        if (!_setTarget) { from = 1; to = 0; }

        if (animator.GetLayerWeight(AnimatorHash.LayerTarget) == to || c_targetedLerpHolder != null) return;

        c_targetedLerpHolder = StartCoroutine(LerpAnimation(AnimatorHash.LayerTarget, from, to));

        SetWeightFromWeapon(_weapon, _setTarget);
    }

    public void PlayHurt()
    {
        animator.SetTrigger(AnimatorHash.Hurt);
    }

    public void PlayDead()
    {
        animator.ResetTrigger(AnimatorHash.Hurt);
        animator.SetBool(AnimatorHash.DeadB, true);
        animator.SetTrigger(AnimatorHash.DeadT);
    }

    public void StopDead()
    {
        animator.SetBool(AnimatorHash.DeadB, false);
    }

    private IEnumerator LerpAnimation(int _target, float _from, float _to)
    {
        float time = 0;

        while (true)
        {
            time += Time.deltaTime * 10f;

            animator.SetLayerWeight(_target, Mathf.Lerp(_from, _to, time));

            if (time >= 1f)
            {
                break;
            }

            yield return new WaitForNextFrameUnit();
        }

        c_targetedLerpHolder = null;
        yield break;
    }
}
