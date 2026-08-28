using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] public KitchenCookingSlot[] solidSlotUIs;
    [SerializeField] public GameObject[] dashedSlotUIs;
    [SerializeField] public List<KitchenCookSlotItem> slots = new List<KitchenCookSlotItem>();

    public void AddIngredient(string ingredientID)
    {

        foreach (KitchenCookSlotItem slot in slots)
        {
            if (slot.ingredientID == ingredientID)
            {
                slot.count++;

                KitchenCookingSlot sameSlot = FindSameIngredient(ingredientID);
                if (sameSlot != null)
                {
                    sameSlot.AddIngredient(ingredientID);
                }
                return;
            }
        }

        if (slots.Count >= maxSlot)
        {
            return;
        }

        slots.Add(new KitchenCookSlotItem(ingredientID, 1));

        KitchenCookingSlot emptySlot = FindEmptySlot();
        if (emptySlot == null)
        {
            return;
        }

        emptySlot.gameObject.SetActive(true);
        emptySlot.SetIngredient(ingredientID);

        int index = -1;
        for (int i = 0; i < solidSlotUIs.Length; i++)
        {
            if (solidSlotUIs[i] == emptySlot)
            {
                index = i;
                break;
            }
        }
        if (index != -1)
        {
            dashedSlotUIs[index].SetActive(false);
        }
    }

    public void ClearSlots()
    {
        slots.Clear();
        for (int i = 0; i < solidSlotUIs.Length; i++)
        {
            solidSlotUIs[i].gameObject.SetActive(false);
            dashedSlotUIs[i].SetActive(true);
        }
    }
    public KitchenCookingSlot FindEmptySlot()
    {
        foreach (KitchenCookingSlot slot in solidSlotUIs)
        {
            if (slot.gameObject.activeSelf)
            {
                continue;
            }
            return slot;
        }
        return null;
    }
    public KitchenCookingSlot FindSameIngredient(string ingredientID)
    {
        foreach (KitchenCookingSlot slot in solidSlotUIs)
        {
            if (slot.ingredientID == ingredientID)
            {
                return slot;
            }
        }
        return null;
    }
    public void CookComplete()
    {
        slots.Clear();
    }
}
