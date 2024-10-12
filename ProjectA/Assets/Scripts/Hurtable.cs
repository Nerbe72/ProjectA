using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtable : MonoBehaviour
{
    [SerializeField] private float hitAngle;
    

    private void OnDrawGizmosSelected()
    {
        Vector3 forward = transform.forward;
        UnityEditor.Handles.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        forward = Quaternion.AngleAxis(-hitAngle * 0.5f, transform.up) * forward;
        UnityEditor.Handles.DrawSolidArc(transform.position, transform.up, forward, hitAngle, 1.0f);
    }
}
