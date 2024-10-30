using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    private PlayerStat stat;
    private PlayerWeapon weapon;

    private void Start()
    {
        stat = PlayerStat.Instance;
    }

    
}
