using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;
    public int coins;

    private void Awake() {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start() {
        reset();
    }

    private void reset()
    {
        coins = 0;
    }

    public void addCoins(int amount = 1)
    {
        coins += amount;
    }
}
