using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICharacterController : IComponent
{
    MonoBehaviour MonoBehaviour { get; }

    void Initialize();
    float CurrentSpeed();
    void Move(Vector3 Direction);
    void Jump();
    void Dispose();

    //Getters;
    Vector3 GetVelocity();
    Vector3 GetMovementVelocity();
    bool IsGrounded();
    void AddMomentum(Vector3 _momentum);
    bool IsSliding();
    Vector3 GetMomentum();
}
