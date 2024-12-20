using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager Instance;

    //사전 데이터 호출용
    [SerializeField] private List<WeaponData> totalWeapons;
    [SerializeField] private List<int> optainWeaponId;
    [SerializeField] private List<MagicData> totalMagics; 

    //사전 전제: 모든 무기는 id 1000 이상부터 시작
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

    //모든 무기 데이터를 사전에 불러옴
    private void InitDataList()
    {
        //무기 딕셔너리 초기화
        allWeaponDictionary = new Dictionary<int, WeaponData>();
        for (int i = 0; i < totalWeapons.Count; i++)
        {
            allWeaponDictionary.Add(totalWeapons[i].weaponID, totalWeapons[i]);
        }

        //마법 딕셔너리 초기화
        allMagicDictionary = new Dictionary<int, MagicData>();
        for (int i = 0; i < totalMagics.Count; i++)
        {
            allMagicDictionary.Add(totalMagics[i].magicID, totalMagics[i]);
        }

        //소지목록 초기화
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
        //최대 시간복잡도가 1000을 넘지 않음
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
