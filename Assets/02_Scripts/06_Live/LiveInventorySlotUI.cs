using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LiveInventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image iconImage;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text amountText;

    private string itemId;
    private DishBase dishBase;

    public string ItemId => itemId;

    public string GetItemId()
    {
        return itemId;
    }

    public DishBase GetDishBase()
    {
        return dishBase;
    }

    public Sprite GetItemSprite()
    {
        if (dishBase == null)
            return null;

        SpriteRenderer renderer = dishBase.spriteRenderer;

        if (renderer == null)
            renderer = dishBase.GetComponent<SpriteRenderer>();

        if (renderer == null)
            return null;

        return renderer.sprite;
    }

    public void Setup(string id, Sprite icon, string itemName, int amount)
    {
        itemId = id;

        iconImage.sprite = icon;
        iconImage.enabled = icon != null;

        nameText.text = itemName;
        amountText.text = amount.ToString();

        dishBase = null;

        if (string.IsNullOrEmpty(itemId))
            return;

        if (ItemVisualRepository.Instance == null)
            return;

        if (ItemVisualRepository.Instance.TryGetPrefab(
            itemId,
            out GameObject prefab))
        {
            if (prefab != null)
            {
                dishBase = prefab.GetComponent<DishBase>();
            }
        }
    }

    public void Clear()
    {
        itemId = string.Empty;
        dishBase = null;

        iconImage.sprite = null;
        iconImage.enabled = false;

        nameText.text = string.Empty;
        amountText.text = string.Empty;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (string.IsNullOrEmpty(itemId))
            return;

        FoodPlace targetPlace = FindEmptyFoodPlace();

        if (targetPlace == null)
            return;

        targetPlace.TryPlaceFromInventory(this);
    }

    private FoodPlace FindEmptyFoodPlace()
    {
        FoodArea foodArea = FindFirstObjectByType<FoodArea>();

        if (foodArea == null)
            return null;

        FoodPlace[] places = foodArea.FoodPlaces;

        if (places == null)
            return null;

        foreach (FoodPlace place in places)
        {
            if (place != null && !place.IsOccupied)
                return place;
        }

        return null;
    }
}