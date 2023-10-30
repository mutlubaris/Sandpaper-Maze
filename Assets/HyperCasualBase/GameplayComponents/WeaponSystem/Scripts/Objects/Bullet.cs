using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Rigidbody rigidbody;
    public float lifeTime;
    public float Speed;
    public int BounceCount;
    [HideInInspector]
    public int Damage;

    public IDamagable owner;

    public GameObject Decal;

    private Vector3 startPos;

    protected virtual void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    protected virtual void Update()
    {
        rigidbody.velocity = transform.forward * Speed * Time.deltaTime;
    }



    protected virtual void OnCollisionEnter(Collision collision)
    {

        Instantiate(Decal, collision.GetContact(0).point, Quaternion.LookRotation(transform.forward, collision.GetContact(0).normal));

        ITarget target = collision.collider.GetComponentInChildren<ITarget>();
        if(target != null && !ReferenceEquals(target, owner))
        {
            target.Hit(Damage);
        }

        if(BounceCount > 0)
        {
            Vector3 dir = Vector3.Reflect(transform.forward, collision.GetContact(0).normal).normalized;
            transform.rotation = Quaternion.LookRotation(dir);
            BounceCount--;
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
