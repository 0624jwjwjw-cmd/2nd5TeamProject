using UnityEngine;
using DG.Tweening;

public class ResultTextTween : MonoBehaviour
{
    [Header("Tween")]
    [SerializeField] private float startScale = 0f;
    [SerializeField] private float overshootScale = 1.2f;
    [SerializeField] private float duration = 0.3f;

    private Vector3 originalScale;
    private Tween scaleTween;

    private void Awake()
    {
        originalScale = transform.localScale;
    }

    public void Play()
    {
        scaleTween?.Kill();

        transform.localScale = originalScale * startScale;

        scaleTween = transform
            .DOScale(originalScale * overshootScale, duration * 0.7f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                scaleTween = transform
                    .DOScale(originalScale, duration * 0.3f)
                    .SetEase(Ease.InOutQuad);
            });
    }

    public void ResetScale()
    {
        scaleTween?.Kill();
        transform.localScale = originalScale;
    }

    private void OnDestroy()
    {
        scaleTween?.Kill();
    }
}