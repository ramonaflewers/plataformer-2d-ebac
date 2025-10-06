using UnityEngine;
using DG.Tweening;

[CreateAssetMenu(fileName = "UIAnimationConfig", menuName = "Config/UIAnimationConfig")]
public class UIAnimationConfig : ScriptableObject
{
    [Header("Button Animation")]
    public float duration = 0.2f;
    public float delay = 0.05f;
    public Ease ease = Ease.OutBack;

    [Header("Coin Collect Animation")]
    public float collectScaleUp = 1.3f;
    public float collectScaleUpDuration = 0.2f;
    public Ease collectEaseUp = Ease.OutBack;
    public float collectScaleDownDuration = 0.2f;
    public Ease collectEaseDown = Ease.InBack;

    [Header("Coin UI Animation")]
    public float coinAddDelay = 0.1f;
    public float flashDuration = 0.2f;
    public Color flashColor = Color.yellow;
    public Vector3 punchScale = Vector3.one * 0.2f;
    public int punchVibrato = 5;
    public float punchElasticity = 1f;
}
