using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugPanel : AdvanceUI.AdvancePanel
{

    public TMP_InputField TimeField;
    public Slider TimeSlider;


    protected override void Start()
    {
        base.Start();
        TimeField.text = Time.timeScale.ToString();
        TimeSlider.value = Time.timeScale;
    }

    private void OnEnable()
    {
        if (Managers.Instance == null)
            return;

        InputManager.Instance.OnTouch.AddListener(ToogleOnCount);
    }

    private void OnDisable()
    {
        if (Managers.Instance == null)
            return;

        InputManager.Instance.OnTouch.AddListener(ToogleOnCount);
    }

    public void ToogleOnCount(int value)
    {
        if (value == 3)
            TooglePanel();
    }

    public void LoadNextLevel()
    {
        LevelManager.Instance.LoadNextLevel();
    }

    public void LoadPreviousLevel()
    {
        LevelManager.Instance.LoadPreviousLevel();
    }

    public void RestartLevel()
    {
        LevelManager.Instance.ReloadLevel();
    }

    public void CompilateStage(bool value)
    {
        GameManager.Instance.CompeleteStage(value);
    }

    public void IncreaseTimeScale()
    {
        Time.timeScale += 0.5f;
        TimeField.text = Time.timeScale.ToString();
    }

    public void DecreaseTimeScale()
    {
        Time.timeScale -= 0.5f;
        TimeField.text = Time.timeScale.ToString();
    }

    public void SetTimeScale()
    {
        float input = float.Parse(TimeField.text);
        Time.timeScale = input;
        TimeField.text = Time.timeScale.ToString();
        TimeSlider.value = input;
    }

    public void SetTimeScaleSlider()
    {
        Time.timeScale = TimeSlider.value;
        TimeField.text = Time.timeScale.ToString();
    }

    public void ResetTime()
    {
        
        Time.timeScale = 1f;
        TimeField.text = Time.timeScale.ToString();
        TimeSlider.value = 1f;
    }
}
