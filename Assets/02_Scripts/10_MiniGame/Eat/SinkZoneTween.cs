using UnityEngine;
using DG.Tweening;

public class SinkZoneTween : MonoBehaviour
{
    [Header("Scale")]
    [SerializeField] private float targetScale = 1.15f;
    [SerializeField] private float duration = 0.15f;

    private Vector3 originalScale;
    private Tween scaleTween;
    private bool isHighlighted;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void SetHighlight(bool value)
    {
        if (isHighlighted == value)
            return;

        isHighlighted = value;

        scaleTween?.Kill();

        Vector3 target = value
            ? originalScale * targetScale
            : originalScale;

        scaleTween = transform
            .DOScale(target, duration)
            .SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
    }
}