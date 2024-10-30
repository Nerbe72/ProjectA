using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBullet : MonoBehaviour
{
    protected const int rotateAngle = 90;
    protected bool isTrace;
    protected Transform traceTarget;
    protected int traceForce;
    protected int bulletSpeed;

    private void Update()
    {
        if (isTrace)
            TracingPath();
        else
            DefaultPath();
    }

    /// <summary>
    /// 타겟이 있는경우 추적 설정
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_force">추적 강도( 0 - 100 )</param>
    public void SetTrace(Target _target, int _force = 30)
    {
        if (_target == null) { isTrace = false; return; }

        StartCoroutine(TraceDelayCo());
        traceTarget = _target.transform;
        traceForce = Math.Clamp(_force, 0, 100);
    }

    /// <summary>
    /// 기본적인 값 설정
    /// </summary>
    /// <param name="_spawnPosition">스폰 위치</param>
    /// <param name="_direction">발사 시작 방향</param>
    /// /// <param name="_speed">발사 속도</param>
    public void SetData(Vector3 _spawnPosition, Quaternion _direction, int _speed = 2)
    {
        transform.position = _spawnPosition;
        transform.rotation = _direction;
        bulletSpeed = _speed;
    }

    /// <summary>
    /// 추적시 발사 형태를 작성
    /// </summary>
    protected virtual void TracingPath()
    {

    }

    /// <summary>
    /// 기본 발사 형태를 작성
    /// </summary>
    protected virtual void DefaultPath()
    {

    }

    protected void BreakObject()
    {
        Destroy(gameObject);
        Destroy(this);
    }

    private IEnumerator TraceDelayCo()
    {
        yield return new WaitForSeconds(0.1f);
        isTrace = true;
        yield break;
    }
}
