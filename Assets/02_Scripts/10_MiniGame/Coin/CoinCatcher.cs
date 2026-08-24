using UnityEngine;

public class CoinCatcher : MonoBehaviour
{
    private RectTransform rectTransform;
    private RectTransform parentRect;
    private Vector2 startPosition;
    [SerializeField] private MiniGameManager gameManager;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = rectTransform.parent.GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;
    }

    private void Update()
    {
        if (!gameManager.IsMiniGamePlaying)
        {
            rectTransform.anchoredPosition = startPosition;
            return;
        }
        if (InputManager.Instance == null)
            return;

        if (!InputManager.Instance.IsDragging)
            return;

        Move();
    }

    private void Move()
    {
        Vector2 pointerPosition = InputManager.Instance.PointerPosition;

        Vector2 localPosition;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            pointerPosition,
            null,
            out localPosition
        );

        Vector2 position = rectTransform.anchoredPosition;

        position.x = localPosition.x;

        position.x = Mathf.Clamp(
            position.x,
            GetMinX(),
            GetMaxX()
        );

        rectTransform.anchoredPosition = position;
    }

    private float GetMinX()
    {
        float parentWidth = parentRect.rect.width;
        float catcherWidth = rectTransform.rect.width;

        return -(parentWidth - catcherWidth) / 2f;
    }

    private float GetMaxX()
    {
        float parentWidth = parentRect.rect.width;
        float catcherWidth = rectTransform.rect.width;

        return (parentWidth - catcherWidth) / 2f;
    }
}