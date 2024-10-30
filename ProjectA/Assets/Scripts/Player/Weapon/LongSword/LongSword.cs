using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongSword : MonoBehaviour, IWeapon
{
    private BoxCollider weaponCollider;

    private int WeaponID;
    private int MeleeDamage;
    private int MagicDamage;

    private void Awake()
    {
        weaponCollider = GetComponent<BoxCollider>();
    }

    public void SetData(WeaponData _data)
    {
        WeaponID = _data.weaponID;
        MeleeDamage = _data.meleeDamage;
        MagicDamage = _data.magicDamage;
    }

    public void Attack(bool _t)
    {
        if (gameObject.activeSelf)
            weaponCollider.enabled = _t;
    }

    public void OnHand()
    {
        gameObject.SetActive(true);
    }

    public void OutHand()
    {
        gameObject.SetActive(false);
    }

    public int GetId()
    {
        return WeaponID;
    }

    public void Equip()
    {

    }

    public void UnEquip()
    {
        Destroy(gameObject);
        Destroy(this);
    }

    public GameObject GetSelf()
    {
        return gameObject;
    }
}
