using System.Collections;
using System.Collections.Generic;
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

        //call from json
        if (!LoadStatData())
        {
            //재생성

        }
    }

    //호출 성공 여부 반환
    public bool LoadStatData()
    {
        currentStat = new Stat();

        //if no such file : generate and reset
        try
        {
            string dataPath = "StatData/" + currentSaveSlot;
            var dataJson = Resources.Load<TextAsset>(dataPath);
            currentStat = JsonUtility.FromJson<Stat>(dataJson.text);
            return true;
        }
        catch { return false; }
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
