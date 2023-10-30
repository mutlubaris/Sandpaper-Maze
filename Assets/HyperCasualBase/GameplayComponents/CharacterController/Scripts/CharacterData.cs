using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;


public class CharacterData : ScriptableObject
{
    public CharacterControlType CharacterControlType = CharacterControlType.None;

    [HideIf("isAI")]
    public CharacterPlayerType CharacterPlayerType = CharacterPlayerType.Platformer;

    [ShowIf("isAI")]
    public CharacterAIType CharacterAIType = CharacterAIType.Petrol;


    public CharacterHeathData CharacterHealthData = new CharacterHeathData();
    public CharacterMovementData CharacterMovementData = new CharacterMovementData();
    public CharacterWeaponData CharacterWeaponData = new CharacterWeaponData();


    private bool isAI { get { return CharacterControlType == CharacterControlType.AI; } }
}

[System.Serializable]
public class CharacterHeathData
{
    public CharacterHeathData()
    {
        MaxHealth = 10;
        InitialDamage = 1;
    }
    public int MaxHealth;
    public int InitialDamage;
}

[System.Serializable]
public class CharacterMovementData
{

    public float MoveSpeed = 7;
    //'Aircontrol' determines to what degree the player is able to move while in the air;
    [Range(0f, 1f)]
    public float airControl = 0.4f;

    //Jump speed;
    public float jumpSpeed = 10f;

    //Jump duration variables;
    public float jumpDuration = 0.2f;

    public float airFriction = 0.5f;

    public float groundFriction = 100f;

    //Amount of downward gravity;
    public float gravity = 30f;
    [Tooltip("How fast the character will slide down steep slopes.")]
    public float slideGravity = 5f;

    //Acceptable slope angle limit;
    public float slopeLimit = 80f;

    [Tooltip("Whether to calculate and apply momentum relative to the controller's transform.")]
    public bool useLocalMomentum = false;
}

[System.Serializable]
public class CharacterWeaponData
{
    [ValueDropdown("GetWeapons")]
    public string EqquiepedWeapon = "None";
    [Range(1, 2)]
    public float ShootRateDivider = 1f;


#if UNITY_EDITOR
    private static List<string> GetWeapons()
    {
        string[] guid = UnityEditor.AssetDatabase.FindAssets("t:WeaponData");
        List<WeaponData> weaponDatas = new List<WeaponData>();
        for (int i = 0; i < guid.Length; i++)
        {
            weaponDatas.Add(UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(guid[i]), typeof(WeaponData)) as WeaponData);
        }
        List<string> keys = new List<string>();
        keys.Add("None");
        foreach (var item in weaponDatas)
        {
            keys.Add(item.WeaponName);
        }
        return keys;
    }
#endif
}
