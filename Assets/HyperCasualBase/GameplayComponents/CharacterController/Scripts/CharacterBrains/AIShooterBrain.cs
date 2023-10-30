using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIShooterBrain : CharacterBrainBase, ITarget
{
    public Transform t => transform;

    public void Hit(int damage)
    {
        Character.DamageCharacter(damage);
    }

    public override void Logic()
    {
        
    }
}
