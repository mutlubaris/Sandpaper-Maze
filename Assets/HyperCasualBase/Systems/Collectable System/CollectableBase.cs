using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableBase : MonoBehaviour, ICollectable
{
    public GameObject CollectEffectPrefab;

    public virtual void Collect(Collector collector)
    {
        if(CollectEffectPrefab != null)
        {
            ParticleSystem collectEffect = Instantiate(CollectEffectPrefab, transform.position, transform.rotation).GetComponent<ParticleSystem>();
            var main = collectEffect.main;
            main.stopAction = ParticleSystemStopAction.Destroy;
        }
    }
}
