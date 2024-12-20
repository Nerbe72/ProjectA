using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class BossState
{
    public int ID;
    public bool IsDeath;

    public BossState (int _id, bool _isDeath)
    {
        ID = _id;
        IsDeath = _isDeath;
    }
}
