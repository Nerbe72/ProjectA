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
        //키 입력 제한


        //키 입력 제한 해제

        //씬 이동
        yield break;
    }

    private IEnumerator ReturnBlack()
    {
        //키 입력 제한



        //키 입력 제한 해제
        //키 위치 초기화
        yield break;
    }

    public void OnMove(AxisEventData eventData)
    {
        Debug.Log("Drag");
    }

    /// <summary>
    /// 타이틀로 돌아갈 때 DontDestroyOnLoad된 오브젝트를 모두 제거함
    /// </summary>
    public void DestroyAllObjectsForMain()
    {
        foreach (var obj in dontDestroyObjects)
        {
            Destroy(obj);
        }
    }
}
