using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum WeaponType
{
    MeleeOneHand = 0,
    Magic = 2,
}

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/WeaponData", order = 2)]
public class WeaponData : ScriptableObject
{
    // ���� ������ ���� ���۰��� �ٸ��� ����
    // ex) 1000: �Ѽհ�1, 2001:������2, 3001:��հ�2
    public int weaponId;
    public string weaponName;
    public Sprite weaponImage;
    public LazyLoadReference<GameObject> weaponPrefab;
    public int meleeDamage;
    public int magicDamage;
    public WeaponType weaponType;
    public Vector3 RHandMatchPosition;
    public Vector3 LHandMatchPosition;
    public Quaternion HandMatchRotation;
}
