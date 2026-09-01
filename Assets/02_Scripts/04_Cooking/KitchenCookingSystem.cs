using UnityEngine;

public class KitchenCookingSystem : MonoBehaviour
{
    [SerializeField] private KitchenCookingSlotManager kitchenCookingSlotManager;
    [SerializeField] private int specialRate;
    [SerializeField] private KitchenCookResult cookResult;
    [SerializeField] private string burnedDishID = "BD_01";
    [SerializeField] private string trashDishID = "TD_01";

    private readonly DishMatchingService dishMatchingService = new DishMatchingService();

    public void StartCook()
    {
        if (IsAllSlotsEmpty()) return;

        string resultDishID = dishMatchingService.FindMatchingDish(kitchenCookingSlotManager.slots);

        if (resultDishID == null)
        {
            ShowResultAndAddToInventory(trashDishID, ItemType.Dish, isSpecial: false);
        }
        else if (!ReciepeUnlockManager.Instance.IsUnlocked(resultDishID))
        {
            ShowResultAndAddToInventory(burnedDishID, ItemType.Dish, isSpecial: false);
        }
        else if (Random.Range(0, 100) < specialRate)
        {
            string specialDishID = dishMatchingService.FindMatchingSpecialDish(resultDishID);
            ShowResultAndAddToInventory(specialDishID, ItemType.SpecialDish, isSpecial: true);
        }
        else
        {
            ShowResultAndAddToInventory(resultDishID, ItemType.Dish, isSpecial: false);
        }

        ConsumeIngredients();
        kitchenCookingSlotManager.ClearSlots();
        kitchenCookingSlotManager.ClearSolidSlotUIs();
    }

    private bool IsAllSlotsEmpty()
    {
        foreach (KitchenCookSlotItem item in kitchenCookingSlotManager.slots)
        {
            if (item != null && item.ingredientID != null)
                return false;
        }
        return true;
    }

    private void ShowResultAndAddToInventory(string dishID, ItemType itemType, bool isSpecial)
    {
        cookResult.gameObject.SetActive(true);
        if (isSpecial)
            cookResult.SetResultSpecialDishInfo(dishID);
        else
            cookResult.SetResultDishInfo(dishID);

        InventoryManager.Instance.AddItem(dishID, 1, itemType);
    }

    private void ConsumeIngredients()
    {
        foreach (KitchenCookSlotItem slot in kitchenCookingSlotManager.slots)
        {
            InventoryManager.Instance.RemoveItem(slot.ingredientID, slot.count);
        }
    }
}
