using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat Instance;

    [Header("스탯 데이터")]
    private Stat stats;

    public int currentHealth;
    public int currentSouls;
    public WeaponData currentWeapon;
    public MagicData currentMagic;
    public int currentMeleeDamage;
    public int currentMeleeDefense;
    public int currentMagicDefense;
    public int currentMagicCount;

    private List<IStatObserver> observers = new List<IStatObserver>();

    private void Start()
    {
        stats = SaveManager.Instance.GetPlayerStat();
        Init();
    }

    private void OnDestroy()
    {
        observers.Clear();
    }

    private void Init()
    {
        //currentWeapon = stats.Weapon;
        SetHealth(stats.Health);
        SetMeleeDamage(stats.MeleeDamage);
        SetMagicDefense(stats.MeleeDefense);
        SetMagicDefense(stats.MagicDefense);
        if (currentMagic != null)
            currentMagicCount = currentMagic.BulletCount;
    }

    public void SetHealth(int _hp)
    {
        currentHealth = _hp;
        BroadcastHealth();
    }

    public void GetSoul(int _souls)
    {
        currentSouls += _souls;
        BroadcastSouls();
    }

    public bool UseSoul(int _souls)
    {
        if (currentSouls - _souls <= 0)
            return false;
        currentSouls = System.Math.Clamp(currentSouls - _souls, 0, int.MaxValue);

        BroadcastSouls();
        return true;
    }

    public void SetMeleeDamage(int _medmg)
    {
        currentMeleeDamage = _medmg;
    }

    public void SetMeleeDefense(int _medef)
    {
        currentMeleeDefense = _medef;
    }

    public void SetMagicDefense(int _madef)
    {
        currentMagicDefense = _madef;
    }

    public bool SetWeapon(int _weaponId)
    {
        //기존의 무기를 그대로 설정하려 하는 경우 pass
        if (currentWeapon != null && currentWeapon.weaponID == _weaponId) return true;

        currentWeapon = WeaponManager.Instance.GetWeaponFromId(_weaponId);

        BroadcastWeapon();
        return true;
    }

    public Stat GetStat()
    {
        return stats;
    }

    public void ResetHealth()
    {
        currentHealth = stats.Health;
        BroadcastHealth();
    }

    public void AddObserver(IStatObserver _observer)
    {
        if (_observer == null) return;

        observers.Add(_observer);
    }

    public void RemoveObserver(IStatObserver _observer)
    {
        if (_observer == null) return;

        observers.Remove(_observer);
    }

    private void BroadcastHealth()
    {
        foreach (var observer in observers)
        {
            observer.OnHealthChanged(stats.Health, currentHealth);
        }
    }

    private void BroadcastSouls()
    {
        foreach (var observer in observers)
        {
            observer.OnSoulChanged(currentSouls);
        }
    }

    private void BroadcastWeapon()
    {
        foreach (var observer in observers)
        {
            observer.OnWeaponChanged(currentWeapon);
        }
    }
}
