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

        //��󿡰� ������/ (����, ����) �������� ����
        other.GetComponent<Enemy>().Hit(PlayerController.Instance.GetDamageGiven());
        BreakObject();
    }

    protected override void TracingPath()
    {
        if (traceTarget == null) return;

        //Ÿ�ٿ� �����ϰų� ����ģ ��� ���� ����
        if (Vector3.Distance(transform.position, traceTarget.position) <= 0.5f)
            SetTrace(null);

        Vector3 directionTo = (traceTarget.transform.position - transform.position).normalized;

        // ������ ���� ������ ���� �� ���� / 100 * _force ���� ���� ȸ��
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
