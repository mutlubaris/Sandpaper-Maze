using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class PlayerData 
{

    public PlayerData()
    {
        UnlockedSkins = new Dictionary<string, SkinType>();
        UnlockedSkins["None"] = SkinType.FullBody;
        CurrencyData = new Dictionary<ExchangeType, int>();
        CurrencyData[ExchangeType.Coin] = 0;
        RateUsData = new RateUsData();
        CurrentSkin = "None";
    }

    [BoxGroup("Skin Data")]
    [ValueDropdown("GetSkins")]
    public string CurrentSkin;


    [BoxGroup("Skin Data")]
    [ValueDropdown("GetSkins")]
    public string CurrentLoadingSkin;


    [BoxGroup("Skin Data")]
    [ShowInInspector]
    public int CurrentLoadingSkinTier;


    private Dictionary<string, SkinType> unlockedSkins = new Dictionary<string, SkinType>();
    [BoxGroup("Skin Data")]
    [OnValueChanged("NotifyChange")]
    [ShowInInspector]
    public Dictionary<string, SkinType> UnlockedSkins
    {
        get
        {
            return unlockedSkins;
        }
        set
        {
            unlockedSkins = value;

        }
    }

    private Dictionary<ExchangeType, int> currencyData = new Dictionary<ExchangeType, int>();
    [BoxGroup("Currency Data")]
    [ShowInInspector]
    [OnValueChanged("NotifyChange")]
    public Dictionary<ExchangeType, int> CurrencyData
    {
        get
        {
            return currencyData;
        }
        set
        {
            currencyData = value;
        }
    }

    public RateUsData RateUsData;


#if UNITY_EDITOR
    private static List<string> GetSkins()
    {
        string[] guid = UnityEditor.AssetDatabase.FindAssets("t:SkinData");
        SkinData skinData = UnityEditor.AssetDatabase.LoadAssetAtPath(UnityEditor.AssetDatabase.GUIDToAssetPath(guid[0]), (typeof(SkinData))) as SkinData;
        List<string> keys = new List<string>();
        foreach (var item in skinData.Skins)
        {
            if (!keys.Contains(item.SkinID))
                keys.Add(item.SkinID);
        }
        return keys;
    }
#endif

    private void NotifyChange()
    {
        EventManager.OnPlayerDataChange.Invoke();
    }
}
