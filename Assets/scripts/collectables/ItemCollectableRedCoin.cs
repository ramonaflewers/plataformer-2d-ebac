using UnityEngine;

public class ItemCollectableRedCoin : collectableBase
{
    protected override void onCollect()
    {
        base.onCollect();
        UICoinsManager.Instance.AddCoins(5);
    }
}
