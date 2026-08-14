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
    public DishBase DishBase => dishBase;

    public void Setup(string id, DishBase dish, string itemName, int amount)
    {
        itemId = id;
        dishBase = dish;

        nameText.text = itemName;
        amountText.text = amount.ToString();

        if (dishBase != null && dishBase.spriteRenderer != null)
        {
            iconImage.sprite = dishBase.spriteRenderer.sprite;
            iconImage.enabled = iconImage.sprite != null;
        }
        else
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
        }
    }

    public string GetItemId()
    {
        return itemId;
    }

    public Sprite GetItemSprite()
    {
        if (dishBase == null)
            return null;

        if (dishBase.spriteRenderer == null)
            return null;

        return dishBase.spriteRenderer.sprite;
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