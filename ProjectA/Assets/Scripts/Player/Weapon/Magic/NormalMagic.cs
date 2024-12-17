using UnityEngine;

public class NormalMagic : MagicBullet
{
    private void OnTriggerEnter(Collider other)
    {
        if (other == null) return;
        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            BreakObject();
            return;
        }
        if (!other.CompareTag("Enemy")) return;
        if (other.GetComponent<Enemy>() == null) return;

        //대상에게 데미지/ (물리, 마법) 데미지를 입힘
        other.GetComponent<Enemy>().Hit(PlayerController.Instance.GetDamageGiven());
        BreakObject();
    }

    protected override void TracingPath()
    {
        if (traceTarget == null) return;

        //타겟에 근접하거나 지나친 경우 추적 해제
        if (Vector3.Distance(transform.position, traceTarget.position) <= 0.5f)
            SetTrace(null);

        Vector3 directionTo = (traceTarget.transform.position - transform.position).normalized;

        // 외적을 통해 방향을 구한 뒤 내적 / 100 * _force 값을 통해 회전
        float dot = Vector3.Dot(transform.forward, directionTo);
        Vector3 cross = Vector3.Cross(transform.forward, traceTarget.position - transform.position);

        float angle = rotateAngle * Mathf.Rad2Deg * (traceForce / 100);

        transform.Rotate(cross, angle * Time.deltaTime);

        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }

    protected override void DefaultPath()
    {
        transform.position += transform.forward * bulletSpeed * Time.deltaTime;
    }
}
