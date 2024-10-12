using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CameraType
{
    POV,
    Target,
    Sit,
}

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance;

    [HideInInspector] public Camera main;
    [SerializeField] private List<GameObject> vCams;

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
            return;
        }

        SceneManager.sceneLoaded += ChangeCameraToMain;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ChangeCameraToMain;
    }

    private void ChangeCameraToMain(Scene _scene, LoadSceneMode _mode)
    {
        main = Camera.main;
    }

    public void ResetMousePosition(Scene _scene, LoadSceneMode _mode)
    {
        Input.mousePosition.Set(0, 0, 0);
    }

    public Vector3 GetEulerY()
    {
        return new Vector3(0, main.transform.eulerAngles.y, 0);
    }


    /// <param name="_setTarget">Ÿ���� �����ϴ� ��� true, Ÿ���� �����ϴ� ��� false</param>
    public void SetFollow(CameraType _type, Target _target = null)
    {
        if (_type == CameraType.Target && _target == null) return;

        SwitchCamera((int)_type);

        if (_type != CameraType.Target) return;

        vCams[(int)_type].GetComponent<CinemachineVirtualCamera>().LookAt = _target.transform;
    }

    private void SwitchCamera(int _followType)
    {
        //������� ����
        if (vCams[_followType].activeSelf)
            return;

        for (int i = 0; i < vCams.Count; i++)
        {
            if (i == _followType)
                vCams[i].SetActive(true);
            else
                vCams[i].SetActive(false);
        }
    }
}
