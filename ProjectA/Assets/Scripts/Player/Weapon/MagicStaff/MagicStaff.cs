using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicStaff : MonoBehaviour, IWeapon
{
    private CameraManager cameraManager;
    private WeaponManager weaponManager;

    private int WeaponID;
    private int MeleeDamage;
    private int MagicDamage;

    [SerializeField] private Transform BulletOutFrom;


    private void Start()
    {
        cameraManager = CameraManager.Instance;
        weaponManager = WeaponManager.Instance;
    }

    public void SetData(WeaponData _data)
    {
        WeaponID = _data.weaponId;
        MeleeDamage = _data.meleeDamage;
        MagicDamage = _data.magicDamage;
    }

    public void Attack(bool _t)
    {
        //źȯ �߻�
        if (_t)
        {
            //BulletOutFrom���� ���� �� ī�޶� �ٶ󺸴� �������� �߻�
            //��, Ÿ���� ���ΰ�� Ÿ�� �������� �߻�
        }
        Debug.Log($"{name} Attack!");
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
