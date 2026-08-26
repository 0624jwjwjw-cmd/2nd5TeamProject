using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class KitchenCookingSystem : MonoBehaviour
{
    [SerializeField] private KitchenCookingSlotManager kitchenCookingSlotManager;

    [SerializeField] private int specialRate;

    [SerializeField] private KitchenCookResult cookResult;

    [SerializeField] private string burnedDishID = "BD_01";
    [SerializeField] private string trashDishID = "TD_01";

    public void StartCook()
    {
        string resultDishID = FindMatchingDish();

        if (resultDishID == null)
        {
            cookResult.gameObject.SetActive(true);
            cookResult.SetResultDishInfo(burnedDishID);
            InventoryManager.Instance.AddItem(burnedDishID, 1, ItemType.Dish);
        }
        else
        {
            if (!ReciepeUnlockManager.Instance.IsUnlocked(resultDishID))
            {
                cookResult.gameObject.SetActive(true);
                cookResult.SetResultDishInfo(trashDishID);
                InventoryManager.Instance.AddItem(trashDishID, 1, ItemType.Dish);
            }
            else
            {
                if (Random.Range(0, 100) < specialRate)
                {
                    //resultDishID를 가지고 매칭되는 specialDishID를 가져오는 로직\
                    //string specialDishID = 
                    cookResult.gameObject.SetActive(true);
                    //cookResult.SetResultSpecialDishInfo(specialDishID);
                    //InventoryManager.Instance.AddItem(specialDishID, 1, ItemType.Dish);
                }
                else
                {
                    cookResult.gameObject.SetActive(true);
                    cookResult.SetResultDishInfo(resultDishID);
                    InventoryManager.Instance.AddItem(resultDishID, 1, ItemType.Dish);
                }
            }
        }

        foreach (KitchenCookSlotItem slot in kitchenCookingSlotManager.slots)
        {
            InventoryManager.Instance.RemoveItem(slot.ingredientID, slot.count);
        }

        kitchenCookingSlotManager.ClearSlots();
        for (int i = 0; i < kitchenCookingSlotManager.solidSlotUIs.Length; i++)
        {
            kitchenCookingSlotManager.solidSlotUIs[i].Clear();
        }
    }
    private string FindMatchingDish()
    {
        foreach(KeyValuePair<string, DishData> kvp in GameDataRepository.Instance.dishLookup)
        {
            string id = kvp.Key;
            DishData dish = kvp.Value;

            if (CanCook(dish))
            {
                return id;
            }
        }
        return null;
    }
    private bool CanCook(DishData dish)
    {
        if (dish.Materials.Length != kitchenCookingSlotManager.slots.Count)
        {
            return false;
        }

        for (int i = 0; i < dish.Materials.Length; i++)
        {
            IngredientData material = dish.Materials[i].IngredientData;
            int requiredAmount = dish.Materials[i].Amount;

            KitchenCookSlotItem matchedSlot = FindSlotByID(material.ID);

            if (matchedSlot == null || matchedSlot.count != requiredAmount)
            {
                return false;
            }
        }
        return true;
    }
    private KitchenCookSlotItem FindSlotByID(string ingredientID)
    {
        foreach(KitchenCookSlotItem slot in kitchenCookingSlotManager.slots)
        {
            if(slot.ingredientID == ingredientID)
            {
                return slot;
            }
        }
        return null;
    }
}
