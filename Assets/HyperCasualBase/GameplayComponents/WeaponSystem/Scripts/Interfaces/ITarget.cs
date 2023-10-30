using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget 
{
    Transform t { get; }

    void Hit(int damage);
}
