using System;
using UnityEngine;

public class KitchenCookingSlotUIBinder : MonoBehaviour
{
    [SerializeField] private KitchenCookingSlot[] solidSlotUIs;
    [SerializeField] private GameObject[] dashedSlotUIs;

    public void ShowIngredientInEmptySlot(string ingredientID)
    {
        KitchenCookingSlot emptySlot = FindEmptySlot();
        if (emptySlot == null) return;

        emptySlot.gameObject.SetActive(true);
        emptySlot.SetIngredient(ingredientID);

        int index = Array.IndexOf(solidSlotUIs, emptySlot);
        if (index != -1)
            dashedSlotUIs[index].SetActive(false);
    }

    public void RefreshExistingSlot(string ingredientID)
    {
        KitchenCookingSlot sameSlot = FindSlotShowing(ingredientID);
        if (sameSlot != null)
            sameSlot.AddIngredient(ingredientID);
    }

    public void ResetAll()
    {
        for (int i = 0; i < solidSlotUIs.Length; i++)
        {
            solidSlotUIs[i].gameObject.SetActive(false);
            dashedSlotUIs[i].SetActive(true);
        }
    }

    public void ClearAllSolidSlots()
    {
        for (int i = 0; i < solidSlotUIs.Length; i++)
            solidSlotUIs[i].Clear();
    }

    private KitchenCookingSlot FindEmptySlot()
    {
        foreach (KitchenCookingSlot slot in solidSlotUIs)
        {
            if (!slot.gameObject.activeSelf)
                return slot;
        }
        return null;
    }

    private KitchenCookingSlot FindSlotShowing(string ingredientID)
    {
        foreach (KitchenCookingSlot slot in solidSlotUIs)
        {
            if (slot.ingredientID == ingredientID)
                return slot;
        }
        return null;
    }
}
