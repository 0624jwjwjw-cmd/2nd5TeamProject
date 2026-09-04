using System.Collections.Generic;

public class DishMatchingService
{
    public string FindMatchingDish(List<KitchenCookSlotItem> slots)
    {
        foreach (KeyValuePair<string, DishData> kvp in GameDataRepository.Instance.dishLookup)
        {
            if (CanCook(kvp.Value, slots))
            {
                return kvp.Key;
            }
        }
        return null;
    }

    public string FindMatchingSpecialDish(string normalDishID)
    {
        if (!GameDataRepository.Instance.TryGetDish(normalDishID, out DishData normalDish))
        {
            return null;
        }

        foreach (KeyValuePair<string, DishData> kvp in GameDataRepository.Instance.specialDishLookup)
        {
            if (HasSameMaterials(normalDish, kvp.Value))
            {
                return kvp.Key;
            }
        }
        return null;
    }

    private bool CanCook(DishData dish, List<KitchenCookSlotItem> slots)
    {
        if (dish.Materials.Length != slots.Count)
        {
            return false;
        }

        for (int i = 0; i < dish.Materials.Length; i++)
        {
            IngredientData material = dish.Materials[i].IngredientData;
            int requiredAmount = dish.Materials[i].Amount;

            KitchenCookSlotItem matchedSlot = FindSlotByID(slots, material.ID);
            if (matchedSlot == null || matchedSlot.count != requiredAmount)
            {
                return false;
            }
        }
        return true;
    }

    private KitchenCookSlotItem FindSlotByID(List<KitchenCookSlotItem> slots, string ingredientID)
    {
        foreach (KitchenCookSlotItem slot in slots)
        {
            if (slot.ingredientID == ingredientID)
            {
                return slot;
            }
        }
        return null;
    }

    private bool HasSameMaterials(DishData dishA, DishData dishB)
    {
        if (dishA.Materials.Length != dishB.Materials.Length)
        {
            return false;
        }

        for (int i = 0; i < dishA.Materials.Length; i++)
        {
            if (dishA.Materials[i].IngredientData.ID != dishB.Materials[i].IngredientData.ID || dishA.Materials[i].Amount != dishB.Materials[i].Amount)
            {
                return false;
            }
        }
        return true;
    }
}
