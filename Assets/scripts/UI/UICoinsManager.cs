using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class UICoinsManager : MonoBehaviour
{
    public static UICoinsManager Instance;

    [Header("Currency")]
    public int coins;
    private int coinBuffer = 0;
    private bool isProcessingCoins = false;

    [Header("UI Elements")]
    public TMP_Text coinText;
    public List<GameObject> buttons;

    [Header("Animation Config")]
    public UIAnimationConfig animationConfig;

    private Color originalCoinTextColor;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        HideAllButtons();
    }

    private void Start()
    {
        if (coinText != null)
            originalCoinTextColor = coinText.color;

        ShowButtons();
        ResetCoins();
    }

    public void AddCoins(int amount = 1)
    {
        coinBuffer += amount;

        if (!isProcessingCoins)
            StartCoroutine(ProcessCoinQueue());
    }

    private IEnumerator ProcessCoinQueue()
    {
        isProcessingCoins = true;

        while (coinBuffer > 0)
        {
            coins++;
            coinBuffer--;

            UpdateCoinUI();
            PlayCoinTextFlash();

            yield return new WaitForSeconds(animationConfig.coinAddDelay);
        }

        isProcessingCoins = false;
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = $"x{coins}";
    }

    private void PlayCoinTextFlash()
    {
        if (coinText == null || animationConfig == null) return;

        coinText.DOComplete(true);

        coinText.DOColor(animationConfig.flashColor, animationConfig.flashDuration / 2)
            .SetLoops(2, LoopType.Yoyo);

        coinText.rectTransform.DOPunchScale(
            animationConfig.punchScale,
            animationConfig.flashDuration,
            animationConfig.punchVibrato,
            animationConfig.punchElasticity
        );
    }

    public void ResetCoins()
    {
        coins = 0;
        coinBuffer = 0;
        isProcessingCoins = false;
        UpdateCoinUI();
    }

    private void HideAllButtons()
    {
        foreach (var b in buttons)
        {
            b.transform.localScale = Vector3.zero;
            b.SetActive(false);
        }
    }

    private void ShowButtons()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            var b = buttons[i];
            b.SetActive(true);
            b.transform.DOScale(1, animationConfig.duration)
                .SetDelay(i * animationConfig.delay)
                .SetEase(animationConfig.ease);
        }
    }

    public void PlayCoinCollectedEffect(Transform coinTransform)
    {
        if (animationConfig == null || coinTransform == null) return;

        coinTransform.DOKill();
        coinTransform.localScale = Vector3.one;

        coinTransform.DOScale(animationConfig.collectScaleUp, animationConfig.collectScaleUpDuration)
            .SetEase(animationConfig.collectEaseUp)
            .OnComplete(() =>
            {
                coinTransform.DOScale(0f, animationConfig.collectScaleDownDuration)
                    .SetEase(animationConfig.collectEaseDown)
                    .OnComplete(() =>
                    {
                        coinTransform.gameObject.SetActive(false);
                    });
            });
    }
}
