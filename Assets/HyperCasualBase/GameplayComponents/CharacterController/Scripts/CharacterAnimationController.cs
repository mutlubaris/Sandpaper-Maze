using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimationController : MonoBehaviour
{
    Character character;
    Character Character { get { return (character == null) ? character = GetComponentInParent<Character>() : character; } }

    List<Animator> animators;
    List<Animator> Animators { get { return (animators == null || animators.Count == 0) ? animators = new List<Animator>(GetComponentsInChildren<Animator>(true)) : animators; } }

    private int currentAnimationLayer;

    private void OnEnable()
    {
        Character.OnCharacterJump.AddListener(()=> TriggerAnimation("Jump"));
        Character.OnCharacterAttack.AddListener(()=> TriggerAnimation("Attack"));
        Character.OnCharacterDie.AddListener(OnDeath);
        Character.OnCharacterHit.AddListener(() => TriggerAnimation("Hit"));
        Character.OnCharacterRevive.AddListener(ResetTransform);
        Character.OnCharacterSet.AddListener(ResetTransform);
        Character.OnWeaponAcquired.AddListener(SetWeaponLayer);
        Character.OnWeaponReload.AddListener(() =>
        {
            TriggerAnimation("Reload");
            foreach (var Animator in Animators)
            {
                Animator.SetLayerWeight(Animator.GetLayerIndex("Reload"), 1);
            }

            Run.After(3f, () =>
            {
                foreach (var Animator in Animators)
                {
                    Animator.SetLayerWeight(Animator.GetLayerIndex("Reload"), 0);
                }
            });
            });
    }

    private void OnDisable()
    {
        Character.OnCharacterJump.RemoveListener(() => TriggerAnimation("Jump"));
        Character.OnCharacterAttack.RemoveListener(() => TriggerAnimation("Attack"));
        Character.OnCharacterDie.RemoveListener(OnDeath);
        Character.OnCharacterHit.RemoveListener(() => TriggerAnimation("Hit"));
        Character.OnCharacterRevive.RemoveListener(ResetTransform);
        Character.OnCharacterSet.RemoveListener(ResetTransform);
        Character.OnWeaponAcquired.RemoveListener(SetWeaponLayer);
        Character.OnWeaponReload.RemoveListener(() =>
        {
            TriggerAnimation("Reload");
            foreach (var Animator in Animators)
            {
                Animator.SetLayerWeight(Animator.GetLayerIndex("Reload"), 1);
            }

            Run.After(3f, () =>
            {
                foreach (var Animator in Animators)
                {
                    Animator.SetLayerWeight(Animator.GetLayerIndex("Reload"), 0);
                }
            });
        });

    }

    private void Update()
    {
        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        foreach (var Animator in Animators)
        {
            Animator.SetFloat("Speed", Character.CurrentSpeed);
            Animator.SetBool("isGrounded", Character.IsGrounded);
            Animator.SetBool("isDead", Character.isDead);
            for (int j = 0; j < Animator.layerCount -1; j++)
            {
                Animator.SetLayerWeight(j, 0);
            }

            Animator.SetLayerWeight(currentAnimationLayer, 1);
        }
    }
    private void SetWeaponLayer(WeaponData weaponData)
    {
        for (int i = 0; i < Animators.Count; i++)
        {
            for (int j = 0; j < Animators[i].layerCount; j++)
            {
                Animators[i].SetLayerWeight(j, 0);
            }

            if (Animators[i].gameObject.activeInHierarchy)
                currentAnimationLayer = weaponData.AnimationLayerID;
        }
    }

    private void TriggerAnimation(string animationID)
    {
        foreach (var Animator in Animators)
        {
            Animator.SetTrigger(animationID);
        }
    }

    private void OnDeath()
    {
        foreach (var Animator in Animators)
        {
            Animator.applyRootMotion = true;
            Animator.SetTrigger("Die");
        }
    }

    private void ResetTransform()
    {
        foreach (var Animator in Animators)
        {
            Animator.applyRootMotion = false;
            Transform parent = Animator.transform.parent;
            Animator.transform.SetParent(null);
            Animator.transform.position = parent.transform.position;
            Animator.transform.rotation = parent.transform.rotation;
            Animator.transform.SetParent(parent.transform);
        }
        
    }
}
