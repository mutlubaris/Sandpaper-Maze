using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AdvanceUI;

public class TutorialPanel : AdvancePanel
{
    private void OnEnable()
    {
        if (Managers.Instance == null)
            return;

        SceneController.Instance.OnSceneLoaded.AddListener(ShowPanelAnimated);
        LevelManager.Instance.OnLevelStart.AddListener(HidePanelAnimated);
    }



    private void OnDisable()
    {
        if (Managers.Instance == null)
            return;

        SceneController.Instance.OnSceneLoaded.RemoveListener(ShowPanelAnimated);
        LevelManager.Instance.OnLevelStart.RemoveListener(HidePanelAnimated);
    }

    public override void ShowPanelAnimated()
    {
        base.ShowPanelAnimated();
        CanvasGroup.blocksRaycasts = false;
    }
}
