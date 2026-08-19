using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodPlace : MonoBehaviour, IPointerClickHandler,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
<<<<<<< HEAD
    public bool IsFilled => !string.IsNullOrWhiteSpace(ItemId);
    public string ItemId { get; private set; }
=======
    [SerializeField] private Image foodImage;

    private string foodId = string.Empty;
    private DishBase dishBase;
>>>>>>> parent of 918d069 (/fix ÏùåÏãù ÏãúÏä§ÌÖú ItemId Í∏∞Î∞òÏúºÎ°ú Î≥ÄÍ≤Ω)

    private Canvas canvas;
    private RectTransform dragIcon;
    private Image dragImage;

    public string FoodId => foodId;
    public bool IsFilled => !string.IsNullOrEmpty(foodId);
    public bool IsOccupied => IsFilled;
    public DishBase DishBase => dishBase;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsOccupied)
            return;

        if (LiveManager.Instance != null &&
            LiveManager.Instance.IsLive)
            return;

        if (dishBase == null)
            return;

        SpriteRenderer renderer = dishBase.spriteRenderer;

        if (renderer == null)
            renderer = dishBase.GetComponent<SpriteRenderer>();

        if (renderer == null || renderer.sprite == null)
            return;

        GameObject obj = new GameObject("DragFood");

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
        if (dragIcon == null)
            return;

        dragIcon.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon == null)
            return;

        if (LiveManager.Instance != null &&
            LiveManager.Instance.IsLive)
        {
            DestroyDragIcon();
            return;
        }

        GameObject targetObject = eventData.pointerEnter;

        if (targetObject == null)
        {
            ReturnFoodToInventory();
        }
        else
        {
            FoodPlace targetPlace =
                targetObject.GetComponentInParent<FoodPlace>();

            if (targetPlace == null)
            {
                ReturnFoodToInventory();
            }
        }

        DestroyDragIcon();
    }

    public bool TryPlaceFromInventory(LiveInventorySlotUI slot)
    {
        if (slot == null || IsOccupied)
            return false;

<<<<<<< HEAD
        if (string.IsNullOrWhiteSpace(itemId))
            return false;

        ItemId = itemId;
=======
        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[FoodPlace] InventoryManager∞° æ¯Ω¿¥œ¥Ÿ.");
            return false;
        }

        string itemId = slot.ItemId;
        DishBase sourceDish = slot.DishBase;

        if (string.IsNullOrEmpty(itemId) || sourceDish == null)
            return false;

        SpriteRenderer renderer = sourceDish.spriteRenderer;

        if (renderer == null)
            renderer = sourceDish.GetComponent<SpriteRenderer>();

        if (renderer == null || renderer.sprite == null)
        {
            Debug.LogWarning(
                $"[FoodPlace] ¿ΩΩƒ Sprite∞° æ¯Ω¿¥œ¥Ÿ. ID: {itemId}"
            );
            return false;
        }

        if (!InventoryManager.Instance.HasItem(itemId, 1))
            return false;

        if (!InventoryManager.Instance.RemoveItem(itemId, 1))
            return false;

        foodId = itemId;
        dishBase = sourceDish;

        if (foodImage != null)
        {
            foodImage.sprite = renderer.sprite;
            foodImage.enabled = true;
        }

        FoodArea foodArea = FindFirstObjectByType<FoodArea>();

        if (foodArea != null)
            foodArea.CheckFoodPlaces();
>>>>>>> parent of 918d069 (/fix ÏùåÏãù ÏãúÏä§ÌÖú ItemId Í∏∞Î∞òÏúºÎ°ú Î≥ÄÍ≤Ω)

        return true;
    }

    // πÊº€ ¡ﬂ ¿ΩΩƒ º∑√Î Ω√ ªÁøÎ
    // ¿Œ∫•≈‰∏Æ∑Œ µπ∑¡∫∏≥ª¡ˆ æ ∞Ì øœ¿¸»˜ ¡¶∞≈
    public void RemoveFood()
    {
        if (!IsOccupied)
            return;

        Clear();

        FoodArea foodArea = FindFirstObjectByType<FoodArea>();

        if (foodArea != null)
            foodArea.CheckFoodPlaces();
    }

    // πÊº€ Ω√¿€ ¿¸ ¡¢Ω√ø°º≠ ¿ΩΩƒ¿ª ª©≥æ ∂ß∏∏ ªÁøÎ
    private void ReturnFoodToInventory()
    {
        if (!IsOccupied)
            return;

        string removedFoodId = foodId;

        if (InventoryManager.Instance != null &&
            !string.IsNullOrEmpty(removedFoodId))
        {
            InventoryManager.Instance.AddItem(removedFoodId, 1);
        }

        Clear();

        FoodArea foodArea = FindFirstObjectByType<FoodArea>();

        if (foodArea != null)
            foodArea.CheckFoodPlaces();
    }

    private void DestroyDragIcon()
    {
        if (dragIcon != null)
            Destroy(dragIcon.gameObject);

        dragIcon = null;
        dragImage = null;
    }

    private void Clear()
    {
        foodId = string.Empty;
        dishBase = null;

        if (foodImage != null)
        {
            foodImage.sprite = null;
            foodImage.enabled = false;
        }
    }
}
