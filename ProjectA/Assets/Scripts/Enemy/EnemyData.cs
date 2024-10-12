using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyStatData", order = 1)]
public class EnemyData : ScriptableObject
{
    public string Name;
    [Tooltip("보스인 경우 조우시 체력바 표시")] public bool IsBoss;

    [Header("플레이어 인식 범위")]
    [Tooltip("적 시야각")][Range(0, 360)] public int sightAngle;
    [Tooltip("적 시야거리")] public float sightDistance;
    [Tooltip("시야(시선) 높이")] public Vector3 sightHeight;

    [Header("기본 스탯")]
    public int Hp;
    public List<EnemyAttackType> AttackTypes;
    public int MeleeDefense;
    public int MagicDefense;

    [Header("패턴")]
    [Tooltip("앉은 상태에서 스폰")] public bool StartWithSit;
    [Tooltip("적이 갖는 패턴의 갯수")] public int PatternCount;
}

[Serializable]
public class EnemyAttackType
{
    EnemyAttack Type;
    public int MeleeDamage;
    public int MagicDamage;
}
