using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum Hand
{
    Right,
    Left,
}

public class PlayerWeapon : MonoBehaviour
{
    [Header("장착시킬 손 위치")]
    [SerializeField] private GameObject RHand;
    [SerializeField] private GameObject LHand;

    private int weaponIndexLimit = 2;

    private IWeapon[] equippedWeaponInstances;
    private int currentWeaponIndex;

    private void Awake()
    {
        equippedWeaponInstances = new IWeapon[weaponIndexLimit];
        currentWeaponIndex = 0;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SwapWeapon();
            Debug.Log($"장착한 무기 갯수: {equippedWeaponInstances.Length}, 현재 무기 인덱스: {currentWeaponIndex}\n" +
                $"현재 들고있는 무기: {WeaponManager.Instance.GetWeaponFromId(equippedWeaponInstances[currentWeaponIndex].GetId())}");
        }
    }

    public void EquipWeapon(Hand _hand, WeaponData _weapon)
    {
        if (_weapon.weaponPrefab.asset == null) return;

        GameObject gameObject = GameObject.Instantiate(_weapon.weaponPrefab.asset);
        IWeapon weapon = gameObject.GetComponent<IWeapon>();
        weapon.SetData(_weapon);
        equippedWeaponInstances[currentWeaponIndex] = weapon;
        
        gameObject.transform.parent = _hand == Hand.Left ? LHand.transform : RHand.transform;
        gameObject.transform.localPosition = _hand == Hand.Left ? _weapon.LHandMatchPosition : _weapon.RHandMatchPosition;
        gameObject.transform.localRotation = _weapon.HandMatchRotation;

        SetActiveWeapon();
    }

    //보여줄 무기를 선택
    public void SetShowenWeapon()
    {

    }

    public void UnEquipWeapon(int _id)
    {
        //equipedWeapons에서 제거
        //게임 오브젝트 제거
        int count = equippedWeaponInstances.Length;
        for (int i = 0; i < count; i++)
        {
            if (equippedWeaponInstances[i].GetId() == _id)
            {
                IWeapon go = equippedWeaponInstances[_id];
                equippedWeaponInstances[i] = null;
                go.UnEquip();
            }
        }
        Resources.UnloadUnusedAssets();
    }

    //들고있는 무기를 스왑함 A to B / A to null / null to A
    //애니메이션의 중간에 호출
    public void SwapWeapon()
    {
        int activeWeaponCount = equippedWeaponInstances.Count(w => w != null);
        
        //장착한 무기의 갯수가 1개 미만인 경우 스킵
        if (activeWeaponCount <= 1) return;

        if (equippedWeaponInstances[currentWeaponIndex] != null)
        {
            equippedWeaponInstances[currentWeaponIndex].GetSelf().gameObject.SetActive(false);
            currentWeaponIndex = (currentWeaponIndex + 1) % weaponIndexLimit;
        }

        if (equippedWeaponInstances[currentWeaponIndex] != null)
        {
            equippedWeaponInstances[currentWeaponIndex].GetSelf().gameObject.SetActive(true);
            PlayerStat.Instance.SetWeapon(equippedWeaponInstances[currentWeaponIndex].GetId());
        }
    }

    public void SetActiveWeapon()
    {
        int count = equippedWeaponInstances.Length;
        for (int i = 0; i < count; i++)
        {
            if (equippedWeaponInstances[i] != null)
            {
                equippedWeaponInstances[i].GetSelf().gameObject.SetActive(i == currentWeaponIndex);
            }
        }
    }

    public void Attack(int _currentWeaponID, bool _t)
    {
        int count = equippedWeaponInstances.Length;

        for (int i = 0; i < count; i++)
        {
            if (equippedWeaponInstances[i] == null) continue;

            if (equippedWeaponInstances[i].GetId() == _currentWeaponID)
                equippedWeaponInstances[i].Attack(_t);
        }
    }
}
