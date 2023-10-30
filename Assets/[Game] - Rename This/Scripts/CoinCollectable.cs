using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CoinCollectable : CollectableBase
{

    private void Start()
    {
        transform.DORotate(Vector3.up * 360, 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental);
    }

    public override void Collect(Collector collector)
    {
        base.Collect(collector);

        PlayerData playerData = GameManager.Instance.PlayerData;

        playerData.CurrencyData[ExchangeType.Coin]++;
        EventManager.OnPlayerDataChange.Invoke();
        Destroy(gameObject);
    }
}
