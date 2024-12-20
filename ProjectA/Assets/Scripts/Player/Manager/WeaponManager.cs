using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    //���� ������ ȣ���
    [SerializeField] private List<WeaponData> totalWeapons;
    [SerializeField] private List<int> optainWeaponId;
    [SerializeField] private List<MagicData> totalMagics; 

    //���� ����: ��� ����� id 1000 �̻���� ����
    //id, data
    private Dictionary<int, WeaponData> allWeaponDictionary;
    private Dictionary<int, MagicData> allMagicDictionary;

    //fid:(Floor(id / 1000)), id
    private Dictionary<int, List<int>> optainWeapon;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameManager.dontDestroyObjects.Add(gameObject);
        } else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }

        InitDataList();
    }

    //��� ���� �����͸� ������ �ҷ���
    private void InitDataList()
    {
        //���� ��ųʸ� �ʱ�ȭ
        allWeaponDictionary = new Dictionary<int, WeaponData>();
        for (int i = 0; i < totalWeapons.Count; i++)
        {
            allWeaponDictionary.Add(totalWeapons[i].weaponID, totalWeapons[i]);
        }

        //���� ��ųʸ� �ʱ�ȭ
        allMagicDictionary = new Dictionary<int, MagicData>();
        for (int i = 0; i < totalMagics.Count; i++)
        {
            allMagicDictionary.Add(totalMagics[i].magicID, totalMagics[i]);
        }

        //������� �ʱ�ȭ
        optainWeapon = new Dictionary<int, List<int>>();
        List<int> keys = allWeaponDictionary.Keys.ToList<int>();
        for (int i = 0; i < keys.Count; i++)
        {
            int fid = WeaponIdIndex(keys[i]);
            if (!optainWeapon.ContainsKey(fid))
                optainWeapon.Add(fid, new List<int>());

            if (optainWeaponId.Contains(keys[i]))
                optainWeapon[fid].Add(keys[i]);
        }
    }

    public WeaponData GetWeaponFromId(int _id)
    {
        return allWeaponDictionary[_id];
    }

    public MagicData GetMagicFromId(int _id)
    {
        return allMagicDictionary[_id];
    }

    public int WeaponIdIndex(int _id)
    {
        int fid = 0;
        fid = (int)Mathf.Floor(_id / 1000);

        return fid;
    }

    public bool HasWeapon(int _id)
    {
        //�ִ� �ð����⵵�� 1000�� ���� ����
        int fid = (int)Mathf.Floor(_id / 1000);

        return optainWeapon[fid].Contains(_id);
    }

    public Dictionary<int, List<int>> GetOptainWeapons()
    {
        if (optainWeapon == null) return new Dictionary<int, List<int>>();

        return new Dictionary<int, List<int>>(optainWeapon);
    }

    public List<MagicData> GetTotalMagicData()
    {
        return new List<MagicData>(totalMagics);
    }
}
