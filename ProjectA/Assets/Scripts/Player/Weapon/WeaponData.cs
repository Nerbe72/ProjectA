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
    // 무기 종류에 따라 시작값을 다르게 설정
    // ex) 1000: 한손검1, 2001:스태프2, 3001:양손검2
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
