using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Target : MonoBehaviour, ITarget
{
    public Transform t { get => transform; }

    private IDamagable damagable;
    public IDamagable Damagable { get { return (damagable == null) ? GetComponentInChildren<IDamagable>() : damagable; } }

    public bool Animate;

    private bool isAnimating;

    public void Hit(int damage)
    {
        if (Damagable != null)
            damagable.Damage(damage);

        if(Animate)
        {
            if(!isAnimating)
            {
                isAnimating = true;

                transform.DOPunchScale(Vector3.one * 0.2f, 0.2f, 1, 0.5f).SetEase(Ease.InOutExpo).OnComplete(() => isAnimating = false);
            }
        }
    }
}
