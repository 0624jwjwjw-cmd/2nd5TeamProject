using UnityEngine;
using UnityEngine.UI;
public class CoinCatcher : MonoBehaviour
{
    [Header("Manager")]
    [SerializeField] private MiniGameManager gameManager;

    [Header("Visual")]
    [SerializeField] private Image playerImage;
    [SerializeField] private Sprite idleSprite;
    [SerializeField] private Sprite leftSprite;
    [SerializeField] private Sprite rightSprite;

    [Header("Idle Setting")]
    [SerializeField] private float idleTime = 1f;

    private RectTransform rectTransform;
    private RectTransform parentRect;
    private Vector2 startPosition;

    private float idleTimer;
    private float previousX;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        parentRect = rectTransform.parent.GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;

        previousX = rectTransform.anchoredPosition.x;

        SetIdleSprite();
    }

    private void Update()
    {
        if (!gameManager.IsMiniGamePlaying)
        {
            rectTransform.anchoredPosition = startPosition;
            idleTimer = 0f;
            SetIdleSprite();
            return;
        }

        if (InputManager.Instance == null)
            return;

        if (!InputManager.Instance.IsDragging)
        {
            CheckIdle();
            return;
        }

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

        // 이동 방향 확인
        float currentX = position.x;

        if (currentX > previousX)
        {
            SetRightSprite();
            idleTimer = 0f;
        }
        else if (currentX < previousX)
        {
            SetLeftSprite();
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleTime)
            {
                SetIdleSprite();
            }
        }

        previousX = currentX;
    }

    private void CheckIdle()
    {
        idleTimer += Time.deltaTime;

        if (idleTimer >= idleTime)
        {
            SetIdleSprite();
        }
    }

    private void SetIdleSprite()
    {
        playerImage.sprite = idleSprite;
    }

    private void SetLeftSprite()
    {
        playerImage.sprite = leftSprite;
    }

    private void SetRightSprite()
    {
        playerImage.sprite = rightSprite;
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