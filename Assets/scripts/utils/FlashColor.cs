using UnityEngine;
using System;
using System.Collections.Generic;
using DG.Tweening;

public class FlashColor : MonoBehaviour
{
    public event Action OnFlashComplete;
    public List<SpriteRenderer> spriteRenderers;
    public Color color = Color.red;
    public float duration = 3f;

    private List<Tween> _currentTweens = new List<Tween>();

    private void OnValidate()
    {
        spriteRenderers = new List<SpriteRenderer>();

        foreach (var child in transform.GetComponentsInChildren<SpriteRenderer>())
        {
            spriteRenderers.Add(child);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Flash();
        }
    }

    public void Flash()
    {
        foreach (var t in _currentTweens)
        {
            if (t.IsActive()) t.Kill();
        }

        _currentTweens.Clear();

        Sequence sequence = DOTween.Sequence();

        foreach (var sr in spriteRenderers)
        {
            sr.color = Color.white;
            Tween tween = sr.DOColor(color, duration).SetLoops(2, LoopType.Yoyo);
            _currentTweens.Add(tween);
            sequence.Join(tween);
        }

        sequence.OnComplete(() => OnFlashComplete?.Invoke());
        sequence.Play();
    }
}
