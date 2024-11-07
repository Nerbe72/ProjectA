using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    //항상 increased가 더해진 값을 스탯으로 가짐
    private Stat currentStat;
    private Stat currentStatIncreased;
    private int currentSaveSlot;

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

        if (!LoadStatData())
        {
            //재생성
            currentStat = new Stat();

            currentStat.Health = 800;
            currentStat.MeleeDamage = 100;
            currentStat.MeleeDefense = 50;
            currentStat.MagicDefense = 50;
            currentStat.Souls = 0;
            currentStat.WeaponID = 1000;
            currentStat.MagicID = 10000;

            string jsonData = JsonUtility.ToJson(currentStat);
            string path = Path.Combine(Application.dataPath + "/Resources/Datas/PlayerStats.json");
            File.WriteAllText(path, jsonData);
        }
    }

    //호출 성공 여부 반환
    public bool LoadStatData()
    {
        currentStat = new Stat();

        try
        {
            string dataPath = "Datas/PlayerStats";
            var dataJson = Resources.Load<TextAsset>(dataPath);
            currentStat = JsonUtility.FromJson<Stat>(dataJson.text);

            currentStat.Weapon = WeaponManager.Instance.GetWeaponFromId(currentStat.WeaponID);
            currentStat.Magic = WeaponManager.Instance.GetMagicFromId(currentStat.MagicID);

            return true;
        }
        catch { return false; }
    }

    private void InitStatData()
    {

    }

    public Stat GetRegularStat()
    {
        //return from loaded data
        return currentStat;
    }

    public Stat GetSummaryStat()
    {
        return currentStatIncreased;
    }

    public void SaveStatData()
    {

    }

    public void LoadDisplayData()
    {

    }

    public void SaveDisplayData()
    {

    }

    public void LoadBossStateData()
    {
        //보스 사망 여부
    }

    public void SaveBossStateData()
    {

    }
}
