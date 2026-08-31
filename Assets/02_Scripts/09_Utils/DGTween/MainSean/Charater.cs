using UnityEngine;
using DG.Tweening;

public class CharacterTitleTween : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float duration = 0.3f;

    [Header("Scale")]
    [SerializeField] private float firstScale = 1.2f;
    [SerializeField] private float bounceMinScale = 0.9f;
    [SerializeField] private float bounceMaxScale = 1.1f;
    [SerializeField] private float finalScale = 1.2f;

    [Header("Bounce")]
    [SerializeField] private int bounceCount = 3;

    public Tween Play()
    {
        transform.DOKill();
        transform.localScale = Vector3.zero;
        Sequence sequence = DOTween.Sequence();

        // 0 ¡æ 1.2
        sequence.Append(transform.DOScale(firstScale, duration).SetEase(Ease.OutBack));
        sequence.Append(transform.DOScale(bounceMinScale, duration));
        // 0.9 ¡ê 1.1 ¹Ýº¹
        for (int i = 0; i < bounceCount; i++)
        {
            sequence.Append(transform.DOScale(bounceMaxScale, duration));
            sequence.Append(transform.DOScale(bounceMinScale, duration));
        }

        // 1.2 ¡æ 0
        sequence.Append(transform.DOScale(0f,duration).SetEase(Ease.InBack));

        return sequence;
    }
}