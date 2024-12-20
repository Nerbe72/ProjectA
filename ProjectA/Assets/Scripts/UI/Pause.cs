using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject frame;
    [SerializeField] private Button resume;
    [SerializeField] private Button exit;
    //esc 누르면 pause 켜지고 버튼 이벤트 등록

    private void Awake()
    {
        resume.onClick.AddListener(ResumeEvent);
        exit.onClick.AddListener(ExitEvent);
    }

    private void Update()
    {
        if ((Input.GetKeyDown(KeyCode.Escape)))
        {
            frame.SetActive(!frame.activeSelf);
            
            if (frame.activeSelf)
            {
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
    }

    private void ResumeEvent()
    {
        if (frame.activeSelf)
        {
            frame.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    private void ExitEvent()
    {
        Debug.Log("Clicked Exit");
        GameManager.Instance.DestroyAllObjectsForMain();
        SceneManager.LoadScene("Main");
    }
}