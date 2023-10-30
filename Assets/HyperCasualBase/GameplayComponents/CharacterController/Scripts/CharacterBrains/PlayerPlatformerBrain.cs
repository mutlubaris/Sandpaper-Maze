using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPlatformerBrain : CharacterBrainBase, ITarget
{
    public Transform t => transform;

    public void Hit(int damage)
    {
        Character.DamageCharacter(damage);
    }

    public override void Initialize()
    {
        base.Initialize();
        gameObject.AddComponent<Collector>();
    }

    public override void Logic()
    {
        CharacterController.Move(new Vector3(InputManager.Instance.InputDirection.x, 0, InputManager.Instance.InputDirection.y));
    }
}
