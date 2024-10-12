using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    public static PlayerStat Instance;

    [Header("스탯 데이터")]
    //추후 serializefield 제거(디버깅)
    [SerializeField] private Stat stats;

    public int currentHealth;
    public int currentStamina;
    public int currentMana;
    public WeaponData currentWeapon;
    public int currentMeleeDamage;
    public int currentMeleeDefense;
    public int currentMagicDefense;

    private List<IStatObserver> observers = new List<IStatObserver>();

    private void Start()
    {
        SetStatFromDB();

        //테스트용
        stats.Health = 300;
        stats.Stamina = 120;
        stats.Mana = 80;
        Init();
    }

    private void OnDestroy()
    {
        observers.Clear();
    }

    private void SetStatFromDB()
    {
        SaveManager.Instance.LoadStatData();
    }

    private void Init()
    {
        //currentWeapon = stats.Weapon;
        SetHealth(stats.Health);
        SetStamina(stats.Stamina);
        SetMana(stats.Mana);
        SetMeleeDamage(stats.MeleeDamage);
        SetMagicDefense(stats.MeleeDefense);
        SetMagicDefense(stats.MagicDefense);
    }

    public void SetHealth(int _hp)
    {
        currentHealth = _hp;
        BroadcastHealth();
    }

    public void SetStamina(int _st)
    {
        currentStamina = _st;
        BroadcastStamina();
    }

    public void SetMana(int _ma)
    {
        currentMana = _ma;
        BroadcastMana();
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
        if (currentWeapon != null && currentWeapon.weaponId == _weaponId) return true;

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

    private void BroadcastStamina()
    {
        foreach (var observer in observers)
        {
            observer.OnStaminaChanged(stats.Stamina, currentStamina);
        }
    }

    private void BroadcastMana()
    {
        foreach (var observer in observers)
        {
            observer.OnManaChanged(stats.Mana, currentMana);
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
