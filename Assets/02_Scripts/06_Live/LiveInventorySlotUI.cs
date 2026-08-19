using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LiveInventorySlotUI : MonoBehaviour, IPointerClickHandler
{
<<<<<<< HEAD
    [SerializeField] private Image _icon;
    [SerializeField] private int _amount;
    [SerializeField] private string _itemId;

    public string ItemId => _itemId;
    public int Amount => _amount;

    public void Setup(
        string itemId,
        Sprite icon,
        string itemName,
        int amount)
    {
        _itemId = itemId;
        _amount = amount;

        if (_icon != null)
            _icon.sprite = icon;
=======
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
>>>>>>> parent of 918d069 (/fix 음식 시스템 ItemId 기반으로 변경)
    }

    public void Clear()
    {
<<<<<<< HEAD
        _itemId = null;
        _amount = 0;

        if (_icon != null)
            _icon.sprite = null;
=======
        itemId = string.Empty;
        dishBase = null;

        iconImage.sprite = null;
        iconImage.enabled = false;

        nameText.text = string.Empty;
        amountText.text = string.Empty;
>>>>>>> parent of 918d069 (/fix 음식 시스템 ItemId 기반으로 변경)
    }

    public void OnPointerClick(PointerEventData eventData)
    {
<<<<<<< HEAD
        if (string.IsNullOrWhiteSpace(_itemId))
=======
        if (string.IsNullOrEmpty(itemId))
>>>>>>> parent of 918d069 (/fix 음식 시스템 ItemId 기반으로 변경)
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