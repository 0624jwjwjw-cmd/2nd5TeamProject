using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FoodPlace : MonoBehaviour, IPointerClickHandler
{
    [Header("Food UI")]
    [SerializeField] private Image foodImage;

    private string _foodId = string.Empty;
    private DishBase _dishBase;

    public string FoodId => _foodId;
    public bool IsFilled => !string.IsNullOrEmpty(_foodId);
    public bool IsOccupied => IsFilled;
    public DishBase DishBase => _dishBase;

    private void Awake()
    {
        Clear();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (IsOccupied)
            return;
    }

    public bool TryPlaceFromInventory(LiveInventorySlotUI slot)
    {
        if (slot == null || IsOccupied)
            return false;

        DishBase dishBase = slot.GetDishBase();

        if (dishBase == null)
        {
            Debug.LogWarning("[FoodPlace] DishBase를 찾을 수 없습니다.");
            return false;
        }

        if (InventoryManager.Instance == null)
        {
            Debug.LogError("[FoodPlace] InventoryManager가 없습니다.");
            return false;
        }

        string itemId = slot.ItemId;

        if (string.IsNullOrEmpty(itemId))
            return false;

        if (!InventoryManager.Instance.HasItem(itemId, 1))
        {
            Debug.LogWarning($"[FoodPlace] 음식 수량이 없습니다: {itemId}");
            return false;
        }

        SpriteRenderer renderer = dishBase.spriteRenderer;

        if (renderer == null)
            renderer = dishBase.GetComponent<SpriteRenderer>();

        if (renderer == null || renderer.sprite == null)
        {
            Debug.LogWarning(
                $"[FoodPlace] DishBase SpriteRenderer를 찾을 수 없습니다: {itemId}"
            );
            return false;
        }

        if (!InventoryManager.Instance.RemoveItem(itemId, 1))
            return false;

        _foodId = itemId;
        _dishBase = dishBase;

        if (foodImage != null)
        {
            foodImage.sprite = renderer.sprite;
            foodImage.preserveAspect = true;
            foodImage.enabled = true;
        }

        FoodArea foodArea = FindFirstObjectByType<FoodArea>();

        if (foodArea != null)
            foodArea.CheckFoodPlaces();

        return true;
    }

    public void RemoveFood()
    {
        Clear();

        FoodArea foodArea = FindFirstObjectByType<FoodArea>();

        if (foodArea != null)
            foodArea.CheckFoodPlaces();
    }

    private void Clear()
    {
        _foodId = string.Empty;
        _dishBase = null;

        if (foodImage != null)
        {
            foodImage.sprite = null;
            foodImage.enabled = false;
        }
    }
}