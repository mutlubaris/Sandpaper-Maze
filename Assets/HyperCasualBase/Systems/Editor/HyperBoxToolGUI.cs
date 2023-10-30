using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.XR;

[InitializeOnLoad]
public class HyperBoxToolGUI : MonoBehaviour
{

	private static int SelectLevel;

	static string[] _options = new string[LevelData.Levels.Count];

	private static int currentTimeScale;

	static string[] timeScaleOption = new string[5] { "0.1", "0.5", "1", "2", "3" };

	private static bool isLevelAreaShowing = true;

	private static float timeSliderValue = 0.05F;

	public static bool panelIsShowing = true;

	private static bool wasMouseRight = false;

	private static bool gameStopped;



	static HyperBoxToolGUI()
	{
		SceneView.duringSceneGui += OnSceneView;

		EditorApplication.playModeStateChanged += LogPlayModeState;

		SelectLevel = PlayerPrefs.GetInt(PlayerPrefKeys.LastLevel);
		Time.timeScale = 1f;

		for (int i = 0; i < LevelData.Levels.Count; i++)
		{
			_options[i] = "" + (i + 1);
		}

		EventManager.OnLevelDataChange.AddListener(() =>
		{
			_options = new string[LevelData.Levels.Count];
			for (int i = 0; i < LevelData.Levels.Count; i++)
			{
				_options[i] = "" + (i + 1);
			}
		}
		);
	}

	~HyperBoxToolGUI()
	{
		EventManager.OnLevelDataChange.RemoveListener(() =>
		{
			_options = new string[LevelData.Levels.Count];
			for (int i = 0; i < LevelData.Levels.Count; i++)
			{
				_options[i] = "" + (i + 1);
			}
		});
	}


	private static void LogPlayModeState(PlayModeStateChange state)
	{
		if (state == PlayModeStateChange.EnteredEditMode && PlayerPrefs.GetString("gamePlayedOnGui") == "true")
		{
			PlayerPrefs.SetString("gamePlayedOnGui", "false");
			EditorApplication.OpenScene(LevelData.Levels[SelectLevel].LevelProperties[LevelData.Levels[SelectLevel].LoadLevelID]);
		}
	}

	public static void OnSceneView(SceneView scene)
	{
		if (!panelIsShowing)
		{
			Handles.BeginGUI();
			GUI.contentColor = Color.white;
			GUI.backgroundColor = Color.green;
			if (GUI.Button(new Rect(4, 4, 50, 50), (Texture)AssetDatabase.LoadAssetAtPath("Assets/[Game] - Rename This/Graphics/2DGraphics/Sprites/Scripts/tools.png", typeof(Texture)), EditorStyles.miniButton))
				ShowHidePanel();

			Handles.EndGUI();
			return;
		}
		Handles.BeginGUI();

		GUI.Label(new Rect(20, 20, 250, 50), "HyperBox Debug Window", EditorStyles.toolbarButton);

		GUI.contentColor = Color.white;
		GUILayout.BeginArea(new Rect(20, 43, 250, 750));

		var rect = EditorGUILayout.BeginVertical();
		GUI.color = Color.yellow;
		GUI.Box(rect, new GUIContent("Box"));

		GUI.color = Color.white;
		GUILayout.BeginHorizontal();

		GUI.backgroundColor = Color.green;
		GUI.contentColor = Color.white;
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Show/Hide Panel"))
		{
			ShowHidePanel();
		}
		GUILayout.FlexibleSpace();
		if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
		{
			wasMouseRight = true;
		}
		else if (Event.current.type == EventType.MouseDown && Event.current.button == 1)
		{
			wasMouseRight = false;
		}

		GUILayout.EndHorizontal();

		if (panelIsShowing)
		{
			DrawLevelArea();

			if (Application.isPlaying)
			{
				DrawTimeArea();
				DrawInGameFuncTions();
			}
		}

		EditorGUILayout.EndVertical();
		GUILayout.EndArea();

		Handles.EndGUI();
	}

	private static void ShowHidePanel()
	{
		panelIsShowing = !panelIsShowing;
	}

	#region Level Area
	private static void DrawLevelArea()
	{
		EditorGUILayout.BeginVertical();
		GUILayout.BeginHorizontal();

		GUILayout.FlexibleSpace();


		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUI.backgroundColor = Color.yellow;

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		GUI.backgroundColor = Color.cyan;
		GUI.contentColor = Color.white;

		int lastSelectLevel = SelectLevel;

		if (isLevelAreaShowing)
			SelectLevel = GUILayout.SelectionGrid(SelectLevel, _options.ToArray(), 5);

		if (SelectLevel != lastSelectLevel)
		{
			EditorApplication.OpenScene(LevelData.Levels[SelectLevel].LevelProperties[LevelData.Levels[SelectLevel].LoadLevelID]);
		}

		if (!Application.isPlaying && gameStopped)
		{
			gameStopped = false;
			EditorApplication.OpenScene(LevelData.Levels[SelectLevel].LevelProperties[LevelData.Levels[SelectLevel].LoadLevelID]);
		}

		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		GUI.backgroundColor = Color.green;
		GUI.contentColor = Color.white;
		if (GUILayout.Button("Play"))
		{
			PlayScene(SelectLevel);
		}
		if (GUILayout.Button("ShowLevels"))
		{
			ShowLevels();
		}

		GUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
	}
	#endregion
	[MenuItem("My Commands/First Command _o")]
	static void StopCommand()
	{
		if (EditorApplication.isPlaying)
		{
			EditorApplication.isPlaying = false;

			gameStopped = true;
		}
	}
	[MenuItem("My Commands/Second Command _i")]
	public static void PlayCommand()
	{
		wasMouseRight = true;
		PlayScene(SelectLevel);
	}

	#region Time area
	private static void DrawTimeArea()
	{


		var rect2 = EditorGUILayout.BeginVertical();
		GUI.color = Color.yellow;
		GUI.Box(rect2, GUIContent.none);

		GUI.color = Color.white;
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Time Settings");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();


		GUILayout.BeginHorizontal();
		GUILayout.Space(50);

		timeSliderValue = GUILayout.HorizontalSlider(timeSliderValue, 0.05f, 4.0f);

		GUILayout.Space(75);

		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();

		var defaultAlignment = GUI.skin.label.alignment;
		GUILayout.Label("0.05");
		GUI.skin.label.alignment = TextAnchor.UpperRight;
		GUILayout.FlexibleSpace();

		GUILayout.TextField(timeSliderValue.ToString("0.00"));

		GUILayout.Label("3");
		GUI.skin.label.alignment = defaultAlignment;


		GUILayout.EndHorizontal();


		GUILayout.BeginHorizontal();
		GUI.backgroundColor = Color.cyan;

		int lastSelectedTime = currentTimeScale;
		currentTimeScale = GUILayout.SelectionGrid(currentTimeScale, timeScaleOption, 5);
		if (lastSelectedTime != currentTimeScale)
		{
			SetSpeed(float.Parse(timeScaleOption[currentTimeScale], CultureInfo.InvariantCulture.NumberFormat));
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		GUI.backgroundColor = Color.green;

		if (GUILayout.Button("Set Speed"))
		{
			SetSpeed(timeSliderValue);
		}



		GUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();

	}
	#endregion

	#region InGameFunctions
	private static void DrawInGameFuncTions()
	{
		var rect2 = EditorGUILayout.BeginVertical();
		GUI.color = Color.yellow;
		GUI.Box(rect2, GUIContent.none);

		GUI.color = Color.white;

		GUILayout.BeginHorizontal();

		GUILayout.FlexibleSpace();
		GUILayout.Label("Game Functions");

		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("Previous"))
		{
			LevelManager.Instance.LoadPreviousLevel();
		}
		if (GUILayout.Button("Restart"))
		{
			LevelManager.Instance.LoadLastLevel();
		}
		if (GUILayout.Button("Next Scene"))
		{
			LevelManager.Instance.LoadNextLevel();
		}
		GUILayout.EndHorizontal();

		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		GUI.backgroundColor = Color.cyan;
		if (GUILayout.Button("Fail"))
		{
			GameManager.Instance.OnStageFail.Invoke();
		}
		if (GUILayout.Button("Success"))
		{
			GameManager.Instance.OnStageSuccess.Invoke();
		}
		GUILayout.EndHorizontal();

		EditorGUILayout.EndVertical();
	}

	#endregion
	private static LevelData LevelData
	{
		get
		{
			var levelDatas = AssetDatabase.FindAssets("t:LevelData");
			return AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelDatas[0]), typeof(LevelData)) as LevelData;
		}
	}


	public static void PlayScene(int level)
	{
		PlayerPrefs.SetInt(PlayerPrefKeys.LastLevel, 0);

		if (!EditorApplication.isPlaying)
		{
			if (wasMouseRight)
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
					EditorApplication.OpenScene(EditorBuildSettings.scenes[0].path);
				PlayerPrefs.SetInt(PlayerPrefKeys.LastLevel, SelectLevel);

				UnityEditor.EditorApplication.isPlaying = true;
				PlayerPrefs.SetString("gamePlayedOnGui", "true");
			}
			else
			{
				if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
					EditorApplication.OpenScene(LevelData.Levels[level].LevelProperties[LevelData.Levels[level].LoadLevelID]);
			}
		}
		else
		{
			PlayerPrefs.SetInt(PlayerPrefKeys.LastLevel, SelectLevel);
			SceneManager.LoadScene(0);
		}
	}

	private static void SetSpeed(float time)
	{
		Time.timeScale = time;
	}

	private static void ShowLevels()
	{
		isLevelAreaShowing = !isLevelAreaShowing;
	}
}