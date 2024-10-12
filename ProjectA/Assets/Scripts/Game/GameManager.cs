using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
            Destroy(this);

            return;
        }

        Application.targetFrameRate = 120;
        Cursor.lockState = CursorLockMode.Locked;

        SceneManager.sceneLoaded += EscapeBlack;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
            SceneManager.LoadScene("Stage1");
    }

    public void EnterBlack(string _nextSceneName)
    {
        StartCoroutine(GoBlack());
    }

    public void EscapeBlack(Scene _scene, LoadSceneMode _mode)
    {
        if (_scene.isLoaded)
            StartCoroutine(ReturnBlack());
    }

    private IEnumerator GoBlack()
    {
        //Ű �Է� ����


        //Ű �Է� ���� ����

        //�� �̵�
        yield break;
    }

    private IEnumerator ReturnBlack()
    {
        //Ű �Է� ����



        //Ű �Է� ���� ����
        //Ű ��ġ �ʱ�ȭ
        yield break;
    }
}
