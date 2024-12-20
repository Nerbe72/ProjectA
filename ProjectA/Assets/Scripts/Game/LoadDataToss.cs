using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadDataToss : MonoBehaviour
{
    private void Start()
    {
        Time.timeScale = 0f;
        StartCoroutine(DelayedToss());
    }

    IEnumerator DelayedToss()
    {
        yield return new WaitForEndOfFrame();

        if (GameManager.IsNewGame)
        {
            SaveManager.Instance.ResetAllData();
        }
        Stat stat = SaveManager.Instance.GetPlayerStat();
        GameManager.NextScene = stat.MapName;
        PlayerController.Instance.transform.position = stat.SpawnPoint;
        SceneManager.LoadScene("Loading");

        yield break;
    }
}
