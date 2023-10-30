using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinTextController : MonoBehaviour
{
    TextMeshProUGUI coinText;
    TextMeshProUGUI CoinText { get { return (coinText == null) ? coinText = GetComponent<TextMeshProUGUI>() : coinText; } }


    private void Start()
    {
        PlayerData playerData = GameManager.Instance.PlayerData;
        CoinText.SetText(playerData.CurrencyData[ExchangeType.Coin].ToString());
    }

    private void OnEnable()
    {
        EventManager.OnPlayerDataChange.AddListener(UpdateText);
    }

    private void OnDisable()
    {
        EventManager.OnPlayerDataChange.AddListener(UpdateText);
    }

    private void UpdateText()
    {
        CoinText.SetText(GameManager.Instance.PlayerData.CurrencyData[ExchangeType.Coin].ToString());
    }
}
