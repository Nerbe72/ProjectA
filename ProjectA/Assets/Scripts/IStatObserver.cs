using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStatObserver
{
    void OnHealthChanged(int _maxHealth, int _changedHealth);

    void OnStaminaChanged(int _maxStamina, int _changedStamina);

    void OnManaChanged(int _maxMana, int _changedMana);

    void OnWeaponChanged(WeaponData _changedWeapon);

}
