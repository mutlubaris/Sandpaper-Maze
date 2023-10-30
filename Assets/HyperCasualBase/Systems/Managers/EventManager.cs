using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Sirenix.OdinInspector;


public static class EventManager 
{
    public static UnityEvent OnCorrectAnswer = new UnityEvent();
    public static UnityEvent OnAskQuestion = new UnityEvent();

    public static UnityEvent OnPlayerDataChange = new UnityEvent();


    #region Editor
    public static UnityEvent OnLevelDataChange = new UnityEvent();
    #endregion
}

public class PlayerDataEvent : UnityEvent<PlayerData> { }
