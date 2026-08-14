using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodDrag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private LiveInventorySlotUI slotUI;

    private Canvas canvas;
    private RectTransform dragIcon;
    private Image dragImage;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();

        if (slotUI == null)
            slotUI = GetComponent<LiveInventorySlotUI>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (slotUI == null)
            return;

        if (string.IsNullOrEmpty(slotUI.ItemId))
            return;

        Sprite foodSprite = slotUI.GetItemSprite();

        if (foodSprite == null)
        {
            Debug.LogWarning(
                $"[FoodDrag] 음식 Sprite를 찾을 수 없습니다. ID: {slotUI.ItemId}"
            );
            return;
        }

        GameObject obj = new GameObject("DragIcon");

        dragIcon = obj.AddComponent<RectTransform>();
        dragIcon.SetParent(canvas.transform, false);

        dragImage = obj.AddComponent<Image>();
        dragImage.sprite = foodSprite;
        dragImage.preserveAspect = true;
        dragImage.raycastTarget = false;

        dragIcon.sizeDelta = new Vector2(160f, 160f);
        dragIcon.position = eventData.position;

        CanvasGroup group = obj.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
            return;

        dragIcon.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
            return;

        GameObject targetObject = eventData.pointerEnter;

        if (targetObject != null)
        {
            FoodPlace foodPlace =
                targetObject.GetComponentInParent<FoodPlace>();

            if (foodPlace != null)
            {
                foodPlace.TryPlaceFromInventory(slotUI);
            }
        }

        Destroy(dragIcon.gameObject);

        dragIcon = null;
        dragImage = null;
    }
}