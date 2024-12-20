using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    private void Awake()
    {
        Time.timeScale = 0f;
        //���̺� �Ŵ����� ���� ���� �÷��̾� ��ġ�� �� ��ġ�� �ε�
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
