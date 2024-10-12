using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    //�׻� increased�� ������ ���� �������� ����
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
            //�����

        }
    }

    //ȣ�� ���� ���� ��ȯ
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
        //���� ��� ����
    }

    public void SaveBossStateData()
    {

    }
}
