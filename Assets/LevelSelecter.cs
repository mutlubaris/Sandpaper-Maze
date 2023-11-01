using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelSelecter : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelText;

	private int levelIndex = 0;

	private void OnEnable()
	{
		SceneController.Instance.OnSceneLoaded.AddListener(InitialiseLevelText);
	}

	private void OnDisable()
	{
		SceneController.Instance.OnSceneLoaded.RemoveListener(InitialiseLevelText);
	}


	public void IncreaseLevel()
    {
		levelIndex++;
		if (levelIndex > LevelManager.Instance.LevelData.Levels.Count)
		{
			levelIndex = 1;
		}
		UpdateLevelText();
	}

	public void DecreaseLevel()
	{
		levelIndex--;
		if (levelIndex < 1)
		{
			levelIndex = LevelManager.Instance.LevelData.Levels.Count;
		}
		UpdateLevelText();
	}

	public void LoadLevel()
	{
		LevelManager.Instance.LevelIndex = levelIndex - 1;
		LevelManager.Instance.LoadGivenLevel(levelIndex - 1);
	}

	private void InitialiseLevelText()
	{
		levelIndex = LevelManager.Instance.LevelIndex + 1;
		UpdateLevelText();

	}

	private void UpdateLevelText()
	{

		levelText.SetText(levelIndex.ToString());
	}


}
