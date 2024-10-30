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

    private void Start()
    {
        InitEquipWeapon();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
        {
            SwapWeapon();
            //int activeWeaponCount = equippedWeaponInstances.Count(w => w != null);
            //Debug.Log($"장착한 무기 갯수: {activeWeaponCount}");
            string name = PlayerStat.Instance.currentWeapon.name;
            Debug.Log(name);
        }
    }

    private void InitEquipWeapon()
    {
        Dictionary<int, List<int>> optains = WeaponManager.Instance.GetOptainWeapons();

        foreach (int fid in optains.Keys)
        {
            int count = optains[fid].Count;
            for (int i = 0; i < count; i++)
            {
                EquipWeapon(Hand.Right, WeaponManager.Instance.GetWeaponFromId(optains[fid][i]));
            }
        }

        EquipMagic(WeaponManager.Instance.GetTotalMagicData()[0]);
    }

    public void EquipWeapon(Hand _hand, WeaponData _weapon)
    {
        if (_weapon.weaponPrefab.asset == null) return;

        GameObject gameObject = GameObject.Instantiate(_weapon.weaponPrefab.asset);
        IWeapon weapon = gameObject.GetComponent<IWeapon>();
        weapon.SetData(_weapon);

        if (equippedWeaponInstances[currentWeaponIndex] != null) currentWeaponIndex = (currentWeaponIndex + 1) % weaponIndexLimit;
        Debug.Log(currentWeaponIndex);
        equippedWeaponInstances[currentWeaponIndex] = weapon;
        Debug.Log(equippedWeaponInstances[currentWeaponIndex]);
        gameObject.transform.parent = _hand == Hand.Left ? LHand.transform : RHand.transform;
        gameObject.transform.localPosition = _hand == Hand.Left ? _weapon.LHandMatchPosition : _weapon.RHandMatchPosition;
        gameObject.transform.localRotation = _weapon.HandMatchRotation;

        SetActiveWeapon();
    }

    public void EquipMagic(MagicData _magic)
    {
        if (_magic.bulletStyle.asset == null) return;

        PlayerStat.Instance.currentMagic = _magic;
        PlayerStat.Instance.currentMagicCount = _magic.BulletCount;
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

    //애니메이션의 중간에 호출
    public void SwapWeapon()
    {
        int activeWeaponCount = equippedWeaponInstances.Count(w => w != null);
        
        //장착한 무기의 갯수가 1개 미만인 경우 스킵
        if (activeWeaponCount <= 1) return;

        currentWeaponIndex = (currentWeaponIndex + 1) % weaponIndexLimit;

        SetActiveWeapon();
    }

    //현재 무기를 활성화
    public void SetActiveWeapon()
    {
        int count = equippedWeaponInstances.Length;
        for (int i = 0; i < count; i++)
        {
            if (equippedWeaponInstances[i] != null)
            {
                equippedWeaponInstances[i].GetSelf().gameObject.SetActive(i == currentWeaponIndex);
                if (i == currentWeaponIndex) PlayerStat.Instance.SetWeapon(equippedWeaponInstances[i].GetId());
            }
        }
    }

    public void Attack(int _currentWeaponID, bool _isColliderTrue)
    {
        int count = equippedWeaponInstances.Length;

        for (int i = 0; i < count; i++)
        {
            if (equippedWeaponInstances[i] == null) continue;

            if (equippedWeaponInstances[i].GetId() == _currentWeaponID)
                equippedWeaponInstances[i].Attack(_isColliderTrue);
        }
    }
}
