using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class LiveCharacterTween : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private Image characterImage;
    [SerializeField] private CharacterSelectPanel characterSelectPanel;
    [SerializeField] private Sprite[] eatSprites;

    [Header("Rotation")]
    [SerializeField] private float rotateAngle = 10f;
    [SerializeField] private float rotateDuration = 0.5f;

    [Header("Scale")]
    [SerializeField] private float scaleAmount = 1.2f;
    [SerializeField] private float scaleDuration = 0.5f;

    private RectTransform rectTransform;

    private Vector3 originalScale;
    private Sprite originalSprite;
    private Vector3 originalRotation;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        originalScale = rectTransform.localScale;
        originalRotation = rectTransform.localEulerAngles;

        if (characterImage != null)
        {
            originalSprite = characterImage.sprite;
        }
    }

    public void PlayEatReaction()
    {
        // 기존 트윈 제거
        rectTransform.DOKill();

        // 원래 상태로 초기화
        rectTransform.localScale = originalScale;
        rectTransform.localEulerAngles = originalRotation;

       
        int currentIndex = characterSelectPanel.CurrentIndex;

      
        if (currentIndex >= 0 && currentIndex < eatSprites.Length)
        {
            originalSprite = characterImage.sprite;
            characterImage.sprite = eatSprites[currentIndex];
        }


        // 좌우 까딱
        Sequence sequence = DOTween.Sequence();

        sequence.Append(
            rectTransform
                .DOLocalRotate(
                    new Vector3(
                        originalRotation.x,
                        originalRotation.y,
                        originalRotation.z + rotateAngle
                    ),
                    rotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            rectTransform
                .DOLocalRotate(
                    new Vector3(
                        originalRotation.x,
                        originalRotation.y,
                        originalRotation.z - rotateAngle
                    ),
                    rotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        sequence.Append(
            rectTransform
                .DOLocalRotate(
                    originalRotation,
                    rotateDuration
                )
                .SetEase(Ease.InOutSine)
        );

        // 살짝 커졌다가 원래 크기로
        sequence.Join(
            rectTransform
                .DOScale(originalScale * scaleAmount, scaleDuration)
                .SetEase(Ease.OutQuad)
        );

        sequence.Append(
            rectTransform
                .DOScale(originalScale, scaleDuration)
                .SetEase(Ease.InQuad)
        );

        // 모든 연출이 끝나면 원래 이미지로 복구
        sequence.OnComplete(() =>
        {
            if (characterImage != null)
            {
                characterImage.sprite = originalSprite;
            }
        });
    }
}