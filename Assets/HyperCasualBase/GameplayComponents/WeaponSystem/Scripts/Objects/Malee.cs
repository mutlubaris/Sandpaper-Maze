using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Malee : WeaponBase
{
    public override void Shoot()
    {
        if (Time.time < lastShootTime + WeaponData.FireRate)
            return;

        base.Shoot();
        WeaponHolder.Character.OnCharacterAttack.Invoke();
    }
}
