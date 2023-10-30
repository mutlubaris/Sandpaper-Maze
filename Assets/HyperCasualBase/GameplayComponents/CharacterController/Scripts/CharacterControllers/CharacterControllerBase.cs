using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public abstract class CharacterControllerBase : InterfaceBase, ICharacterController
{
    public MonoBehaviour MonoBehaviour { get { return this; } }

    private Character character;
    public Character Character { get { return (character == null) ? character = GetComponent<Character>() : character; } }


    protected virtual void OnEnable()
    {
        Character.OnCharacterDie.AddListener(() =>
        {
            GetComponent<Collider>().enabled = false;
        });

        Character.OnCharacterRevive.AddListener(() =>
        {
            GetComponent<Collider>().enabled = true;
        });

        if (Managers.Instance == null)
            return;
    }

    protected virtual void OnDisable()
    {
        Character.OnCharacterDie.RemoveListener(() =>
        {
            GetComponent<Collider>().enabled = false;
        });

        Character.OnCharacterRevive.RemoveListener(() =>
        {
            GetComponent<Collider>().enabled = true;
        });

        if (Managers.Instance == null)
            return;
    }

    public virtual void Initialize()
    {
        //Debug.Log("Character Controller Type Intialized " + this.GetType());
    }
    public abstract void Move(Vector3 Direction);
    public abstract void Jump();

    public abstract float CurrentSpeed();

    public virtual void Dispose()
    {
        //Debug.Log("Character Controller Type Disposed " + this.GetType());
        Utilities.DestroyExtended(this);
    }

    public abstract Vector3 GetVelocity();
    public abstract Vector3 GetMovementVelocity();
    public abstract bool IsGrounded();
    public abstract void AddMomentum(Vector3 _momentum);
    public abstract bool IsSliding();
    public abstract Vector3 GetMomentum();

    //Events;
    public delegate void VectorEvent(Vector3 v);
    public VectorEvent OnJump;
    public VectorEvent OnLand;
}
