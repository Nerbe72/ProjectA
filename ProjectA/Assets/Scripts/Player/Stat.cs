using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Stat
{
    public int Health;
    public int Souls;
    public int MeleeDefense;
    public int MagicDefense;
    public int MeleeDamage;
    public WeaponData Weapon;
    public MagicData Magic;
    public int WeaponID;
    public int MagicID;

    public string MapName;
    public Vector3 SpawnPoint;
}