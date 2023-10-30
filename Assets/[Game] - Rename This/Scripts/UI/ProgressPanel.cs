using AdvanceUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressPanel : AdvancePanel
{
    private void OnEnable()
    {
        if (!Managers.Instance) return;
        LevelManager.Instance.OnLevelStart.AddListener(ShowPanelAnimated);
        LevelManager.Instance.OnLevelFinish.AddListener(HidePanelAnimated);
    }
    private void OnDisable()
    {
        if (!Managers.Instance) return;
        LevelManager.Instance.OnLevelStart.RemoveListener(ShowPanelAnimated);
        LevelManager.Instance.OnLevelFinish.RemoveListener(HidePanelAnimated);
    }
}
