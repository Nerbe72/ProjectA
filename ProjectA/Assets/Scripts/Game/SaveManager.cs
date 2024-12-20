using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Serialization.Json;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    //항상 increased가 더해진 값을 스탯으로 가짐
    private Stat currentStat = new Stat();
    private Stat currentStatIncreased = new Stat();
    private int currentSaveSlot;

    private List<BossState> bossStates;

    private string defaultDataPath;
    private string bossPath;
    private string playerPath;

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

        defaultDataPath = Path.Combine(Application.persistentDataPath, "Datas");
        bossPath = Path.Combine(defaultDataPath, "BOSS_STATES.json");
        playerPath = Path.Combine(defaultDataPath, "PLAYER_STAT.json");

        if (!Directory.Exists(defaultDataPath))
        {
            Directory.CreateDirectory(defaultDataPath);
        }

        currentStat = LoadPlayerStat();
        SavePlayerStat();
        bossStates = LoadBossStateData();
        SaveBossStateData();
    }

    public Stat LoadPlayerStat()
    {
        Stat tempStat = new Stat();

        if (!File.Exists(playerPath))
        {
            tempStat.Health = 800;
            tempStat.MeleeDamage = 100;
            tempStat.MeleeDefense = 50;
            tempStat.MagicDefense = 50;
            tempStat.Souls = 0;
            tempStat.WeaponID = 1000;
            tempStat.MagicID = 10000;
            tempStat.MapName = "Stage1";
            tempStat.SpawnPoint = new Vector3(18.6f, 0, 7.14f);
        }
        else
        {
            string json = File.ReadAllText(playerPath);
            tempStat = JsonUtility.FromJson<Stat>(json);
        }
        tempStat.Weapon = WeaponManager.Instance.GetWeaponFromId(tempStat.WeaponID);
        tempStat.Magic = WeaponManager.Instance.GetMagicFromId(tempStat.MagicID);

        return tempStat;
    }

    public void SavePlayerStat()
    {
        if (!File.Exists(playerPath))
            File.Create(playerPath).Close();

        string json = JsonUtility.ToJson(currentStat, true);
        File.WriteAllText(playerPath, json);
    }

    /// <summary>
    /// 세이브 파일을 삭제하고 다시 로드함
    /// </summary>
    public void ResetAllData()
    {
        if (File.Exists(playerPath))
            File.Delete(playerPath);

        if (File.Exists(bossPath))
            File.Delete(bossPath);

        LoadPlayerStat();
        LoadBossStateData();
    }

    public Stat GetPlayerStat()
    {
        //return from loaded data
        return currentStat;
    }

    public Stat GetSummaryStat()
    {
        return currentStatIncreased;
    }

    public void NewGame_ResetAllFiles()
    {
        File.Delete(playerPath);
        File.Delete(bossPath);

        LoadPlayerStat();
        LoadBossStateData();
    }

    public List<BossState> LoadBossStateData()
    {
        //전체 보스 정보 불러오기
        if (!File.Exists(bossPath))
            return new List<BossState>();
        else
        {
            string json = File.ReadAllText(bossPath);

            List<BossState> wrap = JsonUtility.FromJson<List<BossState>>(json);
            return wrap;
        }
    }

    public void SaveBossStateData()
    {
        //보스 정보 내보내기
        if (!File.Exists(bossPath))
            File.Create(bossPath).Close();

        string json = JsonUtility.ToJson(new BossStateWrapper(bossStates), true);
        File.WriteAllText(bossPath, json);
    }

    /// <summary>
    /// 본인(ID)에 해당하는 사망정보 반환
    /// </summary>
    /// <param name="_id">보스 id</param>
    /// <returns></returns>
    public BossState GetBossState(int _id)
    {
        BossState current = new BossState(_id, false);

        if (bossStates == null) return current;

        foreach (var bs in bossStates)
        {
            if (bs.ID == _id) current = bs;
        }

        return current;
    }

    public void SetBossState(int _id, bool _isDeath = false)
    {
        int count = bossStates.Count;
        bool isContains = false;
        for (int i = 0; i < count; i++)
        {
            if (bossStates[i].ID == _id)
            {
                bossStates[i].IsDeath = _isDeath;
                Debug.Log(bossStates[i].IsDeath);
                isContains = true;
                break;
            }
        }
        if (!isContains)
            bossStates.Add(new BossState(_id, _isDeath));

        SaveBossStateData();
    }


    [Serializable]
    public class BossStateWrapper
    {
        public List<BossState> BOSS_STATES;

        public BossStateWrapper(List<BossState> _states)
        {
            BOSS_STATES = _states;
        }
    }

}
