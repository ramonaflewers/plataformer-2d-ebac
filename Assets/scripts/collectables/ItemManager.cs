using UnityEngine;
using TMPro;
using DG.Tweening;
using System.Collections;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [Header("Currency")]
    public int coins;
    private int coinBuffer = 0;
    private bool isProcessingCoins = false;

    [Header("UI")]
    public TMP_Text coinText;
    public float coinAddDelay = 0.1f;
    public float flashDuration = 0.2f;
    public Color flashColor = Color.yellow;

    private Color originalColor;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (coinText != null)
        {
            originalColor = coinText.color;
        }

        reset();
    }

    private void reset()
    {
        coins = 0;
        coinBuffer = 0;
        isProcessingCoins = false;
        updateCoinUI();
    }

    public void addCoins(int amount = 1)
    {
        coinBuffer += amount;

        if (!isProcessingCoins)
        {
            StartCoroutine(ProcessCoinQueue());
        }
    }

    private IEnumerator ProcessCoinQueue()
    {
        isProcessingCoins = true;

        while (coinBuffer > 0)
        {
            coins++;
            coinBuffer--;
            updateCoinUI();
            PlayFlashEffect();

            yield return new WaitForSeconds(coinAddDelay);
        }

        isProcessingCoins = false;
    }

    private void updateCoinUI()
    {
        if (coinText != null)
        {
            coinText.text = $"x{coins}";
        }
    }

    private void PlayFlashEffect()
    {
        if (coinText == null) return;

        // Cancel any ongoing tweens
        coinText.DOComplete(true);

        // Flash: color from original to flashColor and back
        coinText.DOColor(flashColor, flashDuration / 2).SetLoops(2, LoopType.Yoyo);
        // Optional scale punch
        coinText.rectTransform.DOPunchScale(Vector3.one * 0.2f, flashDuration, 5, 1);
    }
}
