using System;
using UnityEngine;
using UnityEngine.UI;

public class FoodArea : MonoBehaviour
{
    [SerializeField] private FoodPlace[] _foodPlaces;
    [SerializeField] private Button _startButton;

    public FoodPlace[] FoodPlaces => _foodPlaces;

    private void Awake()
    {
        CheckFoodPlaces();
    }

    public void CheckFoodPlaces()
    {
        if (_startButton == null)
            return;

        if (LiveManager.Instance != null &&
            LiveManager.Instance.IsLive)
        {
            return;
        }

        if (_foodPlaces == null || _foodPlaces.Length == 0)
        {
            _startButton.interactable = false;
            return;
        }

        foreach (FoodPlace foodPlace in _foodPlaces)
        {
            if (foodPlace == null || !foodPlace.IsFilled)
            {
                _startButton.interactable = false;
                return;
            }
        }

        _startButton.interactable = true;
    }
    public void ReturnAllFoodToInventory()
    {
        if (_foodPlaces == null)
            return;

        if (InventoryManager.Instance == null)
        {
            return;
        }

        foreach (FoodPlace foodPlace in _foodPlaces)
        {
            if (foodPlace == null)
                continue;

            if (!foodPlace.IsFilled)
                continue;

            string itemId = foodPlace.ItemId;

            if (string.IsNullOrWhiteSpace(itemId))
                continue;

            if (!TryGetItemType(itemId, out ItemType itemType))
            {
                continue;
            }

            bool returned =
                InventoryManager.Instance.AddItem(
                    itemId,
                    1,
                    itemType
                );

            if (returned)
            {
                foodPlace.RemoveFood();
            }
        }

        CheckFoodPlaces();
    }

    private bool TryGetItemType(
        string itemId,
        out ItemType itemType)
    {
        itemType = default;

        if (string.IsNullOrWhiteSpace(itemId))
            return false;

        if (itemId.StartsWith("IG_", StringComparison.Ordinal))
        {
            itemType = ItemType.Ingredient;
            return true;
        }

        if (itemId.StartsWith("SD_", StringComparison.Ordinal))
        {
            itemType = ItemType.SpecialDish;
            return true;
        }

        if (itemId.StartsWith("DS_", StringComparison.Ordinal) ||
            itemId.StartsWith("BD_", StringComparison.Ordinal) ||
            itemId.StartsWith("TD_", StringComparison.Ordinal))
        {
            itemType = ItemType.Dish;
            return true;
        }

        return false;
    }
}