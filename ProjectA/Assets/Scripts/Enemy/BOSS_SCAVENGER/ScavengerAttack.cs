using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScavengerAttack : MonoBehaviour
{
    [SerializeField] private Enemy self;

    private void OnTriggerEnter(Collider other)
    {
        self.Attack(other, EnemyAttack.A);
    }
}
