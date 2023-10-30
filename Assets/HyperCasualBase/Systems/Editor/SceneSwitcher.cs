using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityToolbarExtender.Examples
{
	static class ToolbarStyles
	{
		public static readonly GUIStyle commandButtonStyle;

		static ToolbarStyles()
		{
			commandButtonStyle = new GUIStyle("Command")
			{
				fontSize = 16,
				alignment = TextAnchor.MiddleCenter,
				imagePosition = ImagePosition.ImageLeft,
				fontStyle = FontStyle.Bold
			};
		}
	}

	[InitializeOnLoad]
	public class SceneSwitchLeftButton
	{
		static SceneSwitchLeftButton()
		{
			ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
			ToolbarExtender.RightToolbarGUI.Add(OnToolbarGUIRight);

		}

		static List<SceneAsset> scenes = new List<SceneAsset>();

		static void OnToolbarGUI()
		{


			GUILayout.BeginHorizontal();

			for (int i = 0; i < EditorBuildSettings.scenes.Length; i++)
			{

				var scene = EditorBuildSettings.scenes[i].path;
				if (scene.Contains("Level") && !scene.Contains("Test"))
					continue;

				if (scene.Contains("elephant"))
					continue;

				if (GUILayout.Button(new GUIContent(i.ToString()), ToolbarStyles.commandButtonStyle))
				{
					SceneHelper.OpenScene(EditorBuildSettings.scenes[i].path);
				}
			}

			GUILayout.EndHorizontal();

			if (Application.isPlaying && PlayerPrefs.GetString("gamePlayedOnGui") == "true")
			{
				if (GUILayout.Button(new GUIContent("◄", "Start Scene 1"), ToolbarStyles.commandButtonStyle))
				{
					LevelManager.Instance.LoadPreviousLevel();
				}
			}

		}
		static void OnToolbarGUIRight()
		{

			if (Application.isPlaying && PlayerPrefs.GetString("gamePlayedOnGui") == "true")
			{

				if (GUILayout.Button(new GUIContent("►", "Load Next Level"), ToolbarStyles.commandButtonStyle))
				{
					LevelManager.Instance.LoadNextLevel();
				}

				if (GUILayout.Button((Texture)AssetDatabase.LoadAssetAtPath("Assets/[Game] - Rename This/Graphics/2DGraphics/Sprites/Scripts/restart.png", typeof(Texture)), EditorStyles.miniButton))
				{
					LevelManager.Instance.LoadLastLevel();
				}
				GUILayout.FlexibleSpace();
			}
			
		}

	}

	static class SceneHelper
	{
		static string sceneToOpen;

		public static void OpenScene(string scene)
		{
			if (EditorApplication.isPlaying)
			{
				return;
			}
			if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
				EditorSceneManager.OpenScene(scene);
		}
	}
}