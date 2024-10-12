using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    public static PlayerInput Instance;

    [HideInInspector] public bool blockingMoveInput;
    private bool blockingCameraInput;

    public bool IsDodge { get; private set; }
    public bool IsRun { get; private set; }
    public bool IsPause { get; private set; }
    public bool IsAttack { get; private set; }
    public bool IsTargeting { get; private set; }

    public float targetWheel { get; private set; }

    private bool refuseMove = false;
    private bool refuseRun = false;
    private bool refuseAttack = false;
    private bool refuseDodge = false;

    private Vector3 moveDir;

    private float runWaitTime = 0f;
    private float runWaitMax = 0.2f;

    WaitForSeconds attackWait = new WaitForSeconds(0.8f);

    Coroutine c_attackHolder;
    Coroutine c_runHolder;

    public Vector3 MoveInput
    {
        get
        {
            if (refuseMove)
                return Vector3.zero;

            return moveDir;
        }
    }

    private void Start()
    {
        SceneManager.sceneLoaded += ResetInput;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ResetInput;
    }

    // Update is called once per frame
    void Update()
    {
        SetMove();
        SetRun();
        SetDodge();
        SetAttack();
        SetTarget();
        SetTargetWheel();
        SetPause();
    }

    private void ResetInput(Scene _scene, LoadSceneMode _mode)
    {
        Cursor.lockState = CursorLockMode.Locked;
        Input.mousePosition.Set(0, 0, 0);
        Input.ResetInputAxes();
    }

    private void SetMove()
    {
        if (refuseMove) return;

        moveDir.Set(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
    }

    private void SetRun()
    {
        if (refuseRun)
        {
            IsRun = false;
            return;
        }

        if (KeyManager.Instance.KeyOn(Keys.Run))
            runWaitTime += Time.deltaTime;
        else
            runWaitTime = 0;

        IsRun = runWaitTime >= runWaitMax;
    }

    private void SetDodge()
    {
        IsDodge = KeyManager.Instance.KeyDown(Keys.Dodge);
    }

    private void SetAttack()
    {
        if (refuseAttack) return;

        if (c_attackHolder != null)
            StopCoroutine(c_attackHolder);

        if (Input.GetButtonDown("Fire1"))
            c_attackHolder = StartCoroutine(AttackCoolCo());
    }

    private void SetTarget()
    {
        IsTargeting = Input.GetMouseButtonDown(2);
    }

    private void SetTargetWheel()
    {
        targetWheel = Input.GetAxis("Mouse ScrollWheel");
    }

    private void SetPause()
    {
        IsPause = Input.GetKeyDown(KeyCode.Escape);
    }

    public void HoldMove(bool _hold)
    {
        refuseMove = _hold;
    }

    public void HoldRun(bool _hold)
    {
        IsRun &= !_hold;
        refuseRun = _hold;
    }

    public void HoldAttack(bool _hold)
    {
        IsAttack &= !_hold;
        refuseAttack = _hold;
    }

    public void HoldDodge(bool _hold)
    {
        IsDodge &= !_hold;
        refuseDodge = _hold;
    }

    private IEnumerator AttackCoolCo()
    {
        refuseAttack = true;
        IsAttack = true;
        yield return attackWait;
        refuseAttack = false;
        IsAttack = false;

        c_attackHolder = null;
        yield break;
    }
}
