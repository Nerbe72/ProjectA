using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/EnemyStatData", order = 1)]
public class EnemyData : ScriptableObject
{
    public string Name;
    [Tooltip("������ ��� ����� ü�¹� ǥ��")] public bool IsBoss;

    [Header("�÷��̾� �ν� ����")]
    [Tooltip("�� �þ߰�")][Range(0, 360)] public int sightAngle;
    [Tooltip("�� �þ߰Ÿ�")] public float sightDistance;
    [Tooltip("�þ�(�ü�) ����")] public Vector3 sightHeight;

    [Header("�⺻ ����")]
    public int Hp;
    public List<EnemyAttackType> AttackTypes;
    public int MeleeDefense;
    public int MagicDefense;

    [Header("����")]
    [Tooltip("���� ���¿��� ����")] public bool StartWithSit;
    [Tooltip("���� ���� ������ ����")] public int PatternCount;
}

[Serializable]
public class EnemyAttackType
{
    EnemyAttack Type;
    public int MeleeDamage;
    public int MagicDamage;
}
