using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 0f;
        //다음 씬 이름을 적용
        StartCoroutine(LoadSceneWait(GameManager.NextScene));
        SceneManager.sceneLoaded += SetTimeScale;
    }

    private void SetTimeScale(Scene _scene, LoadSceneMode _mode)
    {
        Time.timeScale = 1f;
        SceneManager.sceneLoaded -= SetTimeScale;
    }

    IEnumerator LoadSceneWait(string _name)
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSecondsRealtime(0.5f);

        AsyncOperation async = SceneManager.LoadSceneAsync(_name);

        while (!async.isDone)
        {
            yield return null;
        }
        yield break;
    }
}
