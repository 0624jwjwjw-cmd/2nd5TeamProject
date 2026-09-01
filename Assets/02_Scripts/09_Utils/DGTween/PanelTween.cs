using DG.Tweening;
using UnityEngine;

public class PanelTween : MonoBehaviour
{
    [Header("Tween Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private float overshootScale = 1.3f;

    private RectTransform rectTransform;
    private Sequence sequence;
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        Open();
    }

    public void Open()
    {
        rectTransform.DOKill();

        rectTransform.localScale = Vector3.zero;

        sequence?.Kill();

        sequence = DOTween.Sequence();

        sequence.Append(
            rectTransform.DOScale(overshootScale, duration * 0.7f)
        );

        sequence.Append(
            rectTransform.DOScale(1f, duration * 0.3f)
        );

        sequence.SetEase(Ease.OutBack);
    }
    private void OnDisable()
    {
        sequence?.Kill();
    }

    private void OnDestroy()
    {
        sequence?.Kill();
    }
}