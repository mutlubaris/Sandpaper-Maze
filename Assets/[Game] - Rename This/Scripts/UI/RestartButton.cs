using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestartButton : AdvanceUI.AdvanceButton
{
    protected override void OnEnable()
    {
        base.OnEnable();
        OnClickEvent.AddListener(() => GameManager.Instance.CompeleteStage(false));
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        OnClickEvent.RemoveListener(() => GameManager.Instance.CompeleteStage(false));
    }
}
