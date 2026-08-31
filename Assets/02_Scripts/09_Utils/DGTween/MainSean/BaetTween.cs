using UnityEngine;
using DG.Tweening;

public class BeatTween : MonoBehaviour
{
    [Header("Heartbeat Settings")]
    [SerializeField] private float targetScale = 1.05f;
    [SerializeField] private float duration = 1f;

    private Tween heartbeatTween;

    private void Start()
    {
        heartbeatTween = transform
            .DOScale(targetScale, duration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }

    private void OnDestroy()
    {
        heartbeatTween?.Kill();
    }
}