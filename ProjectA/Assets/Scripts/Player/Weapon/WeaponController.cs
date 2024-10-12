using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    [SerializeField] private Collider weaponCollider;

    private int Id;

    public void EnableCollider()
    {
        weaponCollider.enabled = true;
    }

    public void DisableCollider()
    {
        weaponCollider.enabled = false;
    }

    public void SetId(int _id)
    {
        Id = _id;
    }

    public int GetId()
    {
        return Id;
    }
}
