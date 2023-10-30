using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableWeapon : CollectableBase
{
    public WeaponData WeaponData;

    public override void Collect(Collector collector)
    {
        WeaponHolder weaponHolder = collector.GetComponentInChildren<WeaponHolder>();
        if (weaponHolder == null)
            return;

        base.Collect(collector);
        weaponHolder.EquipWeapon(WeaponData);
        Destroy(gameObject);
    }
}
