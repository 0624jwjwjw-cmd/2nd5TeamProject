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
        if (slotUI == null || canvas == null)
            return;

        if (string.IsNullOrEmpty(slotUI.ItemId))
            return;

        DishBase dishBase = slotUI.DishBase;

        if (dishBase == null)
            return;

        SpriteRenderer renderer = dishBase.spriteRenderer;

        if (renderer == null)
            renderer = dishBase.GetComponent<SpriteRenderer>();

        if (renderer == null || renderer.sprite == null)
        {
            Debug.LogWarning(
                $"[FoodDrag] À½½Ä Sprite°¡ ¾ø½À´Ï´Ù. ID: {slotUI.ItemId}"
            );
            return;
        }

        GameObject obj = new GameObject("DragIcon");

        dragIcon = obj.AddComponent<RectTransform>();
        dragIcon.SetParent(canvas.transform, false);

        dragImage = obj.AddComponent<Image>();
        dragImage.sprite = renderer.sprite;
        dragImage.preserveAspect = true;
        dragImage.raycastTarget = false;

        dragIcon.sizeDelta = new Vector2(160f, 160f);
        dragIcon.position = eventData.position;

        CanvasGroup group = obj.AddComponent<CanvasGroup>();
        group.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
<<<<<<< HEAD
        if (string.IsNullOrWhiteSpace(itemId))
=======
        if (dragIcon == null)
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)
            return;

        dragIcon.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
            return;

<<<<<<< HEAD
    public bool TryGetItemId(out string itemId)
    {
        itemId = ItemId;
        return IsDragging && !string.IsNullOrWhiteSpace(ItemId);
=======
        GameObject targetObject = eventData.pointerEnter;

        if (targetObject != null)
        {
            FoodPlace foodPlace =
                targetObject.GetComponentInParent<FoodPlace>();

            if (foodPlace != null)
                foodPlace.TryPlaceFromInventory(slotUI);
        }

        Destroy(dragIcon.gameObject);

        dragIcon = null;
        dragImage = null;
>>>>>>> parent of 918d069 (/fix ìŒì‹ ì‹œìŠ¤í…œ ItemId ê¸°ë°˜ìœ¼ë¡œ ë³€ê²½)
    }
}