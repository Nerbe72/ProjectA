using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    public void SetData(WeaponData _data);
    public void Attack(bool _t);
    public void OnHand();
    public void OutHand();
    public void Equip();
    public void UnEquip();
    public int GetId();
    public GameObject GetSelf();
}
