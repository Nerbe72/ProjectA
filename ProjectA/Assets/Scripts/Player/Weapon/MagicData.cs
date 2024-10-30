using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/MagicData", order = 3)]
public class MagicData : ScriptableObject
{
    public int magicID;
    public string magicName;
    public int BulletCount;
    public int MagicDamage;
    public Sprite magicImage;
    public LazyLoadReference<GameObject> bulletStyle;
}
