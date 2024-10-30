using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatObserver
{
    void OnHealthChanged(int _maxHealth, int _changedHealth)
    {

    }

    void OnSoulChanged(int _currentSoul)
    {

    }

    void OnWeaponChanged(WeaponData _changedWeapon)
    {

    }

    void OnMagicChanged(MagicData _changedMagic)
    {

    }

}
