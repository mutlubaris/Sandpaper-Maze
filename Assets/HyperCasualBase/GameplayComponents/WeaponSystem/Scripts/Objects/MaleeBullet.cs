using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaleeBullet : Bullet
{
    protected override void Update()
    {
        
    }

    protected override void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if(damagable != null)
        {
            if(!ReferenceEquals(damagable, owner))
            {
                damagable.Damage(Damage);
                Instantiate(Decal, damagable.transform.position, Quaternion.identity);
            }
        }
    }
}
