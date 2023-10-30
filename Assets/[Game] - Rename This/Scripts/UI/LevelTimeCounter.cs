using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelTimeCounter : MonoBehaviour
{
  	private float timer;
	private string timerStr;


#if UNITY_EDITOR
	private void OnEnable()
	{
		if (Managers.Instance == null)
			return;
		LevelManager.Instance.OnLevelStart.AddListener(() => timer = 0);
	}


	private void OnDisable()
	{
		if (Managers.Instance == null)
			return;
		LevelManager.Instance.OnLevelStart.AddListener(() => timer = 0);
	}


	private void Update()
	{
		if (Managers.Instance == null || !LevelManager.Instance.IsLevelStarted) return;
		timer += Time.deltaTime;
		timerStr = Mathf.Round(timer).ToString();
	}

	private void OnGUI()
	{
		if (!LevelManager.Instance.IsLevelStarted) return;
		GUIStyle gUIStyle = new GUIStyle();
		gUIStyle.fontSize = 100;
		GUI.Label(new Rect(50, 50, 600, 300), timerStr, gUIStyle);
	}

#endif
}
