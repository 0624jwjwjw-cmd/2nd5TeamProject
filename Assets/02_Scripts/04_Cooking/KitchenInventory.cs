using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KitchenInventory : MonoBehaviour
{
    [SerializeField] private KitchenInventorySlot[] slots;

    private void OnValidate()
    {
        slots = GetComponentsInChildren<KitchenInventorySlot>(true);
    }
    public void SetWhole()
    {
        for(int i=0; i<InventoryManager.Instance.SlotCount;i++)
        {
            slots[i].SetSlot(InventoryManager.Instance.Slots[i]);
        }
        for (int i = InventoryManager.Instance.SlotCount; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
    }
    public void SetIngredient()
    {
        int slotIndex = 0;

        for (int i = 0; i < InventoryManager.Instance.SlotCount; i++)
        {
            if (InventoryManager.Instance.Slots[i].ItemType == ItemType.Ingredient)
            {
                if (slotIndex < slots.Length)
                {
                    slots[slotIndex].SetSlot(InventoryManager.Instance.Slots[i]);
                    slotIndex++;
                }
            }
        }

        for (int i = slotIndex; i < slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
    }
    public void SetDish()
    {
        int slotIndex = 0;

        for(int i=0; i<InventoryManager.Instance.SlotCount; i++)
        {
            if (InventoryManager.Instance.Slots[i].ItemType == ItemType.Dish || InventoryManager.Instance.Slots[i].ItemType == ItemType.SpecialDish)
            {
                if (slotIndex < slots.Length)
                {
                    slots[slotIndex].SetSlot(InventoryManager.Instance.Slots[i]);
                    slotIndex++;
                }
            }
        }
        for(int i= slotIndex; i<slots.Length; i++)
        {
            slots[i].ClearSlot();
        }
    }
}
