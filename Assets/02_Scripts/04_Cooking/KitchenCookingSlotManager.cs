using System.Collections.Generic;
using UnityEngine;

public class KitchenCookSlotItem
{
    public string ingredientID;
    public int count;

    public KitchenCookSlotItem(string ingredientID, int count)
    {
        this.ingredientID = ingredientID;
        this.count = count;
    }
}

public class KitchenCookingSlotManager : MonoBehaviour
{
    [SerializeField] private int maxSlot = 3;
    [SerializeField] private KitchenCookingSlotUIBinder uiBinder;
    [SerializeField] public List<KitchenCookSlotItem> slots = new List<KitchenCookSlotItem>();

    [SerializeField] private string burnedDishID = "BD_01";
    [SerializeField] private string trashDishID = "TD_01";
    public void AddIngredient(string ingredientID)
    {
        if(ingredientID == burnedDishID || ingredientID == trashDishID)
        {
            return;
        }
        int amount = 0;

        foreach(InventorySlotData slotData in InventoryManager.Instance.Slots)
        {
            if(slotData != null && slotData.ItemId == ingredientID)
            {
                amount = slotData.Amount;
                break;
            }
        }
        if (amount <= 0) return;
        foreach (KitchenCookSlotItem slot in slots)
        {
            if (slot.ingredientID == ingredientID)
            {
                if (slot.count >= amount)
                {
                    return;
                }
                slot.count++;
                uiBinder.RefreshExistingSlot(ingredientID);
                return;
            }
        }

        if (slots.Count >= maxSlot)
        {
            return;
        }

        slots.Add(new KitchenCookSlotItem(ingredientID, 1));
        uiBinder.ShowIngredientInEmptySlot(ingredientID);
    }

    public void ClearSlots()
    {
        slots.Clear();
        uiBinder.ResetAll();
    }

    public void ClearSolidSlotUIs()
    {
        uiBinder.ClearAllSolidSlots();
    }

    public void CookComplete()
    {
        slots.Clear();
    }
}
