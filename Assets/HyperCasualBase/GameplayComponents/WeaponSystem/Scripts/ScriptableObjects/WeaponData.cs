using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponData : ScriptableObject
{
    public string WeaponName;
    public float FireRate;
    public int DamagePerHit;
    public int MagazineCapacity;
    [Range(0.1f, 50f)]
    public float Range;

    public int AnimationLayerID;
}
