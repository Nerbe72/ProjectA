using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponIndicator : MonoBehaviour, IStatObserver
{
    public static WeaponIndicator Instance;

    [SerializeField] private Image magicFrame;
    [SerializeField] private Image weaponFrame;
    [SerializeField] private Image potionFrame;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }

    }

    private void Start()
    {
        PlayerStat.Instance.AddObserver(this);
    }

    private void OnDestroy()
    {
        PlayerStat.Instance.RemoveObserver(this);
    }

    public void OnWeaponChanged(WeaponData _changedWeapon)
    {
        if (_changedWeapon == null)
        {
            weaponFrame.sprite = null;
            weaponFrame.color = Color.clear;
        }
        else
        {
            weaponFrame.sprite = _changedWeapon.weaponImage;
            weaponFrame.color = Color.white;
        }
    }

    #region 사용안함
    public void OnHealthChanged(int _maxHealth, int _changedHealth)
    {
    }

    public void OnManaChanged(int _maxMana, int _changedMana)
    {
    }

    public void OnStaminaChanged(int _maxStamina, int _changedStamina)
    {
    }
    #endregion

}
