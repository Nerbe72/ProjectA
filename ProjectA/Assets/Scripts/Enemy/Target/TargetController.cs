using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public static TargetController Instance;
    /*
     * 필요한 정보
     * targetManager
     * cameraManager
     * playerInput
     * 
     * 
     * 
     * 수행하는 동작
     * playerInput으로 받은 targeting 변수에 의한 반응
     * 카메라 시점의 변화
     * 타깃의 추가/제거
     * 플레이어의 입력 방식 변화
     * 
     */

    private TargetManager targetManager;
    private CameraManager cameraManager;
    private PlayerInput playerInput;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            GameManager.dontDestroyObjects.Add(gameObject);
        } else
        {
            Destroy(gameObject);
            Destroy(this);
            return;
        }
    }

    private void Start()
    {
        targetManager = TargetManager.Instance;
        cameraManager = CameraManager.Instance;
        playerInput = PlayerInput.Instance;
    }

    // Update is called once per frame
    private void Update()
    {
        InputDetection();
    }

    private void InputDetection()
    {
        if (playerInput.IsTargeting)
        {
            if (targetManager.CurrentTarget != null)
            {
                cameraManager.SetFollow(CameraType.POV);
                targetManager.UnSetTarget();
            }
            else
            {
                targetManager.SetTarget(cameraManager.main.transform);
                cameraManager.SetFollow(CameraType.Target, targetManager.CurrentTarget);
            }
        }

        if (targetManager.CurrentTarget == null)
        {
            cameraManager.SetFollow(CameraType.POV);
        }

        switch (playerInput.targetWheel)
        {
            case < 0:
                targetManager.SearchTarget(false);
                cameraManager.SetFollow(CameraType.Target, targetManager.CurrentTarget);
                break;
            case > 0:
                targetManager.SearchTarget(true);
                cameraManager.SetFollow(CameraType.Target, targetManager.CurrentTarget);
                break;
            case 0:
            default:
                break;
        }
    }
}
