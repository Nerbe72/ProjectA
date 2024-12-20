using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 0f;
        //세이브 매니저의 값에 맞춰 플레이어 위치와 맵 위치를 로드
        StartCoroutine(LoadSceneWait(GameManager.NextScene));
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }

    IEnumerator LoadSceneWait(string _name)
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(_name);
        Debug.Log(_name);
        while (!async.isDone)
        {
            Debug.Log("Loading");
            yield return null;
        }
        yield break;
    }
}
