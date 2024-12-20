using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IMoveHandler
{
    public static GameManager Instance;
    public static List<GameObject> dontDestroyObjects = new List<GameObject>();

    public static bool IsNewGame = false;
    public static string NextScene = "";

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
        //Cursor.lockState = CursorLockMode.Locked;
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

    public void OnMove(AxisEventData eventData)
    {
        Debug.Log("Drag");
    }

    /// <summary>
    /// Ÿ��Ʋ�� ���ư� �� DontDestroyOnLoad�� ������Ʈ�� ��� ������
    /// </summary>
    public void DestroyAllObjectsForMain()
    {
        foreach (var obj in dontDestroyObjects)
        {
            Destroy(obj);
        }
    }
}
