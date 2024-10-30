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
    /// Ÿ���� �ִ°�� ���� ����
    /// </summary>
    /// <param name="_target"></param>
    /// <param name="_force">���� ����( 0 - 100 )</param>
    public void SetTrace(Target _target, int _force = 30)
    {
        if (_target == null) { isTrace = false; return; }

        StartCoroutine(TraceDelayCo());
        traceTarget = _target.transform;
        traceForce = Math.Clamp(_force, 0, 100);
    }

    /// <summary>
    /// �⺻���� �� ����
    /// </summary>
    /// <param name="_spawnPosition">���� ��ġ</param>
    /// <param name="_direction">�߻� ���� ����</param>
    /// /// <param name="_speed">�߻� �ӵ�</param>
    public void SetData(Vector3 _spawnPosition, Quaternion _direction, int _speed = 2)
    {
        transform.position = _spawnPosition;
        transform.rotation = _direction;
        bulletSpeed = _speed;
    }

    /// <summary>
    /// ������ �߻� ���¸� �ۼ�
    /// </summary>
    protected virtual void TracingPath()
    {

    }

    /// <summary>
    /// �⺻ �߻� ���¸� �ۼ�
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
