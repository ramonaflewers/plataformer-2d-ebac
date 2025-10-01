using UnityEngine;

public class ItemCollectableCoin : collectableBase
{
    protected override void onCollect()
    {
        base.onCollect();
        ItemManager.Instance.addCoins();
    }
}
