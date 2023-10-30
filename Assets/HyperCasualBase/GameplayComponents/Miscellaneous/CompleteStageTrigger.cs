using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class CompleteStageTrigger : MonoBehaviour
{
    public bool OnEnter;
    public bool isSuccess;

    private void OnTriggerEnter(Collider other)
    {
        if (!OnEnter)
            return;

        if(other.TryGetComponent(out ObjectController objectController)) GameManager.Instance.CompeleteStage(isSuccess);
    }

    private void OnTriggerExit(Collider other)
    {
        if (OnEnter)
            return;

        if (other.TryGetComponent(out ObjectController objectController)) GameManager.Instance.CompeleteStage(isSuccess);
    }
}
