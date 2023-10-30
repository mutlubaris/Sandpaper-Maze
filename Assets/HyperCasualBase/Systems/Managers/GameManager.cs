using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;

[System.Serializable]
public class GameConfig
{
    public bool IsLooping
    {
        get
        {
            return PlayerPrefs.GetInt(PlayerPrefKeys.IsLooping, 0) != 0;
        }

        set
        {
            PlayerPrefs.SetInt(PlayerPrefKeys.IsLooping, 1);
        }
    }
}

public class GameManager : Singleton<GameManager>
{

    private PlayerData playerData;
    [ShowInInspector]
    public PlayerData PlayerData
    {
        get
        {
            if(playerData == null)
            {
                playerData = SaveLoadManager.LoadPDP<PlayerData>(SavedFileNameHolder.PlayerData, new PlayerData());
                if (playerData == null)
                    playerData = new PlayerData();
            }

            return playerData;
        }
    }
    public GameConfig GameConfig;

    [HideInInspector]
    public UnityEvent OnGameStart = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnGameEnd = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnStageSuccess = new UnityEvent();
    [HideInInspector]
    public UnityEvent OnStageFail = new UnityEvent();


    private bool isGameStarted;
    [ReadOnly]
    [ShowInInspector]
    public bool IsGameStarted { get { return isGameStarted; } set { isGameStarted = value; } }

    private bool isStageCompleted;
    [ReadOnly]
    [ShowInInspector]
    public bool IsStageCompleted { get { return isStageCompleted; } set { isStageCompleted = value; } }

    private void OnEnable()
    {
        SceneController.Instance.OnSceneLoaded.AddListener(() => IsStageCompleted = false);
    }

    private void OnDisable()
    {
        SceneController.Instance.OnSceneLoaded.RemoveListener(() => IsStageCompleted = false);
    }

    public void StartGame()
    {
        if (isGameStarted)
            return;

        isGameStarted = true;
        OnGameStart.Invoke();
    }

    public void EndGame()
    {
        if (!isGameStarted)
            return;
        isGameStarted = false;
        OnGameEnd.Invoke();
    }

    /// <summary>
    /// Call it when the player wins or loses the game
    /// </summary>
    /// <param name="value"></param>
    [Button]
    public void CompeleteStage(bool value)
    {
        if (!LevelManager.Instance.IsLevelStarted)
            return;

        if (IsStageCompleted == true)
            return;

        if (value)
            OnStageSuccess.Invoke();
        else OnStageFail.Invoke();

        IsStageCompleted = true;
    }

    #region GameConditions

    /// <summary>
    /// Win Game when player is the only one alive. Lose when player dies
    /// Hook this to CharacterManager.OnCharacterRemove.
    /// </summary>
    /// <param name="character"></param>
    private void CheckGameCharacterState(Character character)
    {
        if (character.CharacterData.CharacterControlType == CharacterControlType.None)
            return;

        if (character.CharacterData.CharacterControlType == CharacterControlType.Player)
        {
            if (CharacterManager.Instance.Characters.Count >= 1)
            {
                CharacterManager.Instance.OnPlacementChange.Invoke(CharacterManager.Instance.Characters.Count + 1);
                CompeleteStage(false);
                AnalitycsManager.Instance.LogEvent("Level_Event", "Level_Fail", CharacterManager.Instance.Characters.Count.ToString());
                Debug.Log("Player lose the geme");
            }
        }
        else
        {
            if (CharacterManager.Instance.Characters.Count <= 1)
            {
                CompeleteStage(true);
                Debug.Log("Player won the geme");
            }
        }
    }
    #endregion



    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            SaveLoadManager.SavePDP(playerData, SavedFileNameHolder.PlayerData);
        }
    }

    private void OnApplicationQuit()
    {
        SaveLoadManager.SavePDP(playerData, SavedFileNameHolder.PlayerData);
    }
}
