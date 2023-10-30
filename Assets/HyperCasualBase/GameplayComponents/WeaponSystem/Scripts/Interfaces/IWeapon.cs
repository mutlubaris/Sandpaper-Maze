using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWeapon
{
    WeaponData WeaponData { get; set; }
    void Shoot();
    void Reload();
    void Aim();
    void FindTargets();
}
