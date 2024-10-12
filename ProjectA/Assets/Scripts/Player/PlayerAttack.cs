using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerStat stat;
    private PlayerWeapon weapon;

    private void Start()
    {
        stat = PlayerStat.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (!other.CompareTag("Enemy") || other.CompareTag("Hurtable")) return;

        Enemy target = other.GetComponent<Enemy>();

        if (target == null) return;

        target.Hurt(GetDamageGiven());
    }

    public (int melee, int magic) GetDamageGiven()
    {
        (int melee, int magic) damageGiven = (0, 0);

        if (stat.currentWeapon == null)
            return (stat.currentMeleeDamage, 0);
        damageGiven = (stat.currentMeleeDamage + stat.currentWeapon.meleeDamage, stat.currentWeapon.magicDamage);

        return damageGiven;
    }
}
