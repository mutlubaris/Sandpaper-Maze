using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public abstract class WeaponBase : MonoBehaviour, IWeapon
{
    [SerializeField]
    private WeaponData weaponData;
    public WeaponData WeaponData { get { return weaponData; } set { weaponData = value; } }


    public GameObject BulletPrefab;
    public Transform FirePoint;

    private ITarget target;


    [Header("Current Data")]
    [ReadOnly]
    public int CurrentBulletInMagazine;

    private RotateTowardsVelocity rotateTowards;

    private WeaponHolder weaponHolder;
    public WeaponHolder WeaponHolder { get { return (weaponHolder == null) ? weaponHolder = transform.GetComponentInParent<WeaponHolder>() : weaponHolder; } }

    private ITarget selfTarget;
    public ITarget SelfTarget { get { return (selfTarget == null) ? selfTarget = transform.root.GetComponentInChildren<ITarget>() : selfTarget; } }

    private void Start()
    {
        rotateTowards = transform.root.GetComponentInChildren<RotateTowardsVelocity>();
        CurrentBulletInMagazine = WeaponData.MagazineCapacity;
    }

    private void Update()
    {
        FindTargets();
        Aim();
    }

    public virtual void FindTargets()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, WeaponData.Range);
        float minDistance = Mathf.Infinity;
        bool targetFound = false;

        foreach (var item in colliders)
        {
            ITarget currentTarget = item.GetComponent<ITarget>();

            if (!ReferenceEquals(currentTarget, SelfTarget))
            {
                if (currentTarget != null)
                {
                    float distance = Vector3.Distance(transform.position, currentTarget.t.position);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        target = currentTarget;
                        targetFound = true;
                    }
                }
            }
        }

        if (!targetFound)
            target = null;
    }
    public void Aim()
    {
        if (target == null)
        {
            rotateTowards.enabled = true;
            return;
        }

        FirePoint.transform.LookAt(target.t.position + Vector3.up);

        rotateTowards.enabled = false;
        var lookPos = target.t.position - rotateTowards.transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        rotateTowards.transform.rotation = Quaternion.Slerp(rotateTowards.transform.rotation, rotation, Time.deltaTime * 10f);

        Shoot();
    }


    public virtual void Reload()
    {

        if (CurrentBulletInMagazine > 0)
            return;

        WeaponHolder.NotifyReload();
        Run.After(3f, () => CurrentBulletInMagazine = WeaponData.MagazineCapacity);
    }

    protected float lastShootTime;

    [Button]
    public virtual void Shoot()
    {
        if (WeaponHolder.Character.isDead)
            return;

        if (CurrentBulletInMagazine == 0)
            return;

        if (Time.time < lastShootTime + (WeaponData.FireRate * WeaponHolder.Character.CharacterData.CharacterWeaponData.ShootRateDivider))
            return;

        lastShootTime = Time.time;
        GameObject bulletgo = Instantiate(BulletPrefab, FirePoint.transform.position, FirePoint.transform.rotation);
        Bullet bullet = bulletgo.GetComponent<Bullet>();
        bullet.Damage = WeaponData.DamagePerHit;
        bullet.owner = transform.root.GetComponentInChildren<IDamagable>();

        if (WeaponData.MagazineCapacity == -1)
            return;

        CurrentBulletInMagazine--;
        Reload();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.root.transform.position, WeaponData.Range);
    }
}
